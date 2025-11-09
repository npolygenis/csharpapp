using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CSharpApp.Infrastructure.Security;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IConfiguration configuration, ILogger<AuthService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> Login(UserCredentials userCredentials)
    {
        var restApiSettings = _configuration.GetSection(nameof(RestApiSettings)).Get<RestApiSettings>();
        var jwtSettings = _configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();

        _logger.LogInformation("RestApiSettings: {@RestApiSettings}", restApiSettings);
        _logger.LogInformation("JwtSettings: {@JwtSettings}", jwtSettings);

        if (userCredentials.Username == restApiSettings.Username && userCredentials.Password == restApiSettings.Password)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", userCredentials.Username) }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        return null;
    }
}