#!/usr/bin/pwsh
$ErrorActionPreference = "Stop"
$CURRENTPATH = $pwd.Path

# zip the net8.0-windows build output for distribution
Compress-Archive -Path "$CURRENTPATH\build\TownSuite.TwainScanner\*" -DestinationPath "$CURRENTPATH\build\TownSuite.TwainScanner.zip" -Force