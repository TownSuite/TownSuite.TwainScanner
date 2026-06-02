using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NAPS2.Images.ImageSharp;
using NAPS2.Scan;

namespace TownSuite.TwainScanner
{
    internal class NewScannerList
    {
        public Driver driver;

        public NewScannerList(Driver driver)
        {
            this.driver = driver;
        }

        public NewScannerList(string driver) : this(GetDriver(driver)) { }

        public static Driver GetDriver(string backend)
        {
            return backend switch
            {
                "twain" => Driver.Twain,
                "wia"   => Driver.Wia,
                "sane"  => Driver.Sane,
                "apple" => Driver.Apple,
                "escl"  => Driver.Escl,
                _       => DefaultDriver()
            };
        }

        public static Driver DefaultDriver()
        {
            if (OperatingSystem.IsWindows()) return Driver.Twain;
            return Driver.Escl; // macOS and Linux: use eSCL (AirScan); no platform-specific TFM needed
        }

        /// <summary>Returns the drivers appropriate for the current platform.</summary>
        public static Driver[] PlatformDrivers()
        {
            if (OperatingSystem.IsWindows()) return new[] { Driver.Escl, Driver.Twain, Driver.Wia  };
            if (OperatingSystem.IsMacOS())   return new[] { Driver.Escl };       // AirScan; no -macos TFM needed
            return new[] {  Driver.Escl, Driver.Sane};                            // Linux: SANE + AirScan
        }

        public async Task<IEnumerable<string>> ScanList(ScanningContext scanningContext)
        {
            try
            {
                var list = await GetList(scanningContext);
                return list.Select(s => s.Name);
            }
            catch (Exception ex) when (ex.GetType().Name == "TwainException")
            {
                if (ex.Message == "It worked!")
                    Console.Error.WriteLine("Failed to find a scanner");
                return Array.Empty<string>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{ex.Message}\n\n{ex.StackTrace}");
                return Array.Empty<string>();
            }
        }

        public async Task<List<ScanDevice>> GetList(ScanningContext scanningContext)
        {
            var controller = GetScanController(scanningContext);
            var scanOptions = new ScanOptions { Driver = driver };
            return await controller.GetDeviceList(scanOptions);
        }

        public ScanController GetScanController(ScanningContext scanningContext)
        {
            if (OperatingSystem.IsWindows() && driver == Driver.Twain)
                scanningContext.SetUpWin32Worker();
            return new ScanController(scanningContext);
        }

        public ScanningContext GetScanContext() =>
            new ScanningContext(new ImageSharpImageContext());
    }
}
