namespace FirstASP.Infrastructure;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string email, string verificationCode);
}

public class MockEmailService(ILogger<MockEmailService> logger) : IEmailService
{
    public Task SendVerificationEmailAsync(string email, string code)
    {
        logger.LogInformation("Mock email to {Email}: Verification code - {Code}", email, code);
        return Task.CompletedTask;
    }
}