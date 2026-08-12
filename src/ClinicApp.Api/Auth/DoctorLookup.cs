using ClinicApp.Application.Abstractions;
using ClinicApp.Domain.Entities;
using ClinicApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Api.Auth;

/// <summary>Resolves the authenticated user's doctor person id from People.</summary>
public sealed class DoctorLookup(ClinicDbContext db) : IDoctorLookup
{
    public async Task<Guid?> GetDoctorIdForUserAsync(Guid userId, CancellationToken ct = default)
        => await db.People
            .Where(p => p.StaffUserId == userId && p.Role == PersonRole.Doctor && p.IsActive)
            .Select(p => (Guid?)p.Id)
            .FirstOrDefaultAsync(ct);
}
