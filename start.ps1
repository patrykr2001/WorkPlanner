#!/usr/bin/env pwsh
#Requires -Version 7.0

<#
.SYNOPSIS
    Uruchamia WorkPlanner API i Client jednocześnie.

.DESCRIPTION
    Skrypt uruchamia WorkPlanner.Api i WorkPlanner.Client w osobnych procesach,
    czeka na naciśnięcie klawisza, a następnie zatrzymuje oba serwisy.
#>

param(
    [switch]$NoBuild,
    [switch]$NoLaunch
)

$ErrorActionPreference = "Stop"

# Kolory do logowania
function Write-Info { param([string]$Message) Write-Host "[INFO] $Message" -ForegroundColor Cyan }
function Write-Success { param([string]$Message) Write-Host "[OK] $Message" -ForegroundColor Green }
function Write-Error { param([string]$Message) Write-Host "[ERROR] $Message" -ForegroundColor Red }

# Ścieżka do katalogu solucji
$SolutionDir = $PSScriptRoot
Set-Location $SolutionDir

Write-Info "WorkPlanner - Uruchamianie API i Client..."
Write-Info "Katalog: $SolutionDir"

# Budowanie (opcjonalnie)
if (-not $NoBuild) {
    Write-Info "Budowanie projektów..."
    dotnet build WorkPlanner.Api/WorkPlanner.Api.csproj --configuration Debug | Out-Null
    dotnet build WorkPlanner.Client/WorkPlanner.Client.csproj --configuration Debug | Out-Null
    Write-Success "Budowanie zakończone"
}

# Uruchomienie API
Write-Info "Uruchamianie API (https://localhost:7191)..."
$ApiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", "WorkPlanner.Api/WorkPlanner.Api.csproj", "--launch-profile", "https", "--no-build:$NoBuild" -PassThru -WindowStyle Normal

# Poczekaj chwilę na uruchomienie API
Start-Sleep -Seconds 3

# Sprawdź czy API się uruchomiło
if ($ApiProcess.HasExited) {
    Write-Error "API nie uruchomiło się poprawnie!"
    exit 1
}

Write-Success "API uruchomione (PID: $($ApiProcess.Id))"

# Uruchomienie Client
Write-Info "Uruchamianie Client (https://localhost:7127)..."
$ClientArgs = @("run", "--project", "WorkPlanner.Client/WorkPlanner.Client.csproj", "--launch-profile", "https", "--no-build:$NoBuild")
if ($NoLaunch) {
    $ClientArgs += "--no-launch-profile"
}

$ClientProcess = Start-Process -FilePath "dotnet" -ArgumentList $ClientArgs -PassThru -WindowStyle Normal

# Poczekaj chwilę na uruchomienie Client
Start-Sleep -Seconds 3

# Sprawdź czy Client się uruchomił
if ($ClientProcess.HasExited) {
    Write-Error "Client nie uruchomił się poprawnie!"
    Stop-Process -Id $ApiProcess.Id -Force -ErrorAction SilentlyContinue
    exit 1
}

Write-Success "Client uruchomiony (PID: $($ClientProcess.Id))"
Write-Host ""
Write-Success "==============================================="
Write-Success "WorkPlanner jest uruchomiony!"
Write-Success "API:    https://localhost:7191"
Write-Success "Client: https://localhost:7127"
Write-Success "==============================================="
Write-Host ""
Write-Host "Naciśnij ENTER, aby zatrzymać oba serwisy..." -ForegroundColor Yellow

# Czekaj na naciśnięcie klawisza
$null = [Console]::ReadLine()

# Zatrzymanie procesów
Write-Info "Zatrzymywanie serwisów..."

try {
    if (-not $ClientProcess.HasExited) {
        Stop-Process -Id $ClientProcess.Id -Force -ErrorAction SilentlyContinue
        Write-Success "Client zatrzymany"
    }
} catch {
    Write-Error "Błąd podczas zatrzymywania Client: $_"
}

try {
    if (-not $ApiProcess.HasExited) {
        Stop-Process -Id $ApiProcess.Id -Force -ErrorAction SilentlyContinue
        Write-Success "API zatrzymane"
    }
} catch {
    Write-Error "Błąd podczas zatrzymywania API: $_"
}

Write-Success "WorkPlanner zatrzymany."
