using ClinicApp.Domain.Entities;
using MediatR;

namespace ClinicApp.Application.UseCases.Appointments.GetDayAppointments;

/// <summary>
/// Query for a single day's appointments. Date is required; doctor and status
/// filters are optional. Server-side role scoping: receptionist/admin see all;
/// a doctor sees only their own appointments.
/// </summary>
public sealed record GetDayAppointmentsQuery(
    DateOnly Date,
    Guid? DoctorId,
    AppointmentStatus? Status) : IRequest<GetDayAppointmentsResponse>;

public sealed record GetDayAppointmentsResponse(IReadOnlyList<DayAppointmentDto> Appointments);

/// <summary>Appointment projection for the day view.</summary>
public sealed record DayAppointmentDto(
    Guid Id,
    Guid PatientId,
    string PatientName,
    Guid DoctorId,
    string DoctorName,
    DateTimeOffset StartUtc,
    int SlotMinutes,
    AppointmentStatus Status,
    string Reason);
