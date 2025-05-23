

# See https://github.com/baseclass/Contrib.Nuget, nuget by default does not let output content 


function delete_files([string]$path) {
	If (Test-Path $path) {
		Write-Host "Deleting path $path" -ForegroundColor Green
		Remove-Item -recurse -force $path
	}
}

delete_files "TownSuite.TwainScanner\content"
delete_files "NugetBuild"
mkdir -force TownSuite.TwainScanner\content\TownSuite.TwainScanner
mkdir -force NugetBuild

Copy-Item -Recurse -Force "..\TownSuite.TwainScanner\bin\AnyCPU\Debug\net8.0-windows\publish\*" TownSuite.TwainScanner\content\TownSuite.TwainScanner\

nuget pack TownSuite.TwainScanner\TownSuite.TwainScanner.nuspec -OutputDirectory .\NugetBuild

