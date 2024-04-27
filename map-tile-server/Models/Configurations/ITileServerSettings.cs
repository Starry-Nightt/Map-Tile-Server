namespace map_tile_server.Models.Configurations
{
    public interface ITileServerSettings
    {
        public string ConnectionString { get; set; }
        public string Prefix { get; set; }
    }
}
