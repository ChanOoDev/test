namespace ClinicApp.Application.Abstractions;

/// <summary>
/// Resolves the authenticated staff member's doctor identity (if any).
/// Used to enforce read-scope: a doctor may only see their own appointments.
/// </summary>
public interface IDoctorLookup
{
    /// <summary>Doctor person id for the current staff user, or null if the user is not a doctor.</summary>
    Task<Guid?> GetDoctorIdForUserAsync(Guid userId, CancellationToken ct = default);
}
