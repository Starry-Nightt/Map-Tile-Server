using map_tile_server.Interfaces;

namespace map_tile_server.Models.Configurations
{
    public class TileServerSettings: ITileServerSettings
    {
        public string ConnectionString { get; set; } = String.Empty;
        public string Prefix { get; set; } = String.Empty;
    }
}
