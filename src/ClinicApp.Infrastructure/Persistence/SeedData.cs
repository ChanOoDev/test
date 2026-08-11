using ClinicApp.Domain.Entities;

namespace ClinicApp.Infrastructure.Persistence;

/// <summary>
/// Deterministic seed data so integration tests and local dev share stable IDs.
/// Staff (admin/receptionist/doctor) link to the auth stub identity (R4).
/// </summary>
public static class SeedData
{
    // Auth stub user ids (R4 local stub)
    public static readonly Guid AdminUserId = Guid.Parse("11111111-1111-1111-1111-111111111101");
    public static readonly Guid ReceptionistUserId = Guid.Parse("11111111-1111-1111-1111-111111111102");
    public static readonly Guid DoctorUserId = Guid.Parse("11111111-1111-1111-1111-111111111103");

    // Person row ids (deterministic)
    public static readonly Guid DoctorId = Guid.Parse("22222222-2222-2222-2222-222222222201");
    public static readonly Guid SecondDoctorId = Guid.Parse("22222222-2222-2222-2222-222222222202");

    public static readonly Guid PatientId = Guid.Parse("33333333-3333-3333-3333-333333333301");
    public static readonly Guid SecondPatientId = Guid.Parse("33333333-3333-3333-3333-333333333302");
    public static readonly Guid InactivePatientId = Guid.Parse("33333333-3333-3333-3333-333333333303");

    public static readonly Guid AdminPersonId = Guid.Parse("44444444-4444-4444-4444-444444444401");
    public static readonly Guid ReceptionistPersonId = Guid.Parse("44444444-4444-4444-4444-444444444402");

    public static IReadOnlyList<Person> People => new List<Person>
    {
        new("Admin User", PersonRole.Admin, isActive: true, staffUserId: AdminUserId, id: AdminPersonId),
        new("Receptionist User", PersonRole.Receptionist, isActive: true, staffUserId: ReceptionistUserId, id: ReceptionistPersonId),
        new("Dr. Alice", PersonRole.Doctor, isActive: true, staffUserId: DoctorUserId, id: DoctorId),
        new("Dr. Bob", PersonRole.Doctor, isActive: true, id: SecondDoctorId),
        new("Patient One", PersonRole.Patient, isActive: true, id: PatientId),
        new("Patient Two", PersonRole.Patient, isActive: true, id: SecondPatientId),
        new("Patient Inactive", PersonRole.Patient, isActive: false, id: InactivePatientId),
    };
}