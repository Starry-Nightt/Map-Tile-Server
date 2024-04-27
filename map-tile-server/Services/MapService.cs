using map_tile_server.Models.Configurations;
using Serilog;

namespace map_tile_server.Services
{
    public class MapService : IMapService
    {
        private readonly HttpClient _httpClient;
        private readonly string _prefix;
        public MapService(ITileServerSettings settings)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(settings.ConnectionString);
            _prefix = settings.Prefix;
        }

        public async Task<byte[]> GetTile(int z, int x, int y)
        {

            HttpResponseMessage response = await _httpClient.GetAsync($"/{_prefix}/{z}/{x}/{y}.png");
            Log.Information("{@url}", response.RequestMessage.RequestUri.ToString());
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
    }
}
