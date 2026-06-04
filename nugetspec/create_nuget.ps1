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

$nuspecPath = Join-Path $CURRENTPATH "TownSuite.TwainScanner/TownSuite.TwainScanner.nuspec"
if (-not (Test-Path $nuspecPath)) {
	Write-Error "Nuspec not found: $nuspecPath"
	exit 1
}

Write-Host "Packing using dotnet with nuspec: $nuspecPath" -ForegroundColor Green

# Use MSBuild/NuGet pack via dotnet. Many CI setups accept passing the NuspecFile property.
& $dotnet pack -p:NuspecFile="$nuspecPath" -p:PackageOutputPath="../build" --no-build
if ($LASTEXITCODE -ne 0) { Write-Error "dotnet pack failed"; exit $LASTEXITCODE }

