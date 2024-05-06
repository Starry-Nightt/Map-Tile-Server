using map_tile_server.Models.Details;
using map_tile_server.Models.Entities;

namespace map_tile_server.Services
{
    public interface IMapService
    {
        Task<byte[]> GetTile(int z, int x, int y);

        List<Geo> GetsByUser(string userId);

        Geo Create(string userId, GeoCreateDetail detail);

        void Delete(string id);

        void DeleteAll(string userId);
        List<Location> GetLocations(string key);
    }
}
