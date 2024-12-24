namespace BookStore.Users.Services
{
    public class PasswordHelper
    {
        public static string GenerateHashedPassword(string Password)
        {
            return BCrypt.Net.BCrypt.HashPassword(Password);
        }
        public static bool VerifyPassword(string Password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(Password, hashedPassword);
        }
    }
}
