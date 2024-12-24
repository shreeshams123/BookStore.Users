using BookStore.Users.Models.DTOs;

namespace BookStore.Users.Interfaces
{
    public interface IEmailService
    {
        Task SendMailAsync(EmailMessageDto emailMessageDto);
    }
}
