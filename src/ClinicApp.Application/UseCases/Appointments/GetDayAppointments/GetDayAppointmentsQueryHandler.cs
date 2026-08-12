using ClinicApp.Application.Abstractions;
using MediatR;

namespace ClinicApp.Application.UseCases.Appointments.GetDayAppointments;

/// <summary>
/// Day view handler. Applies server-side read-scope: receptionist/admin see
/// all appointments for the day (with optional doctor/status filters); a doctor
/// is always constrained to their own appointments regardless of requested
/// filters.
/// </summary>
public sealed class GetDayAppointmentsQueryHandler(
    IAppointmentRepository repository,
    IDoctorLookup doctorLookup,
    ICurrentUser currentUser) : IRequestHandler<GetDayAppointmentsQuery, GetDayAppointmentsResponse>
{
    public async Task<GetDayAppointmentsResponse> Handle(
        GetDayAppointmentsQuery request,
        CancellationToken ct)
    {
        // Doctors can never request another doctor's appointments. If the
        // current user is a doctor, force scope to their own doctor id.
        var doctorId = request.DoctorId;
        if (currentUser.UserId == default)
        {
            // No identity resolved (stub missing headers) — handled upstream
            // by authorization, but guard anyway.
            doctorId = null;
        }
        else
        {
            var selfDoctorId = await doctorLookup.GetDoctorIdForUserAsync(currentUser.UserId, ct);
            if (selfDoctorId is not null)
            {
                doctorId = selfDoctorId;
            }
        }

        var appointments = await repository.GetDayAsync(request.Date, doctorId, request.Status, ct);

        return new GetDayAppointmentsResponse(appointments);
    }
}
