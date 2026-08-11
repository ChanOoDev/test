using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ClinicApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.IntegrationTests;

/// <summary>
/// In-memory API host using an isolated temp SQLite DB, seeded with stub data.
/// </summary>
public class ClinicApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Replace the app's DbContext with an isolated temp SQLite DB.
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ClinicDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            var dbPath = Path.Combine(Path.GetTempPath(), $"clinic_test_{Guid.NewGuid():N}.db");
            services.AddDbContext<ClinicDbContext>(o => o.UseSqlite($"Data Source={dbPath}"));
        });
    }
}
