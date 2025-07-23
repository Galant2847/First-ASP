public interface IEmailService
{
    Task SendVerificationEmailAsync(string email, string verificationCode);
}

public class MockEmailService(ILogger<MockEmailService> logger) : IEmailService
{
    public Task SendVerificationEmailAsync(string email, string code)
    {
        logger.LogInformation($"Mock email to {email}: Verification code - {code}");
        return Task.CompletedTask;
    }
}