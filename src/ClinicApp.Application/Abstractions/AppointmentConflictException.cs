namespace ClinicApp.Application.Abstractions;

/// <summary>Raised when a requested slot overlaps an existing appointment for the same doctor.</summary>
public sealed class AppointmentConflictException : Exception
{
    public AppointmentConflictException()
        : base("The selected time conflicts with an existing appointment for this doctor.") { }
}