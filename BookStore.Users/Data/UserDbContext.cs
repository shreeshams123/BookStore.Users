using BookStore.Users.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BookStore.Users.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
    }
}
