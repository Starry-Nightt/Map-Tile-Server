namespace map_tile_server.Models.Entities
{
    public class Location
    {
        public string Id { get; set; } = String.Empty;
        public string Name { get; set; } = string.Empty;
        public double Lng { get; set; }
        public double Lat { get; set; }
    }
}
