using map_tile_server.Filters;
using map_tile_server.Models.Commons;
using map_tile_server.Models.Details;
using map_tile_server.Models.Entities;
using map_tile_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;

namespace map_tile_server.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHelperService _helperService;

        public UserController(IUserService userService, IHelperService helperService)
        {
            _userService = userService;
            _helperService = helperService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Gets()
        {
            var users = _userService.Gets();
            return Ok(new SuccessDetail<List<UserDetail>>(users));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult GetById(string id)
        {

            var user = _userService.GetById(id);
            if (user == null)
            {
                var errorResponse = new ErrorDetail((int)HttpStatusCode.NotFound, $"User with Id = {id} not found");
                return NotFound(errorResponse);
            }
            return Ok(new SuccessDetail<UserDetail>(new UserDetail(user)));
        }


        [HttpPost]
        [Authorize(Roles = "admin")]
        [ServiceFilter(typeof(ValidationFilter))]
        public IActionResult Post([FromBody] UserCreateDetail detail)
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

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,user")]
        public IActionResult Put(string id, [FromBody] UserUpdateDetail detail)
        {

            var userTemp = _userService.GetById(id);
            if (userTemp == null)
            {
                var errorResponse = new ErrorDetail((int)HttpStatusCode.NotFound, $"User with Id = {id} not found");
                return NotFound(errorResponse);
            }

            if (detail.FirstName != null)
            {
                userTemp.FirstName = detail.FirstName;
            }
            if (detail.LastName != null)
            {
                userTemp.LastName = detail.LastName;
            }
            if (detail.Username != null && !IsUsernameExisting(detail.Username))
            {
                userTemp.Username = detail.Username;
            }
            if (detail.Password != null)
            {
                userTemp.Password = detail.Password;
            }

            _userService.Update(id, userTemp);
            return NoContent();

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin, user")]
        public IActionResult Delete(string id)
        {

            var userTemp = _userService.GetById(id);
            if (userTemp == null)
            {
                var errorResponse = new ErrorDetail((int)HttpStatusCode.NotFound,$"User with Id = {id} not found");
                return NotFound(errorResponse);
            }

            _userService.Delete(id);
            return Ok();
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
