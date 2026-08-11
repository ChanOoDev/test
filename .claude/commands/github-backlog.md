# /github-backlog <approved-backlog>

Purpose: create approved backlog in GitHub safely.

Phase 1:
- Create epics first.
- Capture TEMP_ID -> issue number.

Phase 2:
- Create child stories.
- Replace parent temporary IDs.

Phase 3:
- Patch dependencies with real issue numbers.

Phase 4:
- Update epic task checklists.

Validation:
- no unresolved TEMP IDs
- no `Part of #`
- no empty `Depends on`
- no circular dependencies
- no duplicates created

Requires explicit human-approved backlog.
