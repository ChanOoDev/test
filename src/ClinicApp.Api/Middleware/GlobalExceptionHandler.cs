using ClinicApp.Application.Abstractions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Api.Middleware;

/// <summary>Maps domain exceptions to HTTP status codes with machine-readable codes.</summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken ct)
    {
        var (status, code, message) = exception switch
        {
            AppointmentConflictException => (StatusCodes.Status409Conflict, "Appointment.Conflict", exception.Message),
            PatientNotFoundException => (StatusCodes.Status422UnprocessableEntity, "Patient.NotFound", exception.Message),
            DoctorNotFoundException => (StatusCodes.Status422UnprocessableEntity, "Doctor.NotFound", exception.Message),
            FluentValidation.ValidationException ve => (StatusCodes.Status422UnprocessableEntity, "Validation.Failed", string.Join("; ", ve.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"))),
            _ => (StatusCodes.Status500InternalServerError, "Internal.Error", "An unexpected error occurred."),
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception {Code}", code);
        }

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = status,
                Title = code,
                Detail = message,
                Type = $"https://httpstatuses.com/{status}",
            },
            ct);

        return true;
    }
}