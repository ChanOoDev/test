namespace ClinicApp.Domain.Entities;

/// <summary>Requested appointment slot, in UTC.</summary>
public sealed record AppointmentRequest(
    Guid PatientId,
    Guid DoctorId,
    DateTimeOffset StartUtc,
    int SlotMinutes,
    string Reason);