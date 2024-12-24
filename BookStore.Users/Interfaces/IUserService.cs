using BookStore.Users.Models.DTOs;
using BookStore.Users.Models;

namespace BookStore.Users.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<string>> RegisterUserAsync(RegisterUserDto userdto);
        Task<ApiResponse<LoginResponseDto>> LoginUserAsync(LoginUserDto userdto);
        Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<ApiResponse<string>> ResetPassword(string Token, string Password);
    }
}
