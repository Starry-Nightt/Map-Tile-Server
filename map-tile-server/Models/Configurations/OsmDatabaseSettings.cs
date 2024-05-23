namespace map_tile_server.Models.Configurations
{
    public class OsmDatabaseSettings: IOsmDatabaseSettings
    {
        public string LocationDatabaseConnection { get; set; } = string.Empty;
        public string RoutingDatabaseConnection { get; set; } = string.Empty;   
    }
}
