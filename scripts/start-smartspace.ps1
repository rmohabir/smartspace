$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$logDirectory = Join-Path $root '.smartspace-logs'
$apiLog = Join-Path $logDirectory 'api.log'
$apiErrorLog = Join-Path $logDirectory 'api-error.log'
$uiLog = Join-Path $logDirectory 'ui.log'
$uiErrorLog = Join-Path $logDirectory 'ui-error.log'

Set-Location $root

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    throw 'Docker is not installed or is not available on PATH.'
}

if (-not (Test-Path (Join-Path $root '.env'))) {
    throw 'Missing .env. Copy .env.example to .env and set SMARTSPACE_SQL_PASSWORD first.'
}

$sqlPassword = (Get-Content (Join-Path $root '.env') |
    Where-Object { $_ -match '^\s*SMARTSPACE_SQL_PASSWORD\s*=\s*(.+?)\s*$' } |
    Select-Object -First 1) -replace '^\s*SMARTSPACE_SQL_PASSWORD\s*=\s*', ''

if ([string]::IsNullOrWhiteSpace($sqlPassword) -or $sqlPassword -eq 'REPLACE_WITH_LOCAL_STRONG_PASSWORD') {
    throw 'SMARTSPACE_SQL_PASSWORD in .env is missing or still uses the example value.'
}

$env:ConnectionStrings__SmartSpace = "Server=127.0.0.1,1433;Database=SmartSpace;User Id=sa;Password=$sqlPassword;TrustServerCertificate=True;Encrypt=False"

New-Item -ItemType Directory -Path $logDirectory -Force | Out-Null

Write-Host 'Starting SQL Server...' -ForegroundColor Cyan
docker compose up -d sqlserver

Write-Host 'Waiting for SQL Server on 127.0.0.1:1433...' -ForegroundColor Cyan
$sqlReady = $false
for ($attempt = 1; $attempt -le 30; $attempt++) {
    if (Test-NetConnection -ComputerName 127.0.0.1 -Port 1433 -InformationLevel Quiet) {
        $sqlReady = $true
        break
    }

    Start-Sleep -Seconds 2
}

if (-not $sqlReady) {
    throw 'SQL Server did not become available within 60 seconds.'
}

Write-Host 'Building solution...' -ForegroundColor Cyan
dotnet build (Join-Path $root 'SmartSpace.sln') --nologo

Get-CimInstance Win32_Process -Filter "Name='dotnet.exe'" |
    Where-Object {
        $_.CommandLine -like '*src\SmartSpace.Api\SmartSpace.Api.csproj*' -or
        $_.CommandLine -like '*src\SmartSpace.UI\SmartSpace.UI.csproj*'
    } |
    ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }

Write-Host 'Starting API on http://localhost:5080...' -ForegroundColor Cyan
Start-Process dotnet `
    -WorkingDirectory $root `
    -ArgumentList @('run', '--project', 'src\SmartSpace.Api\SmartSpace.Api.csproj', '--launch-profile', 'http', '--no-build') `
    -RedirectStandardOutput $apiLog `
    -RedirectStandardError $apiErrorLog

Write-Host 'Starting UI on http://localhost:3000...' -ForegroundColor Cyan
Start-Process dotnet `
    -WorkingDirectory $root `
    -ArgumentList @('run', '--project', 'src\SmartSpace.UI\SmartSpace.UI.csproj', '--launch-profile', 'http', '--no-build') `
    -RedirectStandardOutput $uiLog `
    -RedirectStandardError $uiErrorLog

Write-Host 'Waiting for the applications...' -ForegroundColor Cyan
for ($attempt = 1; $attempt -le 30; $attempt++) {
    $apiReady = $false
    $uiReady = $false

    try {
        $apiReady = (Invoke-WebRequest 'http://localhost:5080/' -UseBasicParsing -TimeoutSec 2).StatusCode -eq 200
    } catch {
        $apiReady = $false
    }

    try {
        $uiReady = (Invoke-WebRequest 'http://localhost:3000/' -UseBasicParsing -TimeoutSec 2).StatusCode -eq 200
    } catch {
        $uiReady = $false
    }

    if ($apiReady -and $uiReady) {
        break
    }

    Start-Sleep -Seconds 2
}

if (-not $apiReady -or -not $uiReady) {
    Write-Warning "One or both applications did not respond. Check logs in $logDirectory."
} else {
    Start-Process 'http://localhost:3000'
}

Write-Host ''
Write-Host 'SmartSpace is running:' -ForegroundColor Green
Write-Host '  UI:  http://localhost:3000'
Write-Host '  API: http://localhost:5080'
Write-Host "  Logs: $logDirectory"