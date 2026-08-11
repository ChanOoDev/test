namespace ClinicApp.Domain.Entities;

public enum PersonRole
{
    Admin = 1,
    Receptionist = 2,
    Doctor = 3,
    Patient = 4,
}

/// <summary>
/// A person in the system: staff (admin/receptionist/doctor) or a patient.
/// Patients and doctors are master data; staff link to the auth identity.
/// </summary>
public class Person
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public PersonRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid? StaffUserId { get; private set; }

    internal Person() { } // EF Core

    public Person(string name, PersonRole role, bool isActive = true, Guid? staffUserId = null, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        Role = role;
        IsActive = isActive;
        StaffUserId = staffUserId;
    }
}