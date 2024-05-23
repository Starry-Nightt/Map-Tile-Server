namespace map_tile_server.Models.Configurations
{
    public interface IOsmDatabaseSettings
    {
        public string LocationDatabaseConnection { get; set; }
        public string RoutingDatabaseConnection { get; set; }
    }
}
