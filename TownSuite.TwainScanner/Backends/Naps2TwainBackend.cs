﻿#if NET8_0_OR_GREATER
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
    class Naps2Backend : ScannerBackends
    {
        ScanningContext scanningContext;
        ScanController controller;
        Driver driver;
        public Naps2Backend(string dirText, Ocr ocr, Driver driver) : base(dirText, ocr)
        {
            this.driver = driver;
        }

        public override async Task ConfigureSettings()
        {
            await base.ConfigureSettings();
            // Initialize NAPS2 scanning context and other settings here

            scanningContext = new ScanningContext(new GdiImageContext());
            if (driver == Driver.Twain)
            {
                scanningContext.SetUpWin32Worker();
            }

            controller = new ScanController(scanningContext);

            var scanOptions = new ScanOptions()
            {
                PaperSource = PaperSource.Auto,
                PageSize = PageSize.Letter,
                Dpi = 300,
                UseNativeUI = true,
                Driver = driver
            };
            var devices = (await controller.GetDeviceList(scanOptions));

            ListBox sourceListBox;
            if (driver == Driver.Twain)
            {
                sourceListBox = ParentForm.GetTwainSourceList();
            }
            else
            {
                sourceListBox = ParentForm.GetWiaSourceList();
            }

            sourceListBox.DataSource = devices;
            sourceListBox.DisplayMember = "Name";
            sourceListBox.SelectedIndex = 0;
        }

        int picnumber = 0;
        public override async Task Scan(string imageFormat)
        {
            var toolStrip = ParentForm.GetProgressBar();
            var statusLabel = ParentForm.GetStatusLabel();

            toolStrip.Style = ProgressBarStyle.Marquee;
            toolStrip.Visible = true;
            statusLabel.Text = "Scanning...";
            statusLabel.Visible = true;

            try
            {

                await base.Scan(imageFormat);

                ListBox sourceListBox;
                if (driver == Driver.Twain)
                {
                    sourceListBox = ParentForm.GetTwainSourceList();
                }
                else
                {
                    sourceListBox = ParentForm.GetWiaSourceList();
                }
                var device = sourceListBox.SelectedItem as ScanDevice;
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
            finally
            {
                toolStrip.Style = ProgressBarStyle.Blocks;
                toolStrip.Visible = false;
                statusLabel.Visible = false;
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
    }
}

#endif