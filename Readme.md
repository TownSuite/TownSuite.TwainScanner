Scanning component which allows you to control work of flatbed scanner, 
web and digital camera and any other TWAIN device from .NET environment. 
You can use this library in your programs written in any programming languages compatible with .NET technology.


Fork of https://sarafftwain.codeplex.com/


# Scanner setup

* Install your printers twain driver with its official driver pack

# Usage

```powershell
.\TownSuite.TwainScanner.exe
```

Available output filetypes.

* PDF
* PNG
* JPG
* TIFF

Set a default selected output filetype and scanner.  

```powershell
.\TownSuite.TwainScanner.exe -scansettings "JPG \"Brother DS-620\"" 
```


## Enable ocr
The following example enables ocr.   To use ocr the following application arguments must be set on startup.

* -enableocr 
* -ocrapiurl "PLACEHOLDER" 
* -ocrbearertoken "PLACEHOLDER" 
* Optionally a working directory for temp files can be set
  * -tempdir "PLACEHOLDER"

```powershell
TownSuite.TwainScanner.exe -enableocr -ocrapiurl "PLACEHOLDER" -ocrbearertoken "PLACEHOLDER" -tempdir "PLACEHOLDER"
```

