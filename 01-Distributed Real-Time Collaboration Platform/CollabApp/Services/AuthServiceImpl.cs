using CollabApp.Server.Protos;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CollabApp.Server.Services
{
    public class AuthServiceImpl : AuthService.AuthServiceBase
    {
        private static readonly Dictionary<string, string> _users = new Dictionary<string, string>();

        private readonly IConfiguration _configuration;

        public AuthServiceImpl(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override Task<AuthReply> Register(RegisterRequest request, ServerCallContext context)
        {
            if (_users.ContainsKey(request.Username))
                return Task.FromResult(new AuthReply { Message = "User already exists" });

            _users[request.Username] = request.Username;
            
            return Task.FromResult(new AuthReply { Message = "User registred" });
        }

        public override Task<AuthReply> Login(LoginRequest request, ServerCallContext context)
        {
            if (!_users.TryGetValue(request.Username, out var storedPassword) || storedPassword != request.Password)
                return Task.FromResult(new AuthReply { Message = "Invalid credentials" });

            var token = GenerateJwtToken(request.Username);
            
            return Task.FromResult(new AuthReply
            {
                Token = token,
                Message = "Successfully logged in"
            });
        }

        private string GenerateJwtToken(string username)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["JwtAudience"],
                claims: new[] { new Claim(ClaimTypes.Name, username)},
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}