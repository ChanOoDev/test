# Example Stack Profile: .NET + React + AWS

Backend:
- ASP.NET Core Web API
- Clean Architecture
- CQRS only where it adds value
- FluentValidation
- OpenAPI

Frontend:
- React + TypeScript
- explicit API boundary
- schema validation where practical

AWS:
- IaC required for repeatable infrastructure
- least-privilege IAM
- environment separation
- logs/metrics/alarms
- immutable deployment artifacts
