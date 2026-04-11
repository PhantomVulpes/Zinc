namespace Vulpes.Zinc.Api.Configuration;

public interface IJwtTokenService
{
    string GenerateToken(string userId, string username);
    bool ValidateToken(string token);
}
