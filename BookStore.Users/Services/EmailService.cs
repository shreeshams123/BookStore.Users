using MimeKit;
using MailKit.Net.Smtp;
using BookStore.Users.Interfaces;
using BookStore.Users.Models.DTOs;

namespace BookStore.Users.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendMailAsync(EmailMessageDto emailMessageDto)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("BookStore App", _configuration["Email:From"]));
            message.To.Add(new MailboxAddress("", emailMessageDto.Email));
            message.Subject = emailMessageDto.Subject;
            message.Body = new TextPart("html")
            {
                Text = $"<p>{emailMessageDto.Body}</p>"
            };
            using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                await smtpClient.ConnectAsync(_configuration["Email:SmtpServer"], int.Parse(_configuration["Email:SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_configuration["Email:Username"], _configuration["Email:Password"]);
                await smtpClient.SendAsync(message);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                await smtpClient.DisconnectAsync(true);
            }
        }
    }
}
