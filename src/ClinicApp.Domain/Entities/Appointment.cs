namespace ClinicApp.Domain.Entities;

/// <summary>
/// A clinic appointment occupying a fixed-length slot for one doctor.
/// Start is stored in UTC. Slot length is configured (default 30 minutes).
/// </summary>
public class Appointment
{
    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public DateTimeOffset StartUtc { get; private set; }
    public long StartUtcTicks { get; private set; }
    public int SlotMinutes { get; private set; }
    public long EndUtcTicks { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public long RowVersion { get; private set; }

    internal Appointment() { } // EF Core

    public Appointment(Guid patientId, Guid doctorId, DateTimeOffset startUtc, int slotMinutes, string reason)
    {
        Id = Guid.NewGuid();
        PatientId = patientId;
        DoctorId = doctorId;
        StartUtc = startUtc;
        StartUtcTicks = startUtc.UtcTicks;
        SlotMinutes = slotMinutes;
        EndUtcTicks = startUtc.AddMinutes(slotMinutes).UtcTicks;
        Reason = reason;
        Status = AppointmentStatus.Scheduled;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        RowVersion = 1;
    }

    public DateTimeOffset EndUtc => StartUtc.AddMinutes(SlotMinutes);

    /// <summary>True when this appointment's [StartUtc, EndUtc) overlaps the given slot.</summary>
    public bool Overlaps(DateTimeOffset otherStart, int otherSlotMinutes)
    {
        var otherEnd = otherStart.AddMinutes(otherSlotMinutes);
        return StartUtc < otherEnd && otherStart < EndUtc;
    }
}