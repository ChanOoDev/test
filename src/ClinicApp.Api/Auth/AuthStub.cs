using System.Security.Claims;

namespace ClinicApp.Api.Auth;

/// <summary>
/// Local auth stub (R4). Reads identity from request headers used for local
/// development only; swap for a real Identity/Cognito provider later.
///
/// Expected headers:
///   X-Staff-UserId : guid of an active staff Person
///   X-Staff-Role   : admin | receptionist | doctor
/// </summary>
public static class AuthStubDefaults
{
    public const string UserIdHeader = "X-Staff-UserId";
    public const string RoleHeader = "X-Staff-Role";
}

public static class ClinicRoles
{
    public const string Admin = "admin";
    public const string Receptionist = "receptionist";
    public const string Doctor = "doctor";

    /// <summary>Roles allowed to manage (create) appointments.</summary>
    public static readonly string[] AppointmentManagers = [Admin, Receptionist];
}

public static class AuthStub
{
    /// <summary>Build claims principal from local-dev headers, if present.</summary>
    public static ClaimsPrincipal? ResolvePrincipal(IHeaderDictionary headers)
    {
        if (!headers.TryGetValue(AuthStubDefaults.UserIdHeader, out var idValue)
            || string.IsNullOrWhiteSpace(idValue)
            || !Guid.TryParse(idValue.ToString(), out var userId))
        {
            return null;
        }

        var role = headers.TryGetValue(AuthStubDefaults.RoleHeader, out var roleValue)
            ? roleValue.ToString().ToLowerInvariant()
            : string.Empty;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Role, role),
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "StubAuth"));
    }
}