using BookStore.Users.Interfaces;
using BookStore.Users.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Users.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController:ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("register")]

        public async Task<IActionResult> RegisterUser(RegisterUserDto userdto)
        {
            var apiResponse = await _userService.RegisterUserAsync(userdto);
            if (apiResponse.Success)
            {
                return Ok(apiResponse);

            }
            return BadRequest(apiResponse);

        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginUserDto userdto)
        {
            var apiresponse = await _userService.LoginUserAsync(userdto);
            if (apiresponse.Success)
            {
                return Ok(apiresponse);
            }
            return BadRequest(apiresponse);

        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var apiresponse = await _userService.ForgotPasswordAsync(forgotPasswordDto);
            if (apiresponse.Success)
            {
                return Ok(apiresponse);
            }
            return BadRequest(apiresponse);
        }

        [HttpPost("reset-password")]

        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var apiresponse = await _userService.ResetPassword(resetPasswordDto.Token, resetPasswordDto.Password);
            if (apiresponse.Success)
            {
                return Ok(apiresponse);
            }
            return BadRequest(apiresponse);
        }

    }
}
