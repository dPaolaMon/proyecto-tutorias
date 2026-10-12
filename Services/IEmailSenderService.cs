namespace tutorias.Services
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlContent);
        Task SendConfirmationEmailAsync(string toEmail, string confirmationLink, string nombreUsuario);
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink, string nombreUsuario);
    }
}
