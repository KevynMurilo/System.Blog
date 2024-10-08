using System.Blog.Core.Contracts.Services;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace System.Blog.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendVerificationEmailAsync(string email, string verificationCode)
    {
        var smtpEmail = _configuration["EmailSettings:SmtpEmail"];
        var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

        var mail = new MailMessage
        {
            From = new MailAddress(smtpEmail, "Kevyn.Dev"),
            Subject = "Confirme seu endereço de e-mail",
            Body = EmailTemplates.GenerateVerificationEmailBody(verificationCode), 
            IsBodyHtml = true 
        };

        mail.To.Add(email);

        using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
        {
            smtpClient.Credentials = new System.Net.NetworkCredential(smtpEmail, smtpPassword);
            smtpClient.EnableSsl = true;
            await smtpClient.SendMailAsync(mail);
        }
    }
}
