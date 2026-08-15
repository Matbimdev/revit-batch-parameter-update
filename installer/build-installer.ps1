<#
.SYNOPSIS
    Builds the Batch Parameter Update installer in one command.

.DESCRIPTION
    Compiles the four Release configurations, one per supported Revit version, then
    runs the Inno Setup compiler to produce Output\BatchParameterUpdate-Setup-<Version>.exe.

.PARAMETER Version
    Version stamped on the setup executable and shown in Programs and Features.

.PARAMETER SkipBuild
    Reuse whatever is already in bin\Release.*, for a faster repackage while iterating
    on the installer script itself.

.EXAMPLE
    .\build-installer.ps1 -Version 1.0.0
#>

param(
    [string] $Version = "1.0.0",
    [switch] $SkipBuild
)

$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot

try {
    # --- Locate the Inno Setup compiler ------------------------------------
    $isccCandidates = @(
        "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe",
        "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
        "$env:ProgramFiles\Inno Setup 6\ISCC.exe"
    )

    $iscc = $isccCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
    if (-not $iscc) {
        $onPath = Get-Command ISCC.exe -ErrorAction SilentlyContinue
        if ($onPath) { $iscc = $onPath.Source }
    }
    if (-not $iscc) {
        throw "ISCC.exe not found. Install Inno Setup 6:  winget install JRSoftware.InnoSetup"
    }

    Write-Host "Using ISCC: $iscc" -ForegroundColor Cyan

    # --- Compile one Release build per supported Revit version -------------
    if ($SkipBuild) {
        Write-Host "`n[1/2] Skipping build, reusing existing bin\Release.* output." -ForegroundColor Yellow
    }
    else {
        Write-Host "`n[1/2] Building Release configurations..." -ForegroundColor Green

        foreach ($configuration in @("Release.R23", "Release.R24", "Release.R25", "Release.R26")) {
            Write-Host "  $configuration" -ForegroundColor DarkGray
            dotnet build ..\BatchParameterUpdate.sln -c $configuration -v minimal --nologo
            if ($LASTEXITCODE -ne 0) { throw "Build failed for configuration $configuration" }
        }
    }

    # --- Compile the installer ---------------------------------------------
    Write-Host "`n[2/2] Compiling installer $Version..." -ForegroundColor Green
    & $iscc "/DMyAppVersion=$Version" "BatchParameterUpdate.iss"
    if ($LASTEXITCODE -ne 0) { throw "ISCC failed with exit code $LASTEXITCODE" }

    $output = Join-Path $PSScriptRoot "Output\BatchParameterUpdate-Setup-$Version.exe"
    Write-Host "`nDone -> $output" -ForegroundColor Cyan
}
finally {
    Pop-Location
}
