# Enterprise AI Engineering Platform v4

A reusable agentic engineering harness for PRD -> backlog -> architecture -> implementation -> review -> QA -> PR -> release.

## Core workflow

PRD
-> /backlog
-> Product Owner
-> Architect
-> Scrum Master
-> Human approval
-> /github-backlog
-> /feature <issue>
-> Developer
-> /review <issue>
-> /qa <issue>
-> /pr <issue>
-> /release-check

## Principles

- Small, focused prompts
- Context loaded on demand
- Human approval before backlog creation and release
- One issue normally maps to one PR
- Security and architecture are explicit review gates
- MCP/connector results should be scoped and minimal
