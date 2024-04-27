using map_tile_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace map_tile_server.Controllers
{
    [Route("api/map")]
    [ApiController]
    public class MapController : Controller
    {
        private readonly IMapService _mapService;
        public MapController(IMapService mapService)
        {
            _mapService = mapService;

        }

        [Authorize(Roles = "user, admin")]
        [HttpGet("{z}/{x}/{y}.png")]
        public async Task<IActionResult> GetTile(int z, int x, int y)
        {
            byte[] tile = await _mapService.GetTile(z, x, y);
            if (tile != null)
            {
                return File(tile, "image/png");
            }
            return NotFound();
        }
    }
}
