using BookStore.Users.Controllers;
using BookStore.Users.Interfaces;
using BookStore.Users.Models.DTOs;
using BookStore.Users.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookStore.Users.Tests
{
    public class UserControllerTests
    {
        private Mock<IUserService> _mockUserService;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockUserService.Object);
        }

        [Test]
        public async Task RegisterUser_ReturnsOkResult_WhenUserIsRegisteredSuccessfully()
        {
            var userDto = new RegisterUserDto
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Phone = "1234567890",
                Password = "Password123!"
            };

            var apiResponse = new ApiResponse<string>
            {
                Success = true,
                Message = "Registration successful"
            };

            _mockUserService.Setup(service => service.RegisterUserAsync(It.IsAny<RegisterUserDto>())).ReturnsAsync(apiResponse);

            var result = await _controller.RegisterUser(userDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(apiResponse, okResult.Value);
        }

        [Test]
        public async Task RegisterUser_ReturnsBadRequest_WhenUserRegistrationFails()
        {
            var userDto = new RegisterUserDto
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Phone = "1234567890",
                Password = "Password123!"
            };

            var apiResponse = new ApiResponse<string>
            {
                Success = false,
                Message = "Email already exists"
            };

            _mockUserService.Setup(service => service.RegisterUserAsync(It.IsAny<RegisterUserDto>())).ReturnsAsync(apiResponse);

            var result = await _controller.RegisterUser(userDto);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual(apiResponse, badRequestResult.Value);
        }

        [Test]
        public async Task LoginUser_ReturnsOkResult_WhenUserLogsInSuccessfully()
        {
            var userDto = new LoginUserDto
            {
                Email = "johndoe@example.com",
                Password = "Password123!"
            };

            var loginResponseDto = new LoginResponseDto
            {
                Token = "valid-jwt-token",
            };

            var apiResponse = new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Message = "Login successful",
                Data = loginResponseDto
            };

            _mockUserService.Setup(service => service.LoginUserAsync(It.IsAny<LoginUserDto>()))
                .ReturnsAsync(apiResponse);

            var result = await _controller.LoginUser(userDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(apiResponse, okResult.Value);
        }

        [Test]
        public async Task LoginUser_ReturnsBadRequest_WhenLoginFails()
        {
            var userDto = new LoginUserDto
            {
                Email = "johndoe@example.com",
                Password = "Password123!"
            };

            var apiResponse = new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Invalid credentials"
            };

            _mockUserService.Setup(service => service.LoginUserAsync(It.IsAny<LoginUserDto>()))
                .ReturnsAsync(apiResponse);

            var result = await _controller.LoginUser(userDto);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual(apiResponse, badRequestResult.Value);
        }

        [Test]
        public async Task ForgotPassword_ReturnsOkResult_WhenPasswordResetLinkSentSuccessfully()
        {
            var forgotPasswordDto = new ForgotPasswordDto
            {
                Email = "johndoe@example.com"
            };

            var apiResponse = new ApiResponse<string>
            {
                Success = true,
                Message = "Password reset link sent successfully"
            };

            _mockUserService.Setup(service => service.ForgotPasswordAsync(It.IsAny<ForgotPasswordDto>())).ReturnsAsync(apiResponse);

            var result = await _controller.ForgotPassword(forgotPasswordDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(apiResponse, okResult.Value);
        }

        [Test]
        public async Task ForgotPassword_ReturnsBadRequest_WhenPasswordResetLinkSendingFails()
        {
            var forgotPasswordDto = new ForgotPasswordDto
            {
                Email = "johndoe@example.com"
            };

            var apiResponse = new ApiResponse<string>
            {
                Success = false,
                Message = "Email not found"
            };

            _mockUserService.Setup(service => service.ForgotPasswordAsync(It.IsAny<ForgotPasswordDto>())).ReturnsAsync(apiResponse);

            var result = await _controller.ForgotPassword(forgotPasswordDto);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual(apiResponse, badRequestResult.Value);
        }

        [Test]
        public async Task ResetPassword_ReturnsOkResult_WhenPasswordIsResetSuccessfully()
        {
            var resetPasswordDto = new ResetPasswordDto
            {
                Token = "valid-token",
                Password = "NewPassword123!"
            };

            var apiResponse = new ApiResponse<string>
            {
                Success = true,
                Message = "Password reset successfully"
            };

            _mockUserService.Setup(service => service.ResetPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(apiResponse);

            var result = await _controller.ResetPassword(resetPasswordDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(apiResponse, okResult.Value);
        }

        [Test]
        public async Task ResetPassword_ReturnsBadRequest_WhenPasswordResetFails()
        {
            var resetPasswordDto = new ResetPasswordDto
            {
                Token = "invalid-token",
                Password = "NewPassword123!"
            };

            var apiResponse = new ApiResponse<string>
            {
                Success = false,
                Message = "Invalid token"
            };

            _mockUserService.Setup(service => service.ResetPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(apiResponse);

            var result = await _controller.ResetPassword(resetPasswordDto);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual(apiResponse, badRequestResult.Value);
        }

    }
}
