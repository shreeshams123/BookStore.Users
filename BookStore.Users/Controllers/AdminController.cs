using BookStore.Users.Interfaces;
using BookStore.Users.Models.DTOs;
using BookStore.Users.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Users.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController:ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        [HttpPost]

        public async Task<IActionResult> RegisterAdmin(RegisterAdminDto adminDto)
        {
            var apiResponse = await _adminService.RegisterAdminAsync(adminDto);
            if (apiResponse.Success)
            {
                return Ok(apiResponse);

            }
            return BadRequest(apiResponse);

        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAdmin(LoginUserDto adminDto)
        {
            var apiresponse = await _adminService.LoginAdminAsync(adminDto);
            if (apiresponse.Success)
            {
                return Ok(apiresponse);
            }
            return BadRequest(apiresponse);

        }
    }
}
