using System.ComponentModel.DataAnnotations;

namespace BookStore.Users.Models.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
