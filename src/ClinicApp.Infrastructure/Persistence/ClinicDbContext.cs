using ClinicApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ClinicApp.Infrastructure.Persistence;

public class ClinicDbContext(DbContextOptions<ClinicDbContext> options) : DbContext(options)
{
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Person> People => Set<Person>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(b =>
        {
            b.ToTable("Appointments");
            b.HasKey(a => a.Id);

            b.Property(a => a.PatientId).IsRequired();
            b.Property(a => a.DoctorId).IsRequired();
            b.Property(a => a.StartUtc).IsRequired();
            b.Property(a => a.StartUtcTicks).IsRequired();
            b.Property(a => a.SlotMinutes).IsRequired();
            b.Property(a => a.EndUtcTicks).IsRequired();
            b.Property(a => a.Reason).HasMaxLength(500);
            b.Property(a => a.Status)
                .HasConversion(new EnumToStringConverter<AppointmentStatus>());

            // Concurrency token — optimistic locking for reschedule/cancel (S3).
            // SQLite has no native rowversion; a long version set on insert and
            // compared on update provides optimistic concurrency.
            b.Property(a => a.RowVersion).IsConcurrencyToken();

            // Index for day views (S2) and conflict scans.
            b.HasIndex(a => new { a.DoctorId, a.StartUtc });
        });

        modelBuilder.Entity<Person>(b =>
        {
            b.ToTable("People");
            b.HasKey(p => p.Id);
            b.Property(p => p.Name).IsRequired().HasMaxLength(200);
            b.Property(p => p.Role).IsRequired();
            b.Property(p => p.IsActive).IsRequired();
            b.Property(p => p.StaffUserId);
        });
    }
}