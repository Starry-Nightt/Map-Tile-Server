using map_tile_server.Models.Details;
using map_tile_server.Models.Entities;

namespace map_tile_server.Interfaces
{
    public interface IUserService
    {
        List<UserDetail> Gets();
        User? GetByEmail(string email);
        User? GetById(string id);
        UserDetail? GetByEmailAndPassword(LoginDetail detail);
        UserDetail? GetByUsername(string email);
        UserDetail Create(User user);
        void Update(string id, User user);
        void Delete(string id);
    }
}
