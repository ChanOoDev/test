using System.Security.Claims;
using ClinicApp.Application.Abstractions;

namespace ClinicApp.Api.Auth;

/// <summary>Reads the authenticated user id from the request principal (auth stub).</summary>
public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid UserId
    {
        get
        {
            var value = accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : default;
        }
    }
}
