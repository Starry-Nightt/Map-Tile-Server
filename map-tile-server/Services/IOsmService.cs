using map_tile_server.Models.Details;
using map_tile_server.Models.Entities;

namespace map_tile_server.Services
{
    public interface IOsmService
    {
        Task<RoutingDetail?> GetRoute(double startLat, double startLng, double endLat, double endLng);
        Task<RoutingDetail?> GetRouteWalking(double startLat, double startLng, double endLat, double endLng);
        Task<List<Location>> SearchLocation(string? key);
    }
}
