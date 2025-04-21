#if NET8_0_OR_GREATER
using NAPS2.Images;
using NAPS2.Images.Gdi;
using NAPS2.Pdf;
using NAPS2.Scan;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace TownSuite.TwainScanner.Backends
{
    class Naps2Backend : IBackend
    {
        ScanningContext scanningContext;
        ScanController controller;
        public Naps2Backend(string dirText, Ocr ocr) : base(dirText, ocr)
        {
            // Constructor logic here
        }

        public override async Task ConfigureSettings()
        {
            await base.ConfigureSettings();
            // Initialize NAPS2 scanning context and other settings here

            scanningContext = new ScanningContext(new GdiImageContext());
            scanningContext.SetUpWin32Worker();

            controller = new ScanController(scanningContext);

            var scanOptions = new ScanOptions()
            {
                PaperSource = PaperSource.Auto,
                PageSize = PageSize.Letter,
                Dpi = 300,
                UseNativeUI = true,
                Driver = Driver.Twain,

            };
            var devices = (await controller.GetDeviceList(scanOptions));

            var sourceTwainListBox = ParentForm.GetTwainSourceList();
            sourceTwainListBox.Items.Clear();

            foreach (var device in devices)
            {
                var options = new ScanOptions { Device = device };

                sourceTwainListBox.Items.Add(options);
            }
            sourceTwainListBox.SelectedIndex = 0;
        }

        int picnumber = 0;
        public override async Task Scan(string imageFormat)
        {
            await base.Scan(imageFormat);

            // Scanning with TWAIN on Windows must happen from a 32-bit process. You can do this by building your exe as
            // 32-bit, but the better solution is to install the NAPS2.Sdk.Worker.Win32 Nuget package, which includes a
            // pre-compiled 32-bit NAPS2.Worker.exe. Then you only need to call ScanningContext.SetUpWin32Worker and you
            // should be able to scan with TWAIN.
            //
            // If you want to use a worker process but don't want to use a pre-compiled exe (or want to set up your own
            // logging etc), you can also build your own worker exe with the same name (and call WorkerServer.Run in its
            // Main method).


            // Set up the worker; this includes starting a worker process in the background so it will be ready to respond
            // when we need it


            // As we're not using the default (WIA) driver, we need to specify it when listing devices or scanning
            ScanDevice device = (await controller.GetDeviceList(Driver.Twain)).First();
            var options = new ScanOptions { Device = device };

            await foreach (var image in controller.Scan(options))
            {
                picnumber += 1;
                var newpic = new PictureBox();
                System.Drawing.Image resizedImg;
                string origPath = Path.Combine(DirText, "tmpScan" + picnumber.ToString() + "_" + picnumber.ToString() + Guid.NewGuid().ToString() + imageExtension);
                newpic.Tag = origPath;

                using Bitmap img = image.RenderToBitmap();


                switch (imageFormat)
                {
                    case "tiff":
                        img.Save(origPath, ImageFormat.Tiff);
                        break;
                    case "png":
                        img.Save(origPath, ImageFormat.Png);
                        break;
                    case "pdf":
                    case "jpeg":
                    default:
                        // pdf is just an import of a file.  Use jpg.
                        img.Save(origPath, ImageFormat.Jpeg);
                        break;
                }

                resizedImg = new Bitmap(img, new Size(180, 180));


                newpic.Image = resizedImg;
                newpic.Size = new Size(newpic.Image.Width, newpic.Image.Height);
                newpic.Refresh();
                newpic.DoubleClick += Newpic_DoubleClick;
                newpic.MouseEnter += Newpic_MouseEnter;
                newpic.MouseLeave += Newpic_MouseLeave;
                var flowLayoutPanel1 = ParentForm.GetFlowLayoutPanel();
                newpic.Text = "ScanPass" + picnumber.ToString() + "_Pic" + picnumber.ToString();
                flowLayoutPanel1.Controls.Add(newpic);
                // newpic.doTmpSave(DirText + "\\tmpScan" + picnumber.ToString() + "_" + i.ToString() + ".bmp");

                RunOcr(newpic, origPath, OcrEnabled());

                Console.WriteLine("Scanned a page!");
                picnumber += 1;
            }
        }
        public override void Save()
        {
            base.Save();
        }

        public override void Dispose()
        {
            base.Dispose();
            this.scanningContext?.Dispose();
        }

        public async Task Init()
        {
            //// Set up
            //using (var scanningContext = new ScanningContext(new GdiImageContext()))
            //{
            //    var controller = new ScanController(scanningContext);

            //    // Query for available scanning devices
            //    var devices = await controller.GetDeviceList();

            //    // Set scanning options
            //    var options = new ScanOptions
            //    {
            //        Device = devices.First(),
            //        PaperSource = PaperSource.Feeder,
            //        PageSize = PageSize.A4,
            //        Dpi = 300
            //    };

            //    // Scan and save images
            //    int i = 1;
            //    await foreach (var image in controller.Scan(options))
            //    {
            //        image.Save($"page{i++}.jpg");
            //    }

            //    // Scan and save PDF
            //    var images = await controller.Scan(options).ToListAsync();
            //    var pdfExporter = new PdfExporter(scanningContext);
            //    await pdfExporter.Export("doc.pdf", images);
            //}
        }
    }
}

#endif