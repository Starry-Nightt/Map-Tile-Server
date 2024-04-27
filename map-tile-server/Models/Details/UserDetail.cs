using map_tile_server.Models.Entities;

namespace map_tile_server.Models.Details
{
    public class UserDetail
    {
        public string Id { get; set; } = String.Empty;
        public string Username { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string Role { get; set; } = String.Empty;

        public UserDetail(User user)
        {
            Id = user.Id;
            Username = user.Username;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Role = user.Role;
        }
    }
}
