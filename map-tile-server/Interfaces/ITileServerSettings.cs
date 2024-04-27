namespace map_tile_server.Interfaces
{
    public interface ITileServerSettings
    {
        public string ConnectionString { get; set; }
        public string Prefix { get; set; }
    }
}
