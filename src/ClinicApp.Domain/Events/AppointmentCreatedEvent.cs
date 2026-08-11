namespace ClinicApp.Domain.Events;

public sealed record AppointmentCreatedEvent(Guid AppointmentId, Guid DoctorId, DateTimeOffset StartUtc);