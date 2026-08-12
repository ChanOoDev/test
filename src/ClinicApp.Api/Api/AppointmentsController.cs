using ClinicApp.Api.Auth;
using ClinicApp.Application.UseCases.Appointments.CreateAppointment;
using ClinicApp.Application.UseCases.Appointments.GetDayAppointments;
using ClinicApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Api.Api;

[ApiController]
[Route("api/appointments")]
[Authorize]
public class AppointmentsController(ISender mediator) : ControllerBase
{
    /// <summary>Create an appointment. Receptionist/admin only.</summary>
    [HttpPost]
    [Authorize(Roles = $"{ClinicRoles.Admin},{ClinicRoles.Receptionist}")]
    public async Task<ActionResult<CreateAppointmentResponse>> Create(
        CreateAppointmentRequestDto dto,
        CancellationToken ct)
    {
        var command = new CreateAppointmentCommand(
            dto.PatientId,
            dto.DoctorId,
            dto.StartUtc,
            dto.SlotMinutes,
            dto.Reason ?? string.Empty);

        var result = await mediator.Send(command, ct);
        return Created($"/api/appointments/{result.Id}", result);
    }

    /// <summary>
    /// Appointments for a single day. Receptionist/admin see all; a doctor sees
    /// only their own (enforced server-side in the handler).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{ClinicRoles.Admin},{ClinicRoles.Receptionist},{ClinicRoles.Doctor}")]
    public async Task<ActionResult<GetDayAppointmentsResponse>> GetDay(
        [FromQuery] DateOnly date,
        [FromQuery] Guid? doctorId = null,
        [FromQuery] AppointmentStatus? status = null,
        CancellationToken ct = default)
    {
        var query = new GetDayAppointmentsQuery(date, doctorId, status);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }
}

public sealed record CreateAppointmentRequestDto(
    Guid PatientId,
    Guid DoctorId,
    DateTimeOffset StartUtc,
    int SlotMinutes,
    string? Reason);