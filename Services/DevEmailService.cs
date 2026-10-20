using Microsoft.Extensions.Logging;

namespace tutorias.Services
{
    public class DevEmailService : IEmailSenderService
    {
        private readonly ILogger<DevEmailService> _logger;

        public DevEmailService(ILogger<DevEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            _logger.LogInformation("================== [DEV EMAIL SIMULATOR] ==================");
            _logger.LogInformation("Para: {ToEmail}", toEmail);
            _logger.LogInformation("Asunto: {Subject}", subject);
            _logger.LogInformation("Contenido: {HtmlContent}", htmlContent);
            _logger.LogInformation("===========================================================");
            return Task.CompletedTask;
        }

        public Task SendConfirmationEmailAsync(string toEmail, string confirmationLink, string nombreUsuario)
        {
            _logger.LogWarning("[DEV CONFIRMACION DE CORREO] Para: {ToEmail} ({Nombre})", toEmail, nombreUsuario);
            _logger.LogWarning("[DEV ENLACE DE CONFIRMACION]: {Link}", confirmationLink);

            var html = $@"
                <h2>Confirmación de Cuenta Institucional - Sistema de Tutorías IPN</h2>
                <p>Hola <strong>{nombreUsuario}</strong>,</p>
                <p>Gracias por registrarte en el Sistema de Tutorías Académicas de UPIICSA.</p>
                <p>Para activar tu cuenta, por favor confirma tu correo electrónico en el siguiente enlace:</p>
                <p><a href='{confirmationLink}'>Confirmar mi correo institucional</a></p>
                <p>Si no solicitaste este registro, por favor ignora este mensaje.</p>";

            return SendEmailAsync(toEmail, "Confirmación de Cuenta - Tutorías IPN", html);
        }

        public Task SendPasswordResetEmailAsync(string toEmail, string resetLink, string nombreUsuario)
        {
            _logger.LogWarning("[DEV RECUPERACION DE CONTRASENA] Para: {ToEmail} ({Nombre})", toEmail, nombreUsuario);
            _logger.LogWarning("[DEV ENLACE DE RESTABLECIMIENTO]: {Link}", resetLink);

            var html = $@"
                <h2>Recuperación de Contraseña - Sistema de Tutorías IPN</h2>
                <p>Hola <strong>{nombreUsuario}</strong>,</p>
                <p>Hemos recibido una solicitud para restablecer tu contraseña en el Sistema de Tutorías UPIICSA.</p>
                <p>Para crear una nueva contraseña, haz clic en el siguiente enlace:</p>
                <p><a href='{resetLink}'>Restablecer mi contraseña</a></p>
                <p>Si no solicitaste este cambio, puedes ignorar este mensaje de forma segura.</p>";

            return SendEmailAsync(toEmail, "Recuperación de Contraseña - Tutorías IPN", html);
        }
    }
}
