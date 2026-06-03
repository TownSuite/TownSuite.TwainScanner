using Avalonia.Media.Imaging;
using NAPS2.Images;
using NAPS2.Images.Transforms;
using NAPS2.Scan;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TownSuite.TwainScanner.Backends
{
    internal class Naps2Backend : ScannerBackends
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
            ParentView.SetScanningStatus(true, I18N.GetString("Scanning"));

            try
            {
                await base.Scan(imageFormat);

                var resolution = ParentView.GetResolutionDpi();
                var device = ParentView.GetSelectedDevice();

                var options = new ScanOptions
                {
                    PaperSource = PaperSource.Auto,
                    PageSize    = PageSize.Letter,
                    Dpi         = resolution,
                    Driver      = driver,
                    Device      = device,
                    BitDepth    = BitDepth.Color
                };

                await foreach (var scanned in controller.Scan(options))
                {
                    picnumber++;
                    string origPath = Path.Combine(DirText,
                        $"tmpScan{picnumber}_{picnumber}{Guid.NewGuid()}{imageExtension}");

                    using var memImage = scanningContext.ImageContext.Render(scanned);

                    // Save full-resolution file
                    var fmt = imageExtension switch
                    {
                        ".tif"  => ImageFileFormat.Tiff,
                        ".png"  => ImageFileFormat.Png,
                        _       => ImageFileFormat.Jpeg
                    };
                    memImage.Save(origPath, fmt, new ImageSaveOptions());

                    // Build 180-pixel thumbnail for display
                    Bitmap avaloniaThumbnail;
                    using (var ms = new MemoryStream())
                    {
                        using var thumb = scanningContext.ImageContext.PerformTransform(
                            memImage.Clone(), new ThumbnailTransform(180));
                        thumb.Save(ms, ImageFileFormat.Png, new ImageSaveOptions());
                        ms.Position = 0;
                        avaloniaThumbnail = new Bitmap(ms);
                    }

                    ParentView.AddThumbnail(origPath, avaloniaThumbnail);
                    RunOcr(origPath, ParentView.IsOcrChecked);

                    Console.WriteLine("Scanned a page!");
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
