using BookStore.Users.Data;
using BookStore.Users.Models.Entities;
using BookStore.Users.Repositories;
using Microsoft.EntityFrameworkCore;

[TestFixture]
public class UserRepoTests : IDisposable
{
    private UserDbContext _dbContext;
    private UserRepo _userRepo;
    private DbContextOptions<UserDbContext> _dbContextOptions;

    [SetUp]
    public void SetUp()
    {
        _dbContextOptions = new DbContextOptionsBuilder<UserDbContext>()
                            .UseInMemoryDatabase(databaseName: "TestDb")
                            .Options;

        _dbContext = new UserDbContext(_dbContextOptions);
        _userRepo = new UserRepo(_dbContext);

        _dbContext.Users.Add(new User
        {
            UserId = 1,
            Email = "johndoe@example.com",
            Name = "John Doe",
            Password = "Shreesha@123",
            Phone = "8105581789"
        });
        _dbContext.SaveChanges();
    }

    [Test]
    public async Task RegisterUserAsync_ShouldAddUser_WhenEmailDoesNotExist()
    {
        var user = new User { UserId = 2, Email = "unique@example.com", Name = "Unique User", Password = "Password123", Phone = "1234567890" };

        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        if (existingUser == null)
        {
            await _userRepo.RegisterUserAsync(user);
        }

        var addedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        Assert.IsNotNull(addedUser);
        Assert.AreEqual(user.Email, addedUser.Email);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    public void Dispose()
    {
        TearDown();
    }
}
