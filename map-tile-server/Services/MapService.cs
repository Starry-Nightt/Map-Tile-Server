using map_tile_server.Models.Configurations;

namespace map_tile_server.Services
{
    public class MapService : IMapService
    {
        private readonly HttpClient _httpClient;
        public MapService(ITileServerSettings settings)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(settings.ConnectionString + "/" + settings.Prefix);
        }

        public async Task<byte[]> GetTile(int z, int x, int y)
        {

            HttpResponseMessage response = await _httpClient.GetAsync($"/{z}/{x}/{y}.png");
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
