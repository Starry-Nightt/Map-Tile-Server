using map_tile_server.Models.Configurations;
using map_tile_server.Models.Details;
using map_tile_server.Models.Entities;
using MongoDB.Driver;
using Serilog;

namespace map_tile_server.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserService(IDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _usersCollection = database.GetCollection<User>(settings.UserCollectionName);
        }
        public List<UserDetail> Gets()
        {
            List<User> users = _usersCollection.Find(FilterDefinition<User>.Empty).ToList();
            return users.Select(u => new UserDetail(u)).ToList();
        }
        public User? GetById(string id)
        {
            var user = _usersCollection.Find(u => u.Id == id).FirstOrDefault();
            return user;
        }
        public User? GetByEmail(string email)
        {
            var user = _usersCollection.Find(u => u.Email == email).FirstOrDefault();
            return user;
        }

        public UserDetail? GetByEmailAndPassword(LoginDetail detail)
        {
            var user = _usersCollection.Find(u => u.Email == detail.Email && u.Password == detail.Password).FirstOrDefault();
            return user != null ? new UserDetail(user) : null;
        }
        public UserDetail? GetByUsername(string username)
        {
            var user = _usersCollection.Find(u => u.Username == username).FirstOrDefault();
            return user != null ? new UserDetail(user) : null;
        }
        public UserDetail Create(User user)
        {
            _usersCollection.InsertOne(user);
            return new UserDetail(user);
        }
        public void Update(string id, User user)
        {
            _usersCollection.ReplaceOne(u => u.Id == id, user);
        }
        public void Delete(string id)
        {
            _usersCollection.DeleteOne(u => u.Id == id);
        }

    }
}
