using System.Text;

namespace map_tile_server.Services
{
    public class HelperService: IHelperService
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public string GenerateRandomString()
        {
            string chars = "abcdefghijklmnopqrstuvwxyz1234567890";
            StringBuilder sb = new StringBuilder();
            Random rand = new Random();
            for (int i = 0; i < 6; i++)
            {
                int index = rand.Next(chars.Length);
                sb.Append(chars[index]);
            }
            return sb.ToString();
        }
    }
}
