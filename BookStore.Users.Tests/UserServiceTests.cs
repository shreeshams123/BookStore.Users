using System;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Users.Interfaces;
using BookStore.Users.Models.DTOs;
using BookStore.Users.Models.Entities;
using BookStore.Users.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace BookStore.Users.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepo> _mockUserRepo;
        private Mock<IRabbitMqService> _mockRabbitMqService;
        private Mock<IEmailService> _mockEmailService;
        private TokenHelper _tokenHelper;
        private Mock<TokenHelper> _mockTokenHelper;
        private UserService _userService;

        [SetUp]
        public void SetUp()
        {
            _mockUserRepo = new Mock<IUserRepo>();
            _mockRabbitMqService = new Mock<IRabbitMqService>();
            _mockEmailService = new Mock<IEmailService>();
            var mockConfiguration = new Mock<IConfiguration>();
            _mockTokenHelper = new Mock<TokenHelper>(_mockUserRepo.Object, mockConfiguration.Object);
            _userService = new UserService(_mockUserRepo.Object, _mockRabbitMqService.Object, _mockTokenHelper.Object, _mockEmailService.Object);
        }

        [Test]
        public async Task RegisterUserAsync_ReturnsApiResponse_WhenUserIsRegisteredSuccessfully()
        {
            var userDto = new RegisterUserDto { Email = "johndoe@example.com", Password = "Password@123", Name = "John Doe", Phone = "1234567890" };

            _mockUserRepo.Setup(x => x.GetUserByEmailAsync(userDto.Email)).ReturnsAsync((User)null);
            _mockUserRepo.Setup(x => x.RegisterUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _mockRabbitMqService.Setup(x => x.SendMessage(It.IsAny<string>())).Verifiable();
            _mockEmailService.Setup(x => x.SendMailAsync(It.IsAny<EmailMessageDto>())).Returns(Task.CompletedTask);

            var result = await _userService.RegisterUserAsync(userDto);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Registration successful", result.Message);
            _mockRabbitMqService.Verify();
            _mockEmailService.Verify();
        }

        [Test]
        public async Task RegisterUserAsync_ReturnsApiResponse_WhenEmailAlreadyExists()
        {
            var userDto = new RegisterUserDto { Email = "johndoe@example.com", Password = "Password@123", Name = "John Doe", Phone = "1234567890" };

            _mockUserRepo.Setup(x => x.GetUserByEmailAsync(userDto.Email)).ReturnsAsync(new User());

            var result = await _userService.RegisterUserAsync(userDto);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Email already exists", result.Message);
        }

        [Test]
        public async Task LoginUserAsync_ReturnsApiResponse_WhenLoginIsSuccessful()
        {
            var userDto = new LoginUserDto { Email = "johndoe@example.com", Password = "Password@123" };
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password@123");
            var user = new User
            {
                Email = "johndoe@example.com",
                Password = hashedPassword,
                Role = "User",
                UserId = 1
            };

            _mockUserRepo.Setup(x => x.GetUserByEmailAsync(userDto.Email)).ReturnsAsync(user);
            _mockTokenHelper.Setup(x => x.GenerateJwtToken(user.Email, user.UserId, user.Role)).Returns("mockToken");

            var result = await _userService.LoginUserAsync(userDto);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Login Successful", result.Message);
        }

        [Test]
        public async Task LoginUserAsync_ReturnsApiResponse_WhenPasswordIsInvalid()
        {
            var userDto = new LoginUserDto { Email = "johndoe@example.com", Password = "WrongPassword" };
            var validHashedPassword = BCrypt.Net.BCrypt.HashPassword("Password@123");

            var user = new User { Email = "johndoe@example.com", Password = validHashedPassword, Role = "User", UserId = 1 };

            _mockUserRepo.Setup(x => x.GetUserByEmailAsync(userDto.Email)).ReturnsAsync(user);

            var result = await _userService.LoginUserAsync(userDto);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Invalid Password", result.Message);
        }

        [Test]
        public async Task ForgotPasswordAsync_ReturnsApiResponse_WhenEmailExists()
        {
            var forgotPasswordDto = new ForgotPasswordDto { Email = "johndoe@example.com" };
            var user = new User { Email = "johndoe@example.com" };

            _mockUserRepo.Setup(x => x.GetUserByEmailAsync(forgotPasswordDto.Email)).ReturnsAsync(user);
            _mockTokenHelper.Setup(x => x.GeneratePasswordResetToken(user)).Returns("mockResetToken");

            var result = await _userService.ForgotPasswordAsync(forgotPasswordDto);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Reset link sent to mail", result.Message);
        }

        [Test]
        public async Task ForgotPasswordAsync_ReturnsApiResponse_WhenEmailNotFound()
        {
            var forgotPasswordDto = new ForgotPasswordDto { Email = "nonexistent@example.com" };

            _mockUserRepo.Setup(x => x.GetUserByEmailAsync(forgotPasswordDto.Email)).ReturnsAsync((User)null);

            var result = await _userService.ForgotPasswordAsync(forgotPasswordDto);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Email not found", result.Message);
        }

        [Test]
        public async Task ResetPassword_ReturnsApiResponse_WhenPasswordIsResetSuccessfully()
        {
            var resetPasswordDto = new ResetPasswordDto { Token = "validToken", Password = "NewPassword@123" };
            var user = new User { Email = "johndoe@example.com", Password = "oldHashedPassword" };

            _mockTokenHelper.Setup(x => x.GetUserIdFromPasswordResetToken(resetPasswordDto.Token)).Returns(1);
            _mockUserRepo.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user);
            _mockUserRepo.Setup(x => x.UpdateUserAsync(user)).Returns(Task.CompletedTask);

            var result = await _userService.ResetPassword(resetPasswordDto.Token, resetPasswordDto.Password);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Password reset successful", result.Message);
        }

        [Test]
        public async Task ResetPassword_ReturnsApiResponse_WhenTokenIsInvalid()
        {
            var resetPasswordDto = new ResetPasswordDto { Token = "invalidToken", Password = "NewPassword@123" };

            _mockTokenHelper.Setup(x => x.GetUserIdFromPasswordResetToken(resetPasswordDto.Token)).Returns(-1);

            var result = await _userService.ResetPassword(resetPasswordDto.Token, resetPasswordDto.Password);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("User not found", result.Message);
        }

        [Test]
        public async Task ResetPassword_ReturnsApiResponse_WhenPasswordDoesNotMeetCriteria()
        {
            var resetPasswordDto = new ResetPasswordDto { Token = "validToken", Password = "short" };

            var result = await _userService.ResetPassword(resetPasswordDto.Token, resetPasswordDto.Password);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("User not found", result.Message);
        }
    }
}
