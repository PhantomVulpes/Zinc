using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Vulpes.Zinc.Web.Models;

[Authorize]
public abstract class SecuredZincPageModel : ZincPageModel
{
    public Guid GetZincUserKey() => Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
}
