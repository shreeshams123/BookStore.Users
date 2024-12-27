using BookStore.Users.Models.DTOs;
using BookStore.Users.Models;

namespace BookStore.Users.Interfaces
{
    public interface IAdminService
    {
        Task<ApiResponse<string>> RegisterAdminAsync(RegisterAdminDto admindto);
        Task<ApiResponse<LoginResponseDto>> LoginAdminAsync(LoginUserDto adminDto);
    }
}
