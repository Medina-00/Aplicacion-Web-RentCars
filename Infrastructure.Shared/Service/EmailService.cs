
using Core.Application.Dtos.Email;
using Core.Application.Interfaces.Services;
using Core.Domain.Settings;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Infrastructure.Shared.Service
{
    public class EmailService : IEmailService
    {
        public MainSetting MainSetting { get; }
        public EmailService(IOptions<MainSetting> mainSetting)
        {
            MainSetting = mainSetting.Value;
        }


        public async Task SendAsync(EmailRequest request)
        {
            try
            {
                //PRIMERP INICIALIZAMOS MIME PARA LA ESTRUCTURA DEL EMAIL
                MimeMessage message = new();


                //AQUI CONFIGURAMOS EL ENCABEZADO DEL EMAIL
                message.Sender = MailboxAddress.Parse(MainSetting.DisplayName + " <" + MainSetting.EmailFrom + "> ");

                //LUEGO INDICAMOS A QUIEN ENVIARMOS EL CORREO
                message.To.Add(MailboxAddress.Parse(request.To));

                //AQUI INDICAMOS EL SUJETO DEL EMAIL
                message.Subject = request.Subject;

                //ESTO QUE HACEMOS AQUI ES CREANDO EL CUERPO DEL EMAIL
                BodyBuilder builder = new();

                //ESTO AQUI ES OPCIONAL , ES PARA PODER AGREGAR ETIQUETAS HTML EN EMAIL
                builder.HtmlBody = request.Body;
                message.Body = builder.ToMessageBody();

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Connect(MainSetting.SmtHost, MainSetting.SmtPort, SecureSocketOptions.StartTls);
                    smtpClient.Authenticate(MainSetting.SmtUser, MainSetting.SmtPass);
                    await smtpClient.SendAsync(message);
                    smtpClient.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


        }

    }
}
