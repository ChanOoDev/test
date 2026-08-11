using ClinicApp.Application.Abstractions;
using ClinicApp.Domain.Entities;
using MediatR;

namespace ClinicApp.Application.UseCases.Appointments.CreateAppointment;

public sealed class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, CreateAppointmentResponse>
{
    private static readonly int DefaultSlotMinutes = 30;
    private readonly IAppointmentRepository _repository;

    public CreateAppointmentCommandHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateAppointmentResponse> Handle(CreateAppointmentCommand request, CancellationToken ct)
    {
        var slot = request.SlotMinutes > 0 ? request.SlotMinutes : DefaultSlotMinutes;

        if (!await _repository.PatientExistsAsync(request.PatientId, ct))
            throw new PatientNotFoundException(request.PatientId);

        if (!await _repository.DoctorExistsAsync(request.DoctorId, ct))
            throw new DoctorNotFoundException(request.DoctorId);

        var appointment = new Appointment(
            request.PatientId,
            request.DoctorId,
            request.StartUtc,
            slot,
            request.Reason);

        await _repository.AddAsync(appointment, ct); // throws AppointmentConflictException on overlap

        return new CreateAppointmentResponse(
            appointment.Id,
            appointment.PatientId,
            appointment.DoctorId,
            appointment.StartUtc,
            appointment.SlotMinutes,
            appointment.Status);
    }
}