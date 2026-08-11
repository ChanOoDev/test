using ClinicApp.Application.Abstractions;
using ClinicApp.Infrastructure.Persistence;
using ClinicApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ClinicDb")
            ?? "Data Source=clinic.db";

        services.AddDbContext<ClinicDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        return services;
    }
}