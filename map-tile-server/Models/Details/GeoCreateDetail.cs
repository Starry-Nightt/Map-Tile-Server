using map_tile_server.Models.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace map_tile_server.Models.Details
{
    public class GeoCreateDetail
    {
        public  Geometry Geometry { get; set; }
        public  GeoProperties Properties { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
