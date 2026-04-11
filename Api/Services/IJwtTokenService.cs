using Vulpes.Zinc.Core.Models;

namespace Vulpes.Zinc.Api.Services;

public interface IJwtTokenService
{
    string GenerateToken(RegisteredUser user);
}
