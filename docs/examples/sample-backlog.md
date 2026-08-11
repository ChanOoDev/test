# Example Backlog

## Epic E1 — Appointment Management

### E1-S1 Create appointment workflow
Vertical slice: schema + server action/API + form + validation + authorization + tests.

### E1-S2 Daily appointment view
List/filter appointments by day and role scope.

### E1-S3 Reschedule and cancellation
Support schedule changes and cancellation history.

### E1-S4 Doctor appointment workspace
Doctors see only appointments assigned to them.

### E1-S5 Appointment lifecycle
Manage status transitions safely.

## Dependency Graph

E1-S1 -> E1-S2
E1-S1 -> E1-S3
E1-S1 -> E1-S4
E1-S1 -> E1-S5
