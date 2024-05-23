using map_tile_server.Models.Configurations;
using map_tile_server.Models.Details;
using Npgsql;
using Serilog;

namespace map_tile_server.Services
{
    public class OsmService: IOsmService
    {
        private readonly string _routingDatabaseConnection;
        private readonly string _locationDatabaseConnection;

        public OsmService(IOsmDatabaseSettings osmDatabaseSettings)
        {
            _locationDatabaseConnection = osmDatabaseSettings.LocationDatabaseConnection;
            _routingDatabaseConnection = osmDatabaseSettings.RoutingDatabaseConnection;
        }
        public async Task<RoutingDetail> GetRoute(double startLat, double startLng, double endLat, double endLng)
        {
            Log.Information("{@connection}", _routingDatabaseConnection);
            using (var connection = new NpgsqlConnection(_routingDatabaseConnection))
            {
                await connection.OpenAsync();
                string query = $"SELECT ST_AsGeoJSON(ST_Union((the_geom))) FROM ways WHERE gid in (SELECT edge FROM pgr_dijkstra('SELECT gid as id, source, target, length AS cost FROM ways', (SELECT id FROM ways_vertices_pgr ORDER BY the_geom <-> ST_SetSRID(ST_Point({startLat}, {startLng}), 4326) LIMIT 1), (SELECT id FROM ways_vertices_pgr ORDER BY the_geom <-> ST_SetSRID(ST_Point({endLat}, {endLng}), 4326) LIMIT 1), directed := true) foo)";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var results = new RoutingDetail();


                        return results;

                    }
                }
            }

        }
    }
}
