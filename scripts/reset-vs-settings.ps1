$ErrorActionPreference = "Stop"

Set-Location (Split-Path -Parent $PSScriptRoot)

Write-Host "Closing Visual Studio is recommended before running this script." -ForegroundColor Yellow

$pathsToRemove = @(
  ".vs",
  "HrSystem.Web\\bin",
  "HrSystem.Web\\obj",
  "HrSystem.Infrastructure\\bin",
  "HrSystem.Infrastructure\\obj",
  "HrSystem.Application\\bin",
  "HrSystem.Application\\obj",
  "HrSystem.Domain\\bin",
  "HrSystem.Domain\\obj"
)

foreach ($p in $pathsToRemove) {
  if (Test-Path -LiteralPath $p) {
    Write-Host "Removing $p"
    Remove-Item -LiteralPath $p -Recurse -Force
  }
}

Write-Host ""
Write-Host "Done. Reopen `HrSystem.sln` and set `HrSystem.Web` as the Startup Project." -ForegroundColor Green

