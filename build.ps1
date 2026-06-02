#!/usr/bin/pwsh
$ErrorActionPreference = "Stop"
$CURRENTPATH = $pwd.Path

# the custom nuget package will use the net48 build output, so we need to build that first before creating the nuget package. We will also publish the net10.0 build output for distribution, but that is not used for the nuget package.
New-Item -Path "$CURRENTPATH\build" -ItemType Directory -Force
dotnet build TownSuite.TwainScanner.sln -c Release

#  build output for distribution
dotnet publish TownSuite.TwainScanner.sln -c Release -r "win-x64" --self-contained true -f net10.0 -o "$CURRENTPATH\build\TownSuite.TwainScanner"