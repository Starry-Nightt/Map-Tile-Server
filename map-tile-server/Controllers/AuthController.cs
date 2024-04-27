using map_tile_server.Filters;
using map_tile_server.Models.Commons;
using map_tile_server.Models.Configurations;
using map_tile_server.Models.Details;
using map_tile_server.Models.Entities;
using map_tile_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        public AuthController(IJwtSettings settings, IUserService userService, IHelperService helperService)
        {
            _userService = userService;
            _jwtSettings = settings;
            _helperService = helperService;
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
                return Ok(new SuccessDetail<LoginSuccessDetail>(new LoginSuccessDetail { Token = tokenString}) );
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
            var _user = new User(new UserCreateDetail(detail));
            var newUser = _userService.Create(_user);
            return Ok(new SuccessDetail<UserDetail>(newUser));
        }

        [HttpPost("me")]
        [AllowAnonymous]
        public IActionResult GetUserInfo()
        {
            var userInfo = GetCurrentUser();
            if (userInfo == null) return Unauthorized(new ErrorDetail((int)HttpStatusCode.Unauthorized, "Invalid token"));
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
            if (identity != null)
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
    }
}
