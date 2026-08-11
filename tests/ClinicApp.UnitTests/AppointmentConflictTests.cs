using ClinicApp.Domain.Entities;

namespace ClinicApp.UnitTests;

public class AppointmentConflictTests
{
    private static readonly Guid Patient = Guid.Parse("33333333-3333-3333-3333-333333333301");
    private static readonly Guid Doctor = Guid.Parse("22222222-2222-2222-2222-222222222201");

    private static DateTimeOffset At(int hour, int minute = 0)
        => new(2099, 1, 1, hour, minute, 0, TimeSpan.Zero);

    [Fact]
    public void Reservation_has_default_slot_of_30_minutes()
    {
        var appointment = new Appointment(Patient, Doctor, At(10), 30, "checkup");
        Assert.Equal(At(10), appointment.StartUtc);
        Assert.Equal(At(10, 30), appointment.EndUtc);
        Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
    }

    [Fact]
    public void Overlaps_detects_same_start()
    {
        var appointment = new Appointment(Patient, Doctor, At(10), 30, "checkup");
        Assert.True(appointment.Overlaps(At(10), 30));
    }

    [Theory]
    [InlineData(10, 5)]   // inside the slot
    [InlineData(10, 25)]  // straddles the end
    [InlineData(9, 50)]   // straddles the start
    public void Overlaps_detects_partial_overlap(int hour, int minute)
    {
        var appointment = new Appointment(Patient, Doctor, At(10), 30, "checkup");
        Assert.True(appointment.Overlaps(At(hour, minute), 30));
    }

    [Theory]
    [InlineData(10, 30)]  // adjacent — no overlap
    [InlineData(11, 0)]   // later
    [InlineData(9, 0)]    // earlier
    public void Overlaps_allows_adjacent_and_separate_times(int hour, int minute)
    {
        var appointment = new Appointment(Patient, Doctor, At(10), 30, "checkup");
        Assert.False(appointment.Overlaps(At(hour, minute), 30));
    }
}