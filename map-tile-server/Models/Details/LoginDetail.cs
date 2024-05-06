using System.ComponentModel.DataAnnotations;

namespace map_tile_server.Models.Details
{
    public class LoginDetail
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Passowrd is required")]
        public string Password { get; set; } = string.Empty;
    }
}
