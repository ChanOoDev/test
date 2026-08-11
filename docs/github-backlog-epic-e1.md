# Epic: Clinic Appointment Management

## Objective
Allow clinic staff to schedule future patient appointments — create, view, reschedule, cancel, and track status — with role-based access and double-booking prevention.

## Business Value
Receptionists book/run the day reliably, doctors see their own schedule and complete visits, admins manage all. Preventing double-bookings avoids scheduling conflicts and reduces rework.

## Scope

### In Scope
- Create appointment (receptionist/admin)
- Reschedule and cancel appointment
- Daily appointment view (receptionist/admin)
- Doctor appointment workspace (view own only)
- Status lifecycle: scheduled → checked_in → completed; → cancelled
- Double-booking prevention
- RBAC: receptionist=manage, doctor=view own, admin=manage all

### Out of Scope
- Patient self-booking
- SMS
- Payments
- Recurring appointments

## Architecture / Domain Impact
- Stack: ASP.NET Core Web API, Clean Architecture, CQRS where it adds value, FluentValidation, OpenAPI; React + TypeScript; AWS IaC, least-privilege IAM, env separation.
- New Appointment aggregate/table + migration. Patients/doctors referenced as existing master data (assumed).
- Server-side RBAC enforced; UI hiding is not authorization.
- Double-booking must be a DB-level guard (transaction/constraint), not an app-layer read-then-write.
- Timezone: normalize server-side, store UTC.
- Status transitions enforced server-side via a shared validator.
- ADR candidates: double-booking enforcement strategy; appointment status state machine; UTC/timezone storage convention.

## Tasks
- [ ] Story 1 — Create appointment (#2)
- [ ] Story 2 — Daily appointment view (#3)
- [ ] Story 3 — Reschedule and cancel (#4)
- [ ] Story 4 — Doctor appointment workspace (#5)
- [ ] Story 5 — Status lifecycle (#6)

## Acceptance Criteria
- Receptionist can create/reschedule/cancel an appointment (assuming that maps to "manage").
- Doctor can view only their own appointments and can complete their own; cannot create/reschedule/cancel.
- Admin can view/manage all appointments.
- No double-booking possible for a doctor at the same time (server-enforced, concurrency-safe).
- All statuses: scheduled, checked_in, completed, cancelled; illegal transitions rejected.

## Risks / Assumptions
- A1 (RESOLVED 2026-08-11): Fixed configurable slot length, default 30 min. Conflict = same doctor, overlapping fixed slot on same day.
- A2: Conflict = overlapping time, same doctor. No clinic-wide/room resource constraints in PRD.
- A3: Check-in = receptionist/admin; complete = doctor(own)/admin. PRD assigns roles only for manage/view, not status transitions.
- A4: `checked_in` appointments can still be rescheduled/cancelled.
- A5: Cancel reason recorded but not required.
- A6: Patient/doctor master data exists.
- R1: Double-booking race (two receptionists same slot) — DB-level constraint + transaction.
- R2: Timezone errors — UTC server-side, render local.
- R3: Status machine drift — single shared server-side validator.
- R4: AWS identity provider (Cognito vs custom) undefined — RBAC middleware shape depends on it.
- R5: S3 and S5 overlap on status rules — share transition validator; merge if review finds duplication.