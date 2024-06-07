namespace map_tile_server.Services
{
    public interface IEmailService
    {
        void SendEmailForgotPassword(string email, string username);
    }
}
