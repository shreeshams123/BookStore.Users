using BookStore.Users.Models.Entities;

namespace BookStore.Users.Interfaces
{
    public interface IAdminRepo
    {
        Task RegisterUserAsync(Admin admin);
        Task<Admin> GetAdminByIdAsync(int adminId);
        Task<Admin> GetAdminByEmailAsync(string email);
    }
}
