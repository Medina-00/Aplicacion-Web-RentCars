



using Core.Application.Dtos.Email;

namespace Core.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);

    }
}
