using map_tile_server.Models.Details;

namespace map_tile_server.Services
{
    public interface IOsmService
    {
        Task<RoutingDetail> GetRoute(double startLat, double startLng, double endLat, double endLng);
    }
}
