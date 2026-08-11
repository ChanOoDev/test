using ClinicApp.Domain.Entities;

namespace ClinicApp.Application.Abstractions;

/// <summary>
/// Repository for appointments. Implementations must enforce the
/// double-booking guard transactionally (R1) so concurrent creates
/// cannot overlap.
/// </summary>
public interface IAppointmentRepository
{
    /// <summary>
    /// Persist the appointment atomically. Throws
    /// <see cref="AppointmentConflictException"/> when the slot overlaps an
    /// existing appointment for the same doctor.
    /// </summary>
    Task AddAsync(Appointment appointment, CancellationToken ct = default);

    Task<bool> PatientExistsAsync(Guid patientId, CancellationToken ct = default);
    Task<bool> DoctorExistsAsync(Guid doctorId, CancellationToken ct = default);
}