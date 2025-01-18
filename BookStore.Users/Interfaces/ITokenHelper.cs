using BookStore.Users.Models.Entities;

namespace BookStore.Users.Interfaces
{
    public interface ITokenHelper
    {
        string GenerateJwtToken(string email, int id, string role);
        string GeneratePasswordResetToken(User user);
        int GetUserIdFromPasswordResetToken(string token);
    }

}
