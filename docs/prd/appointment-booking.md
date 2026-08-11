# PRD: Clinic Appointment Booking

## Goal
Allow clinic staff to schedule future patient appointments.

## Users
- Receptionist
- Doctor
- Admin

## Requirements
- Receptionist can create, reschedule, and cancel appointments.
- Receptionist can view appointments by day.
- Doctor can view their own appointments.
- Admin can view/manage all appointments.
- Doctor double-booking must be prevented.

## Appointment Data
- patient
- doctor
- date/time
- reason
- status

## Statuses
scheduled, checked_in, completed, cancelled

## Security
- receptionist: manage appointments
- doctor: view own
- admin: manage all

## Out of Scope
- patient self-booking
- SMS
- payments
- recurring appointments
