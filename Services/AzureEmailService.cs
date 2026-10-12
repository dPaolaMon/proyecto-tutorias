using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace tutorias.Services
{
    public class AzureEmailService : IEmailSenderService
    {
        private readonly string? _connectionString;
        private readonly string? _senderAddress;
        private readonly ILogger<AzureEmailService> _logger;

        public AzureEmailService(IConfiguration configuration, ILogger<AzureEmailService> logger)
        {
            _connectionString = configuration["AZURE_COMMUNICATION_SERVICES_CONNECTION_STRING"]
                                ?? Environment.GetEnvironmentVariable("AZURE_COMMUNICATION_SERVICES_CONNECTION_STRING");
            _senderAddress = configuration["EMAIL_SENDER_ADDRESS"]
                             ?? Environment.GetEnvironmentVariable("EMAIL_SENDER_ADDRESS");
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(_connectionString) || string.IsNullOrWhiteSpace(_senderAddress))
            {
                _logger.LogWarning("⚠️ Azure Communication Services no está configurado (falta connection string o remitente). No se envió el correo a {ToEmail}.", toEmail);
                return;
            }

            try
            {
                var emailClient = new EmailClient(_connectionString);
                var emailMessage = new EmailMessage(
                    senderAddress: _senderAddress,
                    recipientAddress: toEmail,
                    content: new EmailContent(subject) { Html = htmlContent });

                var emailSendOperation = await emailClient.SendAsync(WaitUntil.Started, emailMessage);
                _logger.LogInformation("✅ Correo enviado vía Azure Communication Services a {ToEmail}. OperationId: {OpId}", toEmail, emailSendOperation.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al enviar correo vía Azure Communication Services a {ToEmail}", toEmail);
                throw;
            }
        }

        public async Task SendConfirmationEmailAsync(string toEmail, string confirmationLink, string nombreUsuario)
        {
            var html = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                    <h2 style='color: #750946;'>Sistema de Tutorías IPN - UPIICSA</h2>
                    <p>Hola <strong>{nombreUsuario}</strong>,</p>
                    <p>Gracias por registrarte en el Sistema de Tutorías Académicas.</p>
                    <p>Para activar tu cuenta institucional y verificar tu correo, por favor haz clic en el siguiente botón:</p>
                    <p style='text-align: center; margin: 30px 0;'>
                        <a href='{confirmationLink}' style='background-color: #750946; color: #ffffff; padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: bold; display: inline-block;'>Confirmar mi cuenta</a>
                    </p>
                    <p style='color: #666; font-size: 13px;'>Si no puedes hacer clic en el botón, copia y pega este enlace en tu navegador:</p>
                    <p style='word-break: break-all; font-size: 12px; color: #750946;'>{confirmationLink}</p>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;' />
                    <p style='color: #999; font-size: 11px;'>Si no solicitaste este registro, por favor ignora este mensaje.</p>
                </div>";

            await SendEmailAsync(toEmail, "Confirmación de Cuenta - Tutorías IPN", html);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink, string nombreUsuario)
        {
            var html = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                    <h2 style='color: #750946;'>Sistema de Tutorías IPN - UPIICSA</h2>
                    <p>Hola <strong>{nombreUsuario}</strong>,</p>
                    <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta institucional.</p>
                    <p>Para crear una nueva contraseña, por favor haz clic en el siguiente botón:</p>
                    <p style='text-align: center; margin: 30px 0;'>
                        <a href='{resetLink}' style='background-color: #750946; color: #ffffff; padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: bold; display: inline-block;'>Restablecer Contraseña</a>
                    </p>
                    <p style='color: #666; font-size: 13px;'>Si no puedes hacer clic en el botón, copia y pega este enlace en tu navegador:</p>
                    <p style='word-break: break-all; font-size: 12px; color: #750946;'>{resetLink}</p>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;' />
                    <p style='color: #999; font-size: 11px;'>Si no solicitaste restablecer tu contraseña, ignora este mensaje. Tu contraseña actual seguirá siendo válida.</p>
                </div>";

            await SendEmailAsync(toEmail, "Restablecer Contraseña - Tutorías IPN", html);
        }
    }
}
