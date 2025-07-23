using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

public class EmailService(IConfiguration config, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendVerificationEmailAsync(string email, string verificationCode)
    {
        var settings = config.GetSection("EmailSettings");
        
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            settings["FromName"], 
            settings["FromAddress"]));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = "Подтверждение email";

        // HTML-письмо с кодом
        message.Body = new TextPart(TextFormat.Html)
        {
            Text = $"""
                    <h1>Ваш код подтверждения</h1>
                    <p>Используйте этот код для завершения регистрации:</p>
                    <h2 style="color: #0066ff;">{verificationCode}</h2>
                    <p><em>Никому не сообщайте этот код!</em></p>
                    """
        };

        using var client = new SmtpClient();
        
        try
        {
            await client.ConnectAsync(
                settings["SmtpServer"], 
                int.Parse(settings["SmtpPort"]!), 
                SecureSocketOptions.StartTls);
            
            await client.AuthenticateAsync(
                settings["Username"], 
                settings["Password"]);
            
            await client.SendAsync(message);
            logger.LogInformation($"Verification email sent to {email}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email");
            throw new Exception("Failed to send verification email");
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}