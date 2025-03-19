using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserService.DAOs;
using UserService.DTOs;

namespace UserService.Config
{
    public class AuthenConfig
    {
        private readonly MicroserviceUserDbContext _context;
        private readonly IConfiguration _config;

        public AuthenConfig(MicroserviceUserDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }


        public string GenerateToken(LoginDTO request)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                new[] { new Claim(ClaimTypes.Name, request.Identifier) },
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
