using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using map_tile_server.Models.Details;
using Serilog;

namespace map_tile_server.Models.Entities
{
    public class Geo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;
        [BsonElement("geomertry")]
        public  Geometry Geometry { get; set; }
        [BsonElement("properties")]
        public  GeoProperties Properties { get; set; }
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        public Geo(string userId, GeoCreateDetail detail)
        {
            Log.Information("{@detail}", detail);
            UserId = userId;
            Geometry = detail.Geometry;
            Properties = detail.Properties;
            Type = detail.Type;

            Properties = new GeoProperties(Id, UserId);
            Properties.Radius = detail.Properties?.Radius;
        }

    }

    public class GeoProperties
    {
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;
        [BsonElement("userId")]
        public string UserId { get; set;} = string.Empty;
        [BsonElement("radius")]
        public double? Radius { get; set; }
        [BsonElement("body")]
        public GeoBodyDetail? Body { get; set; }

        public GeoProperties(string id, string userId)
        {
            Id = id; UserId = userId;
        }
    }

    public class Geometry
    {
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        [BsonElement("coordinates")]
        public string Coordinates { get; set; } = string.Empty;
    }
}
