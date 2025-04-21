# Exit on error
$ErrorActionPreference = "Stop"

# Move to repo root (assuming this script is in ./IaaC)
Set-Location (Join-Path $PSScriptRoot "..")

$buildDir = "./build"
$nugetSource = "https://nuget.pkg.github.com/Quantum-Space-Org/index.json"

# Get all .nupkg files
$packages = Get-ChildItem -Path $buildDir -Filter *.nupkg

if (-not $packages) {
  Write-Host "❌ No .nupkg files found in $buildDir"
  exit 1
}

# Group by package name and get the highest version
$latestPackages = $packages |
ForEach-Object {
  # Example: Quantum.Core.1.0.2.nupkg -> [Name=Quantum.Core, Version=1.0.2]
  if ($_ -match "^(.*)\.(\d+\.\d+\.\d+(-[A-Za-z0-9\.]+)?)\.nupkg$") {
    [PSCustomObject]@{
      Name
