using map_tile_server.Models.Configurations;
using map_tile_server.Models.Details;
using map_tile_server.Models.Entities;
using Npgsql;
using Serilog;
using System.Text.Json;

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

        public async Task<List<Location>> SearchLocation(string? key)
        {
            var locations = new List<Location>();
            if (key == null || key.Trim().Length == 0) return locations;
            using (var connection = new NpgsqlConnection(_locationDatabaseConnection))
            {
                await connection.OpenAsync();
                string query = $"SELECT DISTINCT ON (name) name, cast(osm_id as text) AS id, ST_X(ST_Transform(way, 4326)) AS longitude, ST_Y(ST_Transform(way, 4326)) AS latitude\r\nFROM planet_osm_point where name like '%{key}%' order by name limit 6";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string name = reader.IsDBNull(0) ? null : reader.GetString(0);
                            string id = reader.IsDBNull(1) ? null : reader.GetString(1);
                            double? longitude = reader.IsDBNull(2) ? null : reader.GetDouble(2);
                            double? latitude = reader.IsDBNull(3) ? null : reader.GetDouble(3);

                            if (longitude == null || latitude == null || id == null) continue;

                            var location = new Location
                            {
                                Id = id,
                                Name = name,
                                Lng = (double)longitude,
                                Lat = (double)latitude
                            };

                            locations.Add(location);
                        }
                    }
                }
            }
            return locations;
        }

        public async Task<string> GetRoute(double startLat, double startLng, double endLat, double endLng)
        {
            using (var connection = new NpgsqlConnection(_routingDatabaseConnection))
            {
                await connection.OpenAsync();
                string query = $"WITH start AS (\r\n  SELECT topo.source -- could also be topo.target\r\n  FROM at_2po_4pgr as topo\r\n  ORDER BY topo.geom_way <-> ST_SetSRID(\r\n    ST_MakePoint({startLng},{startLat}),\r\n  4326)\r\n  LIMIT 1\r\n),destination AS (\r\n  SELECT topo.source --could also be topo.target\r\n  FROM at_2po_4pgr as topo\r\n  ORDER BY topo.geom_way <-> ST_SetSRID(\r\n    ST_MakePoint({endLng}, {endLat}),\r\n  4326)\r\n  LIMIT 1\r\n)" +
                    $"SELECT  ST_AsGeoJSON(ST_Transform(ST_Union(geom_way), 4326)) as geom\r\nFROM pgr_dijkstra('\r\n    SELECT id,\r\n         source,\r\n         target,\r\n        cost,\r\n        reverse_cost\r\n        FROM at_2po_4pgr as e,\r\n    (SELECT ST_Expand(ST_Extent(geom_way),0.1) as box FROM at_2po_4pgr as b\r\n        WHERE b.source = '|| (SELECT source FROM start) ||'\r\n        OR b.source = ' || (SELECT source FROM destination) || ') as box WHERE e.geom_way && box.box'\r\n    ,\r\n    array(SELECT source FROM start),\r\n    array(SELECT source FROM destination),\r\n    directed := true) AS di\r\nJOIN   at_2po_4pgr AS pt\r\n  ON   di.edge = pt.id;";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            string geoData = reader.GetString(0);
                            return geoData;
                        }
                    }
                }
            }
            return "";

        }
    }
}
