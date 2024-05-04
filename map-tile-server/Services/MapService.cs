using map_tile_server.Models.Configurations;
using map_tile_server.Models.Details;
using map_tile_server.Models.Entities;
using MongoDB.Driver;
using Serilog;

namespace map_tile_server.Services
{
    public class MapService : IMapService
    {
        private readonly HttpClient _httpClient;
        private readonly string _prefix;
        private readonly IMongoCollection<Geo> _geoCollection;
        public MapService(ITileServerSettings tileServerSettings, IDatabaseSettings databaseSettings, IMongoClient mongoClient)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(tileServerSettings.ConnectionString);
            _prefix = tileServerSettings.Prefix;
            var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);
            _geoCollection = database.GetCollection<Geo>(databaseSettings.GeoCollectionName);
        }

        public async Task<byte[]> GetTile(int z, int x, int y)
        {

            HttpResponseMessage response = await _httpClient.GetAsync($"/{_prefix}/{z}/{x}/{y}.png");
            response.EnsureSuccessStatusCode();

            using (Stream contentStream = await response.Content.ReadAsStreamAsync())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await contentStream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        public List<Geo> GetsByUser(string userId)
        {
            List<Geo> geos = _geoCollection.Find(g => g.UserId == userId).ToList();
            return geos;
        }
        public Geo Create(string userId, GeoCreateDetail detail)
        {
            var geo = new Geo(userId, detail);
            _geoCollection.InsertOne(geo);
            return geo;
        }

        public void Delete(string id)
        {
            _geoCollection.DeleteOne(g => g.Id == id);
        }

        public void DeleteAll(string userId)
        {
            _geoCollection.DeleteMany(g => g.UserId == userId);
        }

    }
}
