using System.Security.Claims;
using coaches.Modules.Shared.Contracts.Common.Interfaces;

namespace coaches.Web.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor)
    : IUser
{
    public string? Id => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
