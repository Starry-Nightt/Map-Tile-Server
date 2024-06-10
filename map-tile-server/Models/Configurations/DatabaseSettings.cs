namespace map_tile_server.Models.Configurations
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string UserCollectionName { get; set; } = string.Empty;
        public string GeoCollectionName { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string OtpCollectionName { get; set; } = string.Empty;
    }
}
