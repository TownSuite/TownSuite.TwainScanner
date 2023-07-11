

# See https://github.com/baseclass/Contrib.Nuget, nuget by default does not let output content 


function delete_files([string]$path) {
	If (Test-Path $path) {
		Write-Host "Deleting path $path" -ForegroundColor Green
		Remove-Item -recurse -force $path
	}
}

delete_files "TownSuite.TwainScanner\output"
delete_files "NugetBuild"
mkdir -force TownSuite.TwainScanner\output
mkdir -force NugetBuild

Copy-Item -force "..\TownSuite.TwainScanner\bin\x86\Release\net48\TownSuite.TwainScanner.exe" TownSuite.TwainScanner\output\TownSuite.TwainScanner.exe

nuget pack TownSuite.TwainScanner\TownSuite.TwainScanner.nuspec -OutputDirectory .\NugetBuild

