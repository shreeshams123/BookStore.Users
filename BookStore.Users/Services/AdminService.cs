using BookStore.Users.Interfaces;
using BookStore.Users.Models.DTOs;
using BookStore.Users.Models.Entities;
using BookStore.Users.Models;
using System.Text.Json;
using System.Text.RegularExpressions;
using BookStore.Users.Repositories;

namespace BookStore.Users.Services
{
    public class AdminService: IAdminService
    {
        private readonly IAdminRepo _adminRepo;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly TokenHelper _tokenHelper;
        public AdminService(IAdminRepo adminRepo, IRabbitMqService rabbitMqService, TokenHelper tokenHelper)
        {
            _adminRepo = adminRepo;
            _rabbitMqService = rabbitMqService;
            _tokenHelper = tokenHelper;
        }
        public async Task<ApiResponse<string>> RegisterAdminAsync(RegisterAdminDto admindto)
        {

            var adminpresent = await _adminRepo.GetAdminByEmailAsync(admindto.Email);
            if (adminpresent != null)
            {

                return new ApiResponse<string> { Success = false, Message = "Email already exists", Data = null };
            }

            var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            if (!Regex.IsMatch(admindto.Password, pattern))
            {

                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Password should contain minimum 8 characters (at least one special character, one number, one lowercase and one uppercase letter)",
                    Data = null
                };
            }

            string hashedpassword = PasswordHelper.GenerateHashedPassword(admindto.Password);

            if (string.IsNullOrEmpty(hashedpassword))
            {
                return new ApiResponse<string> { Success = false, Message = "Failed to hash password", Data = null };
            }

            var newAdmin = new Admin
            {
                Name = admindto.Name,
                Email = admindto.Email,
                Phone = admindto.Phone,
                Password = hashedpassword
            };

            if (newAdmin == null)
            {
                return new ApiResponse<string> { Success = false, Message = "User registration failed", Data = null };
            }

            await _adminRepo.RegisterUserAsync(newAdmin);

            if (_rabbitMqService == null)
            {
                return new ApiResponse<string> { Success = false, Message = "Failed to send email notification", Data = null };
            }

            var emailMessage = new EmailMessageDto
            {
                Email = admindto.Email,
                Subject = "Welcome to BookStore App",
                Body = $"<p>Hello {admindto.Name},</p><p>Welcome to BookStore App!</p>"
            };

            var messageJson = JsonSerializer.Serialize(emailMessage);


            _rabbitMqService.SendMessage(messageJson);

            return new ApiResponse<string> { Success = true, Message = "Registration successful", Data = null };
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAdminAsync(LoginUserDto adminDto)
        {
            var result = await _adminRepo.GetAdminByEmailAsync(adminDto.Email);

            if (result == null)
            {
                return new ApiResponse<LoginResponseDto> { Success = false, Message = "User not found", Data = null };
            }
            bool isValidPassword = PasswordHelper.VerifyPassword(adminDto.Password, result.Password);
            if (isValidPassword)
            {
                var token = _tokenHelper.GenerateJwtToken(result.Email,result.Id,result.Role);
                var newdto = new LoginResponseDto { Name = result.Name, Email = adminDto.Email, Token = token };
                return new ApiResponse<LoginResponseDto> { Success = true, Message = "Login Successful", Data = newdto };
            }
            else
            {
                return new ApiResponse<LoginResponseDto> { Success = false, Message = "Invalid Password", Data = null };
            }
        }
    }
}
