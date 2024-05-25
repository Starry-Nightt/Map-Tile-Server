using map_tile_server.Models.Commons;
using map_tile_server.Models.Details;
using map_tile_server.Models.Entities;
using map_tile_server.Services;
using Microsoft.AspNetCore.Mvc;

namespace map_tile_server.Controllers
{
    [Route("api/osm")]
    public class OsmController : Controller
    {
        private readonly IOsmService _osmService;

        public OsmController(IOsmService osmService)
        {
            _osmService = osmService;
        }

        [HttpGet("location")]
        public async Task<IActionResult> SearchLocation(string? key)
        {
            var result = await _osmService.SearchLocation(key);
            return Ok(new SuccessDetail<List<Location>>(result));
        }

        [HttpPost("routing")]
        public async Task<IActionResult> GetRoute([FromBody] RoutingRequestDetail detail)
        {
            var result = await _osmService.GetRoute(detail.startLat, detail.startLng, detail.endLat, detail.endLng);
            return Ok(new SuccessDetail<string>(result));
        }
    }
}
