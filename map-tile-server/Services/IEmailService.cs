using map_tile_server.Models.Details;

namespace map_tile_server.Services
{
    public interface IEmailService
    {
        Task SendEmailForgetPassword(MailRequest mailRequest);
    }
}
