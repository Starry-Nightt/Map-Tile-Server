namespace map_tile_server.Models.Configurations
{
    public class OsmDatabaseSettings: IOsmDatabaseSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
    }
}
