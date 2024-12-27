using System.ComponentModel.DataAnnotations;

namespace BookStore.Users.Models.DTOs
{
    public class RegisterAdminDto
    {
            [Required]
            public string Name { get; set; }
            [Required]
            [EmailAddress]

            public string Email { get; set; }
            [Required]
            [Phone]
            public string Phone { get; set; }
            [Required]
            public string Password { get; set; }
            [Required]
            [Compare("Password")]
            public string ConfirmPassword { get; set; }
        }
    }

