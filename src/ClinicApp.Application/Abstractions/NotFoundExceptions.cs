namespace ClinicApp.Application.Abstractions;

/// <summary>Raised when the requested patient does not exist or is inactive.</summary>
public sealed class PatientNotFoundException : Exception
{
    public PatientNotFoundException(Guid id)
        : base($"Patient {id} was not found or is inactive.") { }
}

/// <summary>Raised when the requested doctor does not exist or is inactive.</summary>
public sealed class DoctorNotFoundException : Exception
{
    public DoctorNotFoundException(Guid id)
        : base($"Doctor {id} was not found or is inactive.") { }
}