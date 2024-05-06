using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using map_tile_server.Models.Details;

namespace map_tile_server.Models.Entities
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;
        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;
        [BsonElement("role")]
        public string Role { get; set; } = string.Empty;
        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;
        [BsonElement("first_name")]
        public string FirstName { get; set; } = string.Empty;
        [BsonElement("last_name")]
        public string LastName { get; set; } = string.Empty;

        public User(UserCreateDetail data)
        {
            this.Email = data.Email;
            this.Password = data.Password;
            this.Role = data.Role;
            this.Username = data.Username;
            this.FirstName = data.FirstName;
            this.LastName = data.LastName;
        }
    }
}
