using BookStore.Users.Interfaces;
using BookStore.Users.Models;
using BookStore.Users.Models.DTOs;
using BookStore.Users.Models.Entities;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BookStore.Users.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly TokenHelper _tokenHelper;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly IEmailService _emailService;
        public UserService(IUserRepo userRepo, IRabbitMqService rabbitMqService,TokenHelper tokenHelper,IEmailService emailService) { 
            _userRepo = userRepo;
            _rabbitMqService = rabbitMqService;
            _tokenHelper = tokenHelper;
            _emailService = emailService;
        }
        public async Task<ApiResponse<string>> RegisterUserAsync(RegisterUserDto userdto)
        {
          
            var userpresent = await _userRepo.GetUserByEmailAsync(userdto.Email);
            if (userpresent != null)
            {
                
                return new ApiResponse<string> { Success = false, Message = "Email already exists", Data = null };
            }

            var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            if (!Regex.IsMatch(userdto.Password, pattern))
            {

                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Password should contain minimum 8 characters (at least one special character, one number, one lowercase and one uppercase letter)",
                    Data = null
                };
            }

            string hashedpassword = PasswordHelper.GenerateHashedPassword(userdto.Password);

            if (string.IsNullOrEmpty(hashedpassword))
            {
                return new ApiResponse<string> { Success = false, Message = "Failed to hash password", Data = null };
            }

            var newUser = new User
            {
                Name = userdto.Name,
                Email = userdto.Email,
                Phone = userdto.Phone,
                Password = hashedpassword
            };

            if (newUser == null)
            {
                return new ApiResponse<string> { Success = false, Message = "User registration failed", Data = null };
            }

            await _userRepo.RegisterUserAsync(newUser);

            if (_rabbitMqService == null)
            {
                return new ApiResponse<string> { Success = false, Message = "Failed to send email notification", Data = null };
            }

            var emailMessage = new EmailMessageDto
            {
                Email = userdto.Email,
                Subject = "Welcome to BookStore App",
                Body = $"<p>Hello {userdto.Name},</p><p>Welcome to BookStore App!</p>"
            };

            var messageJson = JsonSerializer.Serialize(emailMessage);


            _rabbitMqService.SendMessage(messageJson);

            return new ApiResponse<string> { Success = true, Message = "Registration successful", Data = null };
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginUserAsync(LoginUserDto userdto)
        {
            var result = await _userRepo.GetUserByEmailAsync(userdto.Email);

            if (result == null)
            {
                return new ApiResponse<LoginResponseDto> { Success = false, Message = "User not found", Data = null };
            }
            bool isValidPassword = PasswordHelper.VerifyPassword(userdto.Password, result.Password);
            if (isValidPassword)
            {
                var token = _tokenHelper.GenerateJwtToken(result.Email,result.UserId,result.Role);
                var newdto = new LoginResponseDto { Name = result.Name, Email = userdto.Email, Token = token };
                return new ApiResponse<LoginResponseDto> { Success = true, Message = "Login Successful", Data = newdto };
            }
            else
            {
                return new ApiResponse<LoginResponseDto> { Success = false, Message = "Invalid Password", Data = null };
            }
        }

        public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userRepo.GetUserByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return new ApiResponse<string> { Success = false, Message = "Email not found", Data = null };
            }
            else
            {
                var resetToken = _tokenHelper.GeneratePasswordResetToken(user);
                var emailmessage = new EmailMessageDto
                {
                    Email = forgotPasswordDto.Email,
                    Subject = "Token for reset password",
                    Body = resetToken
                };
                await _emailService.SendMailAsync(emailmessage);
                return new ApiResponse<string> { Success = true, Message = "Reset link sent to mail", Data = null };
            }

        }

        public async Task<ApiResponse<string>> ResetPassword(string Token, string Password)
        {
            var userId = _tokenHelper.GetUserIdFromPasswordResetToken(Token);
            if (userId == null)
            {
                return new ApiResponse<string> { Success = false, Message = "Invalid token", Data = null };
            }
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponse<string> { Success = false, Message = "User not found", Data = null };
            }
            var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            if (!Regex.IsMatch(Password, pattern))
            {
                return new ApiResponse<string> { Success = false, Message = "Password should contain minimum 8 characters(atleast one special character,one number,one lowercase and one uppercase letter)", Data = null };
            }
            var hashpassword = PasswordHelper.GenerateHashedPassword(Password);
            user.Password = hashpassword;
            await _userRepo.UpdateUserAsync(user);
            return new ApiResponse<string> { Success = true, Message = "Password reset successful", Data = null };
        }
    }
}
