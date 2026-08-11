using ClinicApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClinicApp.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ClinicDbContext>
{
    public ClinicDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ClinicDbContext>()
            .UseSqlite("Data Source=clinic.db")
            .Options;

        return new ClinicDbContext(options);
    }
}