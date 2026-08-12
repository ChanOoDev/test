using FluentValidation;

namespace ClinicApp.Application.UseCases.Appointments.GetDayAppointments;

public sealed class GetDayAppointmentsQueryValidator : AbstractValidator<GetDayAppointmentsQuery>
{
    private static readonly DateOnly MinDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1));
    private static readonly DateOnly MaxDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(5));

    public GetDayAppointmentsQueryValidator()
    {
        RuleFor(x => x.Date)
            .GreaterThanOrEqualTo(MinDate)
            .WithMessage("Date is too far in the past.")
            .WithErrorCode("Appointment.InvalidDate")
            .LessThanOrEqualTo(MaxDate)
            .WithMessage("Date is too far in the future.")
            .WithErrorCode("Appointment.InvalidDate");
    }
}
