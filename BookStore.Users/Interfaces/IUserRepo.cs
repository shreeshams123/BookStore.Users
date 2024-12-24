using BookStore.Users.Models.Entities;

namespace BookStore.Users.Interfaces
{
    public interface IUserRepo
    {
        Task RegisterUserAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(User user);
    }
}
