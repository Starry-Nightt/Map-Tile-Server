namespace map_tile_server.Models.Entities
{
    public class Location
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Lng { get; set; }
        public double Lat { get; set; }
        public string? Amenity { get; set; } = string.Empty;
        public string? HouseName { get; set; } = string.Empty;
        public string? HouseNumber { get; set; } = string.Empty;
        public string? Place { get; set; } = string.Empty;
    }
}
