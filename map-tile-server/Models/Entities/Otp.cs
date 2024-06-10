using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace map_tile_server.Models.Entities
{
    public class Otp
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;
        [BsonElement("otp")]
        public string Code { get; set; } = string.Empty;
    }
}
