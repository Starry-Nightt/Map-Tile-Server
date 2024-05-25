using map_tile_server.Models.Entities;

namespace map_tile_server.Services
{
    public interface IOsmService
    {
        Task<string> GetRoute(double startLat, double startLng, double endLat, double endLng);
        Task<List<Location>> SearchLocation(string? key);
    }
}
