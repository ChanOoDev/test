$ErrorActionPreference = "Stop"
Write-Host "Enterprise AI Engineering Platform v4"
Write-Host "Checking required folders..."

$required = @(
  ".claude\agents",
  ".claude\commands",
  ".claude\templates",
  ".claude\rules",
  ".claude\governance",
  "docs\prd"
)

foreach ($path in $required) {
  if (!(Test-Path $path)) {
    throw "Missing required path: $path"
  }
}

Write-Host "Platform structure OK"
Write-Host "Next: add your PRD and run /backlog <prd-path>"
