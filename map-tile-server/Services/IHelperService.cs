namespace map_tile_server.Services
{
    public interface IHelperService
    {
        public string HashPassword(string password);
        public bool VerifyPassword(string password, string hash);
        public string GenerateRandomString();
    }
}
