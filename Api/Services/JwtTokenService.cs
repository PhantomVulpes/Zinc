using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vulpes.Zinc.Api.Configuration;
using Vulpes.Zinc.Core.Models;

namespace Vulpes.Zinc.Api.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtConfiguration jwtConfig;

    public JwtTokenService(IOptions<JwtConfiguration> jwtConfig)
    {
        this.jwtConfig = jwtConfig.Value;
    }

    public string GenerateToken(RegisteredUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Key.ToString()),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("UserId", user.Key.ToString()),
            new("UserStatus", user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtConfig.Issuer,
            audience: jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(jwtConfig.ExpiryInHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}