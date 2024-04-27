using System.ComponentModel.DataAnnotations;

namespace map_tile_server.Models.Details
{
    public class UserCreateDetail
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = String.Empty;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = String.Empty;
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = String.Empty;
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; } = String.Empty;
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = String.Empty;
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = String.Empty;
    }
}
