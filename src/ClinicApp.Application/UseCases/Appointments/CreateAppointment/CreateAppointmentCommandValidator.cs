using FluentValidation;

namespace ClinicApp.Application.UseCases.Appointments.CreateAppointment;

public sealed class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    private static readonly TimeSpan MinLead = TimeSpan.FromMinutes(5);
    private static readonly int DefaultSlotMinutes = 30;

    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.StartUtc)
            .Must(start => start > DateTimeOffset.UtcNow.Add(MinLead))
            .WithMessage("Appointment time must be in the future.")
            .WithErrorCode("Appointment.PastDate");

        RuleFor(x => x.SlotMinutes)
            .Must(slot => slot > 0 && slot <= 240)
            .WithMessage("Slot length must be positive and at most 4 hours.")
            .WithErrorCode("Appointment.InvalidSlot");

        RuleFor(x => x.StartUtc)
            .Must(start => start.Minute % 15 == 0)
            .WithMessage("Appointment start must align to a 15-minute boundary.")
            .WithErrorCode("Appointment.UnalignedStart");

        RuleFor(x => x.Reason)
            .MaximumLength(500);
    }

    public static int EffectiveSlotMinutes(int? requested) => requested is > 0 ? requested.Value : DefaultSlotMinutes;
}