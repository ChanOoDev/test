using ClinicApp.Application.Abstractions;
using ClinicApp.Application.UseCases.Appointments.GetDayAppointments;
using ClinicApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Persistence.Repositories;

public class AppointmentRepository(ClinicDbContext db) : IAppointmentRepository
{
    public async Task AddAsync(Appointment appointment, CancellationToken ct = default)
    {
        // R1: conflict guard must be concurrency-safe. Serialize writes through
        // the same SQLite connection/file inside a transaction so two concurrent
        // creates cannot both inspect an empty set and both insert overlapping slots.
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var requestedStartTicks = appointment.StartUtcTicks;
        var requestedEndTicks = appointment.EndUtcTicks;

        var overlaps = await db.Appointments.AnyAsync(
            a => a.DoctorId == appointment.DoctorId
              && a.Status != AppointmentStatus.Cancelled
              && a.StartUtcTicks < requestedEndTicks
              && requestedStartTicks < a.EndUtcTicks,
            ct);

        if (overlaps)
        {
            await tx.RollbackAsync(ct);
            throw new AppointmentConflictException();
        }

        db.Appointments.Add(appointment);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    public async Task<bool> PatientExistsAsync(Guid patientId, CancellationToken ct = default)
        => await db.People.AnyAsync(
            p => p.Id == patientId && p.Role == PersonRole.Patient && p.IsActive, ct);

    public async Task<bool> DoctorExistsAsync(Guid doctorId, CancellationToken ct = default)
        => await db.People.AnyAsync(
            p => p.Id == doctorId && p.Role == PersonRole.Doctor && p.IsActive, ct);

    public async Task<List<DayAppointmentDto>> GetDayAsync(
        DateOnly day,
        Guid? doctorId,
        AppointmentStatus? status,
        CancellationToken ct = default)
    {
        // UTC day window in ticks — SQLite can't translate DateTimeOffset comparisons.
        var startTicks = new DateTimeOffset(day.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero).UtcTicks;
        var endTicks = new DateTimeOffset(day.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero).UtcTicks;

        var query = db.Appointments
            .Where(a => a.StartUtcTicks >= startTicks && a.StartUtcTicks < endTicks);

        if (doctorId is not null)
            query = query.Where(a => a.DoctorId == doctorId);

        if (status is not null)
            query = query.Where(a => a.Status == status);

        return await query
            .OrderBy(a => a.StartUtcTicks)
            .Select(a => new DayAppointmentDto(
                a.Id,
                a.PatientId,
                a.Patient.Name,
                a.DoctorId,
                a.Doctor.Name,
                a.StartUtc,
                a.SlotMinutes,
                a.Status,
                a.Reason))
            .ToListAsync(ct);
    }
}