using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTwain.Data;
using NTwain;
using TownSuite.TwainScanner.Backends;
using NAPS2.Scan;
using NAPS2.Images.Gdi;

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
      
        public static NAPS2.Scan.Driver GetDriver(string backend)
        {
            switch (backend)
            {
                case "twain":
                    //return new OriginalTwainBackend(dirText, ocr)
                    //{
                    //    ParentForm = this
                    //};

                    return NAPS2.Scan.Driver.Twain;

                case "wia":
                    return NAPS2.Scan.Driver.Wia;

                default:
                    return NAPS2.Scan.Driver.Twain;

            }
        }
        //private Twain32 _twain;
        public async Task<IEnumerable<string>> ScanList(ScanningContext scanningContext)
        {
            List<string> lstscan = new List<string>();
            try
            {
               var list = await GetList(scanningContext);
                return list.Select(s=>s.Name);
            }
            catch (TwainException ex)
            {
                if (ex.Message == "It worked!")
                {
                    Console.Error.WriteLine("Failed to find a scanner");
                    Console.Out.Flush();
                    Console.Error.Flush();
                }
                return lstscan;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace), "TownSuite Scanner Load Error");

                return lstscan;
            }

        }
        public async Task<List<ScanDevice>> GetList(ScanningContext scanningContext)
        {
   
            var controller = GetScanController(scanningContext);

            var scanOptions = new ScanOptions()
            {
                Driver = driver
            };
            var devices = (await controller.GetDeviceList(scanOptions));

            return devices;
        }

        public ScanController GetScanController(ScanningContext scanningContext)
        {
            if (driver == Driver.Twain)
            {
                scanningContext.SetUpWin32Worker();
            }

            return new ScanController(scanningContext);

        }

      
        public  ScanningContext GetScanContext()
        {
            return new ScanningContext(new GdiImageContext());
        }
    }

}
