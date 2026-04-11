namespace Vulpes.Zinc.Api.Services;

public interface IJwtTokenService
{
    string GenerateToken(string userId, string username);
    bool ValidateToken(string token);
}
