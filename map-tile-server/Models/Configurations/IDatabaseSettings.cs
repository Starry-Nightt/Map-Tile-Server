namespace map_tile_server.Models.Configurations
{
    public interface IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string UserCollectionName { get; set; }
        public string GeoCollectionName { get; set; }
        public string OtpCollectionName { get; set; }
    }
}
