using map_tile_server.Models.Configurations;
using map_tile_server.Models.Details;
using map_tile_server.Models.Entities;
using MongoDB.Driver;
using Serilog;
using System.Text.RegularExpressions;

namespace map_tile_server.Services
{
    public class MapService : IMapService
    {
        private readonly HttpClient _httpClient;
        private readonly string _prefix;
        private readonly IMongoCollection<Geo> _geoCollection;
        private readonly IMongoCollection<Location> _locationCollection;
        public MapService(ITileServerSettings tileServerSettings, IDatabaseSettings databaseSettings, IMongoClient mongoClient)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(tileServerSettings.ConnectionString);
            _prefix = tileServerSettings.Prefix;
            var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);
            _geoCollection = database.GetCollection<Geo>(databaseSettings.GeoCollectionName);
            _locationCollection = database.GetCollection<Location>(databaseSettings.LocationCollectionName);
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

        public List<Location> GetLocations(string key)
        {
            string[] words = key.ToLower().Split(' ').ToArray();
            if (words.Length == 0) return new List<Location>();
            List<Location> locations = _locationCollection.Find(FilterDefinition<Location>.Empty).ToList();
            List<Location> locaitonsWithScore = locations.Select(l => LocationScoreByKey(words, l)).ToList();
            GFG gg = new GFG();
            Log.Information("{@locations} {@key}", locations, key);
            locaitonsWithScore.Sort(gg);
            return locaitonsWithScore.Take(5).ToList();

        }

        private Location LocationScoreByKey(string[] words, Location l)
        {
            string[] cityKey = l.City.ToLower().Split(' ').ToArray();
            int cnt = 0;
            foreach (string word in words)
            {
                foreach (string key in cityKey)
                {
                    if (key.StartsWith(word)) cnt++;
                }
            }
            l.Score = cnt;
            return l;
        }
    }

    class GFG : IComparer<Location>
    {
        public int Compare(Location x, Location y)
        {
            if (x == null || y == null || x.Score == null || y.Score == null)
            {
                return 0;
            }
            int scoreX = (int)x.Score;
            int scoreY = (int)y.Score;

            return scoreY.CompareTo(scoreX);

        }
    }
}
