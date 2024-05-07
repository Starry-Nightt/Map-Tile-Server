using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace map_tile_server.Models.Entities
{
    public class Location
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("lat")]
        public string Lat { get; set; } = string.Empty;

        [BsonElement("lng")]
        public string Lng { get; set; } = string.Empty;

        [BsonElement("country")]
        public string Country { get; set; } = string.Empty;

        [BsonElement("iso2")]
        public string Iso2 { get; set; } = string.Empty;

        [BsonElement("admin_name")]
        public string AdminName { get; set; } = string.Empty;

        [BsonElement("capital")]
        public string Capital { get; set; } = string.Empty;

        [BsonElement("population")]
        public string Population { get; set; } = string.Empty;

        [BsonElement("population_proper")]
        public string PopulationProper { get; set; } = string.Empty;
        [BsonElement("score")]
        public int? Score { get; set; }
    }
}
