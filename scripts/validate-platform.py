from pathlib import Path
import sys

required = [
    ".ai/agents/product-owner.md",
    ".ai/agents/architect.md",
    ".ai/agents/developer.md",
    ".ai/commands/backlog.md",
    ".ai/commands/feature.md",
    ".ai/templates/epic.md",
    ".ai/templates/story.md",
    ".ai/rules/backlog-rules.md",
    ".ai/governance/definition-of-done.md",
]

root = Path(__file__).resolve().parents[1]
missing = [p for p in required if not (root / p).exists()]

if missing:
    print("Missing required files:")
    for m in missing:
        print("-", m)
    sys.exit(1)

print("Platform validation: OK")
