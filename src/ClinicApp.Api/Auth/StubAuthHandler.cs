using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ClinicApp.Api.Auth;

/// <summary>
/// No-op authentication handler for the local stub (R4). The AuthStubMiddleware
/// already sets HttpContext.User from stub headers before [Authorize] runs; this
/// handler only exists so unauthenticated requests are challenged with 401
/// instead of failing the pipeline. Replace with real auth in production.
/// </summary>
public sealed class StubAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // User was set by middleware; represent it as the auth result.
        if (Context.User.Identity?.IsAuthenticated == true)
        {
            var ticket = new AuthenticationTicket(Context.User, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        return Task.FromResult(AuthenticateResult.NoResult());
    }
}