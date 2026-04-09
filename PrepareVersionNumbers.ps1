#!/usr/bin/env pwsh
$ErrorActionPreference = "Stop"
$CURRENTPATH=$pwd.Path

function RunSed($cmd, $path){
	If ($osversion -eq "linux") {
		& "sed" -i $cmd "$path"
	}
	else {
		& "sed.exe" -i $cmd "$path"
	}	
}

function RunVerionUpdater($loc, $path){
	If ($IsWindows) {
		& "./Build/AssemblyInfoUtil.exe" -inc:$loc "$path"
	}
	else {
		assemblyinfoutil -inc:$loc "$path"
	}	
}


RunVerionUpdater 2 "$CURRENTPATH/TownSuite.TwainScanner/TwainScanner.csproj"
RunVerionUpdater 2 "$CURRENTPATH/nugetspec/TownSuite.TwainScanner/TownSuite.TwainScanner.nuspec"
