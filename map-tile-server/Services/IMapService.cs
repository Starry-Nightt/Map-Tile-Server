namespace map_tile_server.Services
{
    public interface IMapService
    {
        public Task<byte[]> GetTile(int z, int x, int y);
    }
}
