using ClinicApp.Domain.Entities;
using MediatR;

namespace ClinicApp.Application.UseCases.Appointments.CreateAppointment;

/// <summary>Command to create an appointment. Start must be UTC.</summary>
public sealed record CreateAppointmentCommand(
    Guid PatientId,
    Guid DoctorId,
    DateTimeOffset StartUtc,
    int SlotMinutes,
    string Reason) : IRequest<CreateAppointmentResponse>;

public sealed record CreateAppointmentResponse(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    DateTimeOffset StartUtc,
    int SlotMinutes,
    AppointmentStatus Status);