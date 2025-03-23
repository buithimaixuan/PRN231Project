using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserService.DAOs;
using UserService.DTOs;
using UserService.PasswordHashing;
using UserService.Services.Implement;
using UserService.Services.Interface;

namespace UserService.Config
{
    public class AuthenConfig
    {
        private readonly MicroserviceUserDbContext _context;
        private readonly IConfiguration _config;
        private readonly IAccountService _service;
        private readonly PasswordHasher _passwordHasher;

        public AuthenConfig(MicroserviceUserDbContext context, IConfiguration config, IAccountService service, PasswordHasher passwordHasher)
        {
            _context = context;
            _config = config;
            _service = service;
            _passwordHasher = passwordHasher;
        }

        public string GenerateToken(LoginDTO request)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                new[] { new Claim(ClaimTypes.Name, request.Identifier) },
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<LoginResponseDTO?> Authenticate(LoginDTO request)
        {
            if (string.IsNullOrEmpty(request?.Identifier) || string.IsNullOrEmpty(request?.Password))
            {
                return null;
            }

            // Tìm tài khoản bằng identifier (Email, Username, hoặc PhoneNumber)
            var account = await _service.GetAccountByIdentifier(request.Identifier);
            if (account == null)
            {
                return null;
            }

            // Kiểm tra trạng thái tài khoản
            if (account.IsDeleted.HasValue && account.IsDeleted.Value)
            {
                return null;
            }

            // Xác minh password
            bool isPasswordValid = _passwordHasher.VerifyPassword(request.Password, account.Password);
            if (!isPasswordValid)
            {
                return null;
            }

            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = _config["Jwt:Key"];
            var tokenValidityMins = _config.GetValue<int>("Jwt:TokenValidityMins");
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, request.Identifier)
                }),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)), SecurityAlgorithms.HmacSha256),
            };

            var tokenHandler = new JwtSecurityTokenHandler();   
            var securityToken = tokenHandler.CreateToken(tokenDescription);
            var accessToken = tokenHandler.WriteToken(securityToken);

            return new LoginResponseDTO
            {
                Message = "Login successful.",
                AccountId = account.AccountId,
                Username = account.Username,
                Email = account.Email,
                Phone = account.Phone,
                RoleId = account.RoleId,
                Token = accessToken
            };

        }
    }
}
