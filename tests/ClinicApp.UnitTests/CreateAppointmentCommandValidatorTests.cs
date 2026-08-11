using ClinicApp.Application.UseCases.Appointments.CreateAppointment;

namespace ClinicApp.UnitTests;

public class CreateAppointmentCommandValidatorTests
{
    private readonly CreateAppointmentCommandValidator _validator = new();

    private static CreateAppointmentCommand Command(DateTimeOffset start, int slotMinutes = 30)
        => new(
            PatientId: Guid.NewGuid(),
            DoctorId: Guid.NewGuid(),
            StartUtc: start,
            SlotMinutes: slotMinutes,
            Reason: "checkup");

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    public void Future_date_is_valid(int daysAhead)
    {
        // Round to a 15-minute boundary so only the future rule is under test.
        var now = DateTimeOffset.UtcNow.AddDays(daysAhead);
        var aligned = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour + 2, 0, 0, TimeSpan.Zero);
        var result = _validator.Validate(Command(aligned));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Past_date_is_invalid()
    {
        var command = Command(DateTimeOffset.UtcNow.AddDays(-1));
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "Appointment.PastDate");
    }

    [Fact]
    public void Zero_slot_is_invalid()
    {
        var command = Command(DateTimeOffset.UtcNow.AddDays(1), slotMinutes: 0);
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "Appointment.InvalidSlot");
    }

    [Fact]
    public void Unaligned_start_is_invalid()
    {
        var command = Command(DateTimeOffset.UtcNow.AddDays(1).AddMinutes(7));
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "Appointment.UnalignedStart");
    }

    [Fact]
    public void EffectiveSlotMinutes_defaults_to_30_when_not_provided()
    {
        Assert.Equal(30, CreateAppointmentCommandValidator.EffectiveSlotMinutes(null));
        Assert.Equal(30, CreateAppointmentCommandValidator.EffectiveSlotMinutes(0));
    }
}