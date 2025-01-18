using BookStore.Users.Interfaces;
using BookStore.Users.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStore.Users.Services
{
    public class TokenHelper: ITokenHelper
    {
        private readonly IUserRepo _userRepo;
        private readonly IConfiguration _configuration;
        public TokenHelper(IUserRepo userRepo,IConfiguration configuration)
        {
            _userRepo = userRepo;
            _configuration = configuration;
        }
        public virtual string GenerateJwtToken(string email, int id, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Role, role) 
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public virtual string GeneratePasswordResetToken(User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                throw new ArgumentNullException(nameof(user), "User or email cannot be null");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:PasswordSecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
            new Claim(ClaimTypes.Email, user.Email),

             new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public virtual int GetUserIdFromPasswordResetToken(string token)
        {

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Token is missing");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:PasswordSecretKey"])),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                var userIdClaim = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    throw new UnauthorizedAccessException("UserId not found or invalid in the token");
                }
                return userId;
            }
            catch (SecurityTokenExpiredException)
            {
                throw new UnauthorizedAccessException("Token has expired.");
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Invalid token: " + ex.Message);
            }
        }

    }
}
