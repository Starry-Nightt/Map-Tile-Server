using map_tile_server.Filters;
using map_tile_server.Models.Commons;
using map_tile_server.Models.Configurations;
using map_tile_server.Models.Details;
using map_tile_server.Models.Entities;
using map_tile_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace map_tile_server.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IJwtSettings _jwtSettings;
        private readonly IHelperService _helperService;
        private readonly IEmailService _emailService;
        public AuthController(IJwtSettings settings, IUserService userService, IHelperService helperService, IEmailService emailService)
        {
            _userService = userService;
            _jwtSettings = settings;
            _helperService = helperService;
            _emailService = emailService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidationFilter))]
        public IActionResult Login([FromBody] LoginDetail detail)
        {
            var userTmp = _userService.GetByEmail(detail.Email);
            if (userTmp == null)
            {
                return Unauthorized(new ErrorDetail((int)HttpStatusCode.Unauthorized, "Email is not registered"));
            }
            var user = Authenticate(detail);
            if (user != null)
            {
                var tokenString = GenerateToken(user);
                return Ok(new LoginSuccessDetail(tokenString));
            }
            return Unauthorized(new ErrorDetail((int)HttpStatusCode.Unauthorized, "Email or password is not correct"));
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidationFilter))]
        public IActionResult Register([FromBody] RegisterDetail detail)
        {
            if (IsEmailExisting(detail.Email))
            {
                return BadRequest(new ErrorDetail((int)HttpStatusCode.BadRequest, "Email was used"));
            }
            if (IsUsernameExisting(detail.Username))
            {
                return BadRequest(new ErrorDetail((int)HttpStatusCode.BadRequest, "Username was used"));
            }
            detail.Password = _helperService.HashPassword(detail.Password);
            var _user = new User(detail);
            var newUser = _userService.Create(_user);
            return Ok(new SuccessDetail<UserDetail>(newUser));
        }

        [HttpPost("me")]
        [AllowAnonymous]
        public IActionResult GetUserInfo()
        {
            var userInfo = GetCurrentUser();
            if (userInfo == null || userInfo.Id == null) return Unauthorized(new ErrorDetail((int)HttpStatusCode.Unauthorized, "Invalid token"));
            User user = _userService.GetById(userInfo.Id);
            var userDetail = new UserDetail(user);
            return Ok(new SuccessDetail<UserDetail>(userDetail));
        }

        [HttpPost("change-password")]
        [Authorize(Roles = "admin,user")]
        [ServiceFilter(typeof(ValidationFilter))]
        public IActionResult ResetPassword([FromBody] ChangePasswordDetail detail)
        {
            var userDetail = GetCurrentUser();
            if (userDetail == null) return Unauthorized(new ErrorDetail((int)HttpStatusCode.Unauthorized, "Invalid token"));
            var user = Authenticate(new LoginDetail { Email = userDetail.Email, Password = detail.OldPassword });
            if (user == null) return Unauthorized(new ErrorDetail((int)HttpStatusCode.Unauthorized, "Your old password is not correct"));
            
            user.Password = _helperService.HashPassword(detail.NewPassword);
            _userService.Update(userDetail.Id, user);
            return NoContent();
        }

        [HttpPost("forget-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDetail detail)
        {
            var user = _userService.GetByEmail(detail.Email);
            if (user == null)
            {
                var errorResponse = new ErrorDetail((int)HttpStatusCode.NotFound, $"User with email = {detail.Email} not found");
                return NotFound(errorResponse);
            }
            var tokenString = GenerateToken(user);
            var otp = _userService.CreateOtp(user.Email);
            MailRequest mail = new MailRequest
            {
                ToEmail = detail.Email,
                Subject = $"OTP code: {otp.Code}",
                Body = $"<p>Hello {user.Username}</p><p>This is your otp code: {otp.Code}</p>"
            };

            await _emailService.SendEmailForgetPassword(mail);
            return Ok(new SuccessDetail<string>(tokenString));
        }

        [HttpPost("validate-otp")]
        [AllowAnonymous]
        public IActionResult ValidateOtp([FromBody] ValidateOtpDetail detail)
        {
            var userInfo = GetCurrentUser();
            if (userInfo == null || userInfo.Id == null) return Unauthorized(new ErrorDetail((int)HttpStatusCode.Unauthorized, "Invalid token"));
            var email = userInfo.Email;
            bool valid = _userService.ValidateOtp(email, detail.Otp);
            if (valid)
            {
                _userService.DeleteOtp(email, detail.Otp);
                return Ok(new SuccessDetail<string?>(null));
            }
            return Unauthorized(new ErrorDetail((int)HttpStatusCode.Unauthorized, "Wrong Otp code"));
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public IActionResult ResetPassword([FromBody] ResetPasswordDetail detail)
        {
            var userDetail = GetCurrentUser();
            if (userDetail == null) return Unauthorized(new ErrorDetail((int)HttpStatusCode.Unauthorized, "Invalid token"));
            var user = _userService.GetById(userDetail.Id);

            user.Password = _helperService.HashPassword(detail.NewPassword);
            _userService.Update(userDetail.Id, user);
            return NoContent();
        }


        private User? Authenticate(LoginDetail detail)
        {

            var user = _userService.GetByEmail(detail.Email);
            if (user == null) return null;

            bool isCorrectPassword = _helperService.VerifyPassword(detail.Password, user.Password);
            return isCorrectPassword ? user : null;
        }

        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var token = new JwtSecurityToken(_jwtSettings.Issuer,
              _jwtSettings.Audience,
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimDetail? GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null || IsTokenExpired())
            {
                var claims = identity.Claims;
                return new ClaimDetail
                {
                    Id = claims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    Email = claims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                    Role = claims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value
                };
            }
            return null;
        }

        private bool IsEmailExisting(string email)
        {
            var userTemp = _userService.GetByEmail(email);
            if (userTemp == null) return false;
            return true;
        }

        private bool IsUsernameExisting(string email)
        {
            var userTemp = _userService.GetByUsername(email);
            if (userTemp == null) return false;
            return true;
        }

        private bool IsTokenExpired()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null) return true;
            var claim = identity.Claims;
            var tokenExp = claim.FirstOrDefault(claim => claim.Type.Equals("exp")).Value;
            var ticks = long.Parse(tokenExp);
            var tokenDate = DateTimeOffset.FromUnixTimeSeconds(ticks).UtcDateTime;
            var now = DateTime.Now.ToUniversalTime();
            return tokenDate < now;
        }
    }
}
