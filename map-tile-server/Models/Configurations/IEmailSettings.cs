namespace map_tile_server.Models.Configurations
{
    public interface IEmailSettings
    {
        public string Email { get; set; } 
        public string Password { get; set; } 
        public string Host { get; set; }
        public string Displayname { get; set; }
        public int Port { get; set; }
    }
}
