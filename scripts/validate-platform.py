from pathlib import Path
import sys

required = [
    ".claude/agents/product-owner.md",
    ".claude/agents/architect.md",
    ".claude/agents/developer.md",
    ".claude/commands/backlog.md",
    ".claude/commands/feature.md",
    ".claude/templates/epic.md",
    ".claude/templates/story.md",
    ".claude/rules/backlog-rules.md",
    ".claude/governance/definition-of-done.md",
]

root = Path(__file__).resolve().parents[1]
missing = [p for p in required if not (root / p).exists()]

if missing:
    print("Missing required files:")
    for m in missing:
        print("-", m)
    sys.exit(1)

print("Platform validation: OK")
