namespace System.Blog.Core.Contracts.Services;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string email, string token, string name);
}
