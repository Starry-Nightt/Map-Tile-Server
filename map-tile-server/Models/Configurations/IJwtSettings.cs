namespace map_tile_server.Models.Configurations
{
    public interface IJwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
