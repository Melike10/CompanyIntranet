using CompanyIntranet.Services;

public class FakeEmailSender : IEmailSender
{
    public Task SendEmailAsync(string toEmail, string subject, string body)
    {
        Console.WriteLine($"[TEST] Mail gönderildi: {toEmail} - {subject}");
        return Task.CompletedTask;
    }
}