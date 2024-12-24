using BookStore.Users.Data;
using BookStore.Users.Interfaces;
using BookStore.Users.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Users.Repositories
{
    public class UserRepo:IUserRepo
    {
        private readonly UserDbContext _context;
        public UserRepo(UserDbContext context)
        {
            _context = context;   
        }
        public async Task RegisterUserAsync(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId== userId);

                return user;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                return user;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

}
