using Microsoft.AspNetCore.Authorization;

namespace Vulpes.Zinc.Web.Models;

[Authorize]
public abstract class SecuredZincPageModel : ZincPageModel
{ }
