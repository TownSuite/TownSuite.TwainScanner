using Avalonia.Media.Imaging;
using NAPS2.Images;
using NAPS2.Images.Gdi;
using NAPS2.Scan;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace TownSuite.TwainScanner.Backends
{
    class Naps2Backend : ScannerBackends
    {
        private ScanningContext scanningContext;
        private ScanController controller;
        private readonly Driver driver;
        private readonly NewScannerList newScannerList;

        public Naps2Backend(string dirText, Ocr ocr, Driver driver) : base(dirText, ocr)
        {
            this.driver = driver;
            this.newScannerList = new NewScannerList(driver);
            scanningContext = newScannerList.GetScanContext();
            controller = newScannerList.GetScanController(scanningContext);
        }

        public override async Task ConfigureSettings()
        {
            await base.ConfigureSettings();

            var devices = await new NewScannerList(driver).GetList(scanningContext);
            ParentView.SetDeviceList(devices, append: true);
        }

        public override async Task Scan(string imageFormat)
        {
            ParentView.SetScanningStatus(true, "Scanning...");

            try
            {
                await base.Scan(imageFormat);

                var resolution = ParentView.GetResolutionDpi();
                var device = ParentView.GetSelectedDevice();

                var options = new ScanOptions()
                {
                    PaperSource = NAPS2.Scan.PaperSource.Auto,
                    PageSize = PageSize.Letter,
                    Dpi = resolution,
                    Driver = driver,
                    Device = device,
                    BitDepth = BitDepth.Color
                };

                await foreach (var image in controller.Scan(options))
                {
                    picnumber += 1;
                    string origPath = Path.Combine(DirText,
                        "tmpScan" + picnumber + "_" + picnumber + Guid.NewGuid().ToString() + imageExtension);

                    using (var img = image.RenderToBitmap())
                    {
                        switch (imageFormat)
                        {
                            case "tiff": img.Save(origPath, ImageFormat.Tiff); break;
                            case "png":  img.Save(origPath, ImageFormat.Png);  break;
                            default:     img.Save(origPath, ImageFormat.Jpeg); break;
                        }

                        Bitmap avaloniaThumbnail;
                        using (var thumb = new System.Drawing.Bitmap(img, new System.Drawing.Size(180, 180)))
                        using (var ms = new MemoryStream())
                        {
                            thumb.Save(ms, ImageFormat.Png);
                            ms.Position = 0;
                            avaloniaThumbnail = new Bitmap(ms);
                        }

                        ParentView.AddThumbnail(origPath, avaloniaThumbnail);
                        RunOcr(origPath, ParentView.IsOcrChecked);

                        Console.WriteLine("Scanned a page!");
                        picnumber += 1;
                    }
                }
            }
            finally
            {
                ParentView.SetScanningStatus(false);
            }
        }

        public override void Save() => base.Save();

        public override string GetBackendType() => driver.ToString();

        public override void Dispose()
        {
            base.Dispose();
            scanningContext?.Dispose();
        }
    }
}
