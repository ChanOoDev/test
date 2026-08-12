namespace ClinicApp.Application.Abstractions;

/// <summary>
/// Current authenticated staff member. Implemented in the Api layer from the
/// auth principal; consumed by handlers for authorization decisions.
/// </summary>
public interface ICurrentUser
{
    Guid UserId { get; }
}
