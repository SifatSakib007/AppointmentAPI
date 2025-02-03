using AppointmentAPI.Data;
using AppointmentAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppointmentAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public string RegisterUser(string userName, string password)
        {
            if (_context.Users.Any(u => u.UserName == userName))
            {
                return "User already exists";
            }


            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                UserName = userName,
                PasswordHash = hashedPassword
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return "User registered successfully";
        }

        public string? AuthenticateUser(string userName, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            try
            {
                return GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔴 Token generation failed: {ex.Message}");
                return null;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"], // ✅ Ensure issuer is set
                audience: _config["JWT:Audience"], // ✅ Add this
                claims: new[] { new Claim(ClaimTypes.Name, user.UserName) },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
