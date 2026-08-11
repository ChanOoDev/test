using FluentValidation;
using MediatR;

namespace ClinicApp.Application.Behaviors;

/// <summary>
/// Runs FluentValidation validators for each request before its handler.
/// Throws <see cref="ValidationException"/> on failure, mapped to 422 by the
/// API global exception handler.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var failures = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, ct)));

            var errors = failures
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (errors.Count > 0)
                throw new ValidationException(errors);
        }

        return await next();
    }
}