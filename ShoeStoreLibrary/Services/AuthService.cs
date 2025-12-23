using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShoeStoreLibrary.Contexts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStoreLibrary.Services
{
    public class AuthService
    {
        private readonly ShoeStoreContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ShoeStoreContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string Authenticate(string login, string password)
        {
            var user = _context.Users
                .Include(u => u.UserRoles) // роли пользователя
                .ThenInclude(ur => ur.Role) // роли через UserRoles
                .FirstOrDefault(u => u.Login == login && u.Password == password);


            if (user == null) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login)
            };

            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName)); // добавление ролей
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}