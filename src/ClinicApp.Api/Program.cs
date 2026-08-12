using System.Text.Json.Serialization;
using ClinicApp.Api.Auth;
using ClinicApp.Api.Middleware;
using ClinicApp.Application.Abstractions;
using ClinicApp.Infrastructure;
using ClinicApp.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<ClinicApp.Application.UseCases.Appointments.CreateAppointment.CreateAppointmentCommand>();
    cfg.AddOpenBehavior(typeof(ClinicApp.Application.Behaviors.ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(
    typeof(ClinicApp.Application.UseCases.Appointments.CreateAppointment.CreateAppointmentCommand).Assembly);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services
    .AddAuthentication("Stub")
    .AddScheme<AuthenticationSchemeOptions, ClinicApp.Api.Auth.StubAuthHandler>("Stub", null);
builder.Services.AddAuthorizationBuilder();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUserService>();
builder.Services.AddScoped<IDoctorLookup, DoctorLookup>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Apply migrations and seed on startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<AuthStubMiddleware>();
app.UseExceptionHandler();
app.UseAuthorization();

app.MapGet("/health", async (ClinicDbContext db, CancellationToken ct) =>
{
    var dbOk = await db.Database.CanConnectAsync(ct);
    return Results.Ok(new
    {
        status = dbOk ? "ok" : "degraded",
        version = "4.0.0",
        database = dbOk ? "ok" : "unreachable",
        utc = DateTimeOffset.UtcNow,
    });
})
.AllowAnonymous();

app.MapControllers();

app.Run();

public partial class Program;
