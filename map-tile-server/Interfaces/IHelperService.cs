namespace map_tile_server.Interfaces
{
    public interface IHelperService
    {
        public string HashPassword(string password);
        public bool VerifyPassword(string hash, string password);
    }
}
