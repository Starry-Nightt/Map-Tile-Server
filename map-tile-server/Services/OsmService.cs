using map_tile_server.Models.Configurations;
using map_tile_server.Models.Details;
using map_tile_server.Models.Entities;
using Microsoft.AspNetCore.Mvc.Razor;
using Npgsql;
using Serilog;
using System.Text.Json;

namespace map_tile_server.Services
{
    public class OsmService: IOsmService
    {
        private readonly string _osmDatabaseConnectionString;

        public OsmService(IOsmDatabaseSettings osmDatabaseSettings)
        {
            _osmDatabaseConnectionString = osmDatabaseSettings.ConnectionString;
        }

        public async Task<List<Location>> SearchLocation(string? key)
        {
            var locations = new List<Location>();
            if (key == null || key.Trim().Length == 0) return locations;
            using (var connection = new NpgsqlConnection(_osmDatabaseConnectionString))
            {
                await connection.OpenAsync();
                string query = $"SELECT name, cast(osm_id as text) AS id, ST_X(ST_Transform(way, 4326)) AS longitude, ST_Y(ST_Transform(way, 4326)) AS latitude, amenity, addrhousename, addrhousenumber, place " +
                    $"FROM planet_osm_point where name ilike '%{key.Trim()}%' order by place limit 5";
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
                            string? amenity = reader.IsDBNull(4) ? null : reader.GetString(4);
                            string? houseName = reader.IsDBNull(5) ? null : reader.GetString(5);
                            string? houseNumber = reader.IsDBNull(6) ? null : reader.GetString(6);
                            string? place = reader.IsDBNull(7) ? null : reader.GetString(7);
                            if (longitude == null || latitude == null || id == null) continue;

                            var location = new Location
                            {
                                Id = id,
                                Name = name,
                                Lng = (double)longitude,
                                Lat = (double)latitude,
                                Amenity = amenity,
                                HouseName = houseName,  
                                HouseNumber = houseNumber,
                                Place = place
                            };

                            locations.Add(location);
                        }
                    }
                }
            }
            return locations;
        }

        public async Task<RoutingDetail?> GetRoute(double startLat, double startLng, double endLat, double endLng)
        {
            using (var connection = new NpgsqlConnection(_osmDatabaseConnectionString))
            {
                await connection.OpenAsync();
                string query = $"WITH start AS (\r\n  SELECT topo.source -- could also be topo.target\r\n  FROM at_2po_4pgr as topo\r\n  ORDER BY topo.geom_way <-> ST_SetSRID(\r\n    ST_MakePoint({startLng},{startLat}),\r\n  4326)\r\n  LIMIT 1\r\n),destination AS (\r\n  SELECT topo.source --could also be topo.target\r\n  FROM at_2po_4pgr as topo\r\n  ORDER BY topo.geom_way <-> ST_SetSRID(\r\n    ST_MakePoint({endLng}, {endLat}),\r\n  4326)\r\n  LIMIT 1\r\n)" +
                    $"SELECT  ST_AsGeoJSON(ST_Transform(ST_Union(geom_way), 4326)) as geom, SUM(pt.km) AS total_distance\r\nFROM pgr_dijkstra('\r\n    SELECT id,\r\n         source,\r\n         target,\r\n        cost,\r\n        reverse_cost\r\n        FROM at_2po_4pgr as e,\r\n    (SELECT ST_Expand(ST_Extent(geom_way),0.1) as box FROM at_2po_4pgr as b\r\n        WHERE b.source = '|| (SELECT source FROM start) ||'\r\n        OR b.source = ' || (SELECT source FROM destination) || ') as box WHERE e.geom_way && box.box'\r\n    ,\r\n    array(SELECT source FROM start),\r\n    array(SELECT source FROM destination),\r\n    directed := true) AS di\r\nJOIN   at_2po_4pgr AS pt\r\n  ON   di.edge = pt.id;";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            string geoData = reader.GetString(0);
                            double distance = reader.GetDouble(1);
                            return new RoutingDetail { Geo = geoData, Distance = distance};
                        }
                    }
                }
            }
            return null;

        }

        public async Task<RoutingDetail?> GetRouteWalking(double startLat, double startLng, double endLat, double endLng)
        {
            using (var connection = new NpgsqlConnection(_osmDatabaseConnectionString))
            {
                await connection.OpenAsync();
                string query = $"WITH start AS (\r\n  SELECT topo.source -- could also be topo.target\r\n  FROM at_2po_4pgr as topo\r\n  ORDER BY topo.geom_way <-> ST_SetSRID(\r\n    ST_MakePoint({startLng},{startLat}),\r\n  4326)\r\n  LIMIT 1\r\n),destination AS (\r\n  SELECT topo.source --could also be topo.target\r\n  FROM at_2po_4pgr as topo\r\n  ORDER BY topo.geom_way <-> ST_SetSRID(\r\n    ST_MakePoint({endLng}, {endLat}),\r\n  4326)\r\n  LIMIT 1\r\n)" +
                    "SELECT  ST_AsGeoJSON(ST_Transform(ST_Union(geom_way), 4326)) as geom, SUM(pt.km)\r\nFROM pgr_dijkstra('\r\n    SELECT id,\r\n         source,\r\n         target,\r\n        cost_updated AS cost\r\n        FROM at_2po_4pgr as e ,\r\n    (SELECT ST_Expand(ST_Extent(geom_way),0.1) as box FROM at_2po_4pgr as b\r\n        WHERE b.source = '|| (SELECT source FROM start) ||'\r\n        OR b.source = ' || (SELECT source FROM destination) || ') as box WHERE e.geom_way && box.box and e.cost_updated notnull'\r\n    ,\r\n    array(SELECT source FROM start),\r\n    array(SELECT source FROM destination),\r\n    directed := false) AS di\r\nJOIN   at_2po_4pgr AS pt\r\n  ON   di.edge = pt.id";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            string geoData = reader.GetString(0);
                            double distance = reader.GetDouble(1);
                            return new RoutingDetail { Geo = geoData, Distance = distance };
                        }
                    }
                }
            }
            return null;

        }
    }
}
