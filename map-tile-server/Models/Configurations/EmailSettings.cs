namespace map_tile_server.Models.Configurations
{
    public class EmailSettings: IEmailSettings
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public string Displayname { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}
