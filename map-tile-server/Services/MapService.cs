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
            string[] splits = Regex.Split(key, @"(\s+|\p{P})").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            string[] words = splits.Where(x => Regex.IsMatch(x, "^[a-zA-Z]+$")).Select(word => word.ToLower()).ToArray();
            List<Location> locations = _locationCollection.Find(l => words.Length != 0).ToList();
            List<Location> res = locations.Where(l => IsMatchCity(words, l)).ToList();
            return res;

        }

        private bool IsMatchCity(string[] words, Location location)
        {
            string[] wordsCity = location.City.Split(' ').Select(w => w.ToLower()).ToArray();
            string[] wordsAdminName = location.AdminName.Split(' ').Select(w => w.ToLower()).ToArray();
            foreach (string word in words)
            {
                if (IsCityContainWord(wordsCity, word)) {
                    return true;
                }

            }
            return false;
        }

        private bool IsCityContainWord(string[] arr, string keyword)
        {
            foreach (string str in arr)
            {
                if (str.StartsWith(keyword)) return true;
            }
            return false;
        }
    }
}
