using System.ComponentModel.DataAnnotations;

namespace map_tile_server.Models.Details
{
    public class ChangePasswordDetail
    {
        [Required(ErrorMessage = "Old password is required")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "New password is required")]
        public string NewPassword { get; set; }
    }
}
