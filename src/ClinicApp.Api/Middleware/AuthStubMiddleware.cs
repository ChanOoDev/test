using ClinicApp.Api.Auth;

namespace ClinicApp.Api.Middleware;

/// <summary>
/// Local dev auth stub (R4). Sets HttpContext.User from stub headers so
/// [Authorize] / [Authorize(Roles=...)] enforce server-side. In production,
/// replace with real authentication; the policy checks stay unchanged.
/// </summary>
public sealed class AuthStubMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var principal = AuthStub.ResolvePrincipal(context.Request.Headers);
        if (principal is not null)
        {
            context.User = principal;
        }

        await next(context);
    }
}