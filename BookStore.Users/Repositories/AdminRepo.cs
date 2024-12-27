using BookStore.Users.Data;
using BookStore.Users.Interfaces;
using BookStore.Users.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Users.Repositories
{
    public class AdminRepo: IAdminRepo
    {
        private readonly UserDbContext _context;
        public AdminRepo(UserDbContext context) { 
        _context = context;
        }
        public async Task RegisterUserAsync(Admin admin)
        {
            try
            {
                await _context.Admins.AddAsync(admin);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<Admin> GetAdminByIdAsync(int adminId)
        {
            try
            {
                var admin = await _context.Admins.FirstOrDefaultAsync(u => u.Id == adminId);

                return admin;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<Admin> GetAdminByEmailAsync(string email)
        {
            try
            {
                var admin = await _context.Admins.FirstOrDefaultAsync(u => u.Email == email);
                return admin;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
