# Project AI Rules

Follow `.claude/rules/*`.

Mandatory:

- Read the assigned issue before coding.
- Respect architecture decisions and existing patterns.
- Do not implement unrelated backlog items.
- Prefer vertical slices.
- Enforce authorization server-side.
- Treat RLS / policy checks as security boundaries where applicable.
- Validate external input.
- Add or update tests for changed behavior.
- Run lint, typecheck, tests, and build where applicable.
- Never expose secrets in logs, prompts, commits, or generated artifacts.
