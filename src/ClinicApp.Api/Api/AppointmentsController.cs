using ClinicApp.Api.Auth;
using ClinicApp.Application.UseCases.Appointments.CreateAppointment;
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
}

public sealed record CreateAppointmentRequestDto(
    Guid PatientId,
    Guid DoctorId,
    DateTimeOffset StartUtc,
    int SlotMinutes,
    string? Reason);