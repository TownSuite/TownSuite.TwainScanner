#!/usr/bin/pwsh
$ErrorActionPreference = "Stop"
$CURRENTPATH = $pwd.Path

# See https://github.com/baseclass/Contrib.Nuget, nuget by default does not let output content 


function delete_files([string]$path) {
	If (Test-Path $path) {
		Write-Host "Deleting path $path" -ForegroundColor Green
		Remove-Item -recurse -force $path
	}
}

delete_files "TownSuite.TwainScanner/content"
delete_files "NugetBuild"
mkdir -p TownSuite.TwainScanner/content/TownSuite.TwainScanner
mkdir -p NugetBuild

Copy-Item -Recurse -Force "../build/TownSuite.TwainScanner/*" TownSuite.TwainScanner/content/TownSuite.TwainScanner/

$dotnet = (Get-Command dotnet -ErrorAction SilentlyContinue).Source
if (-not $dotnet) {
	Write-Error "'dotnet' CLI is not available on PATH. Install .NET SDK to proceed."
	exit 1
}

$projectPath = Join-Path $CURRENTPATH "TownSuite.TwainScanner/TownSuite.TwainScanner.csproj"
if (-not (Test-Path $projectPath)) {
	Write-Error "Project not found: $projectPath"
	exit 1
}

# dotnet pack is project-based: it cannot pack a bare nuspec, so we drive it
# through the stub csproj that references the nuspec. Output path must be
# absolute because dotnet pack resolves relative paths against the project dir.
$outDir = Join-Path $CURRENTPATH "../build"
New-Item -Path $outDir -ItemType Directory -Force | Out-Null
$outDir = (Resolve-Path $outDir).Path

Write-Host "Packing using dotnet with project: $projectPath" -ForegroundColor Green

& $dotnet pack "$projectPath" -c Release -p:PackageOutputPath="$outDir"
if ($LASTEXITCODE -ne 0) { Write-Error "dotnet pack failed"; exit $LASTEXITCODE }

