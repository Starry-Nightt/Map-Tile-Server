﻿using map_tile_server.Models.Commons;
using map_tile_server.Models.Details;
using map_tile_server.Models.Entities;
using map_tile_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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

        [HttpGet("{z}/{x}/{y}.png")]
        [Authorize(Roles = "user, admin")]
        public async Task<IActionResult> GetTile(int z, int x, int y)
        {
            byte[] tile = await _mapService.GetTile(z, x, y);
            if (tile != null)
            {
                return File(tile, "image/png");
            }
            return NotFound();
        }

        [HttpGet("{userId}")]
        public IActionResult Gets(string userId)
        {
            var geos = _mapService.GetsByUser(userId);
            return Ok(new SuccessDetail<List<Geo>>(geos));
        }

        [HttpPost("{userId}")]
        public IActionResult Post(string userId, [FromBody] GeoCreateDetail detail)
        {
            var geoData = _mapService.Create(userId, detail);
            return Ok(new SuccessDetail<Geo>(geoData));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            _mapService.Delete(id);
            return Ok();
        }

        [HttpDelete("clear/{userId}")]
        public IActionResult DeleteAllByUserId(string userId)
        {
            _mapService.DeleteAll(userId);
            return Ok();
        }
    }
}