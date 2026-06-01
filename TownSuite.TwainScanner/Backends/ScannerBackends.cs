using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Avalonia.Threading;
using System.Threading.Tasks;

namespace TownSuite.TwainScanner.Backends
{
    internal class ScannerBackends : IDisposable
    {
        protected string imageFormat;
        protected readonly string DirText;
        protected string FileExtention;
        protected string imageExtension;
        protected int picnumber = 0;
        private readonly Ocr ocr;

        public IMainView ParentView { get; set; }

        public ScannerBackends(string dirText, Ocr ocr)
        {
            this.DirText = dirText;
            this.ocr = ocr;
        }

        public virtual Task ConfigureSettings()
        {
            if (!Directory.Exists(DirText))
                Directory.CreateDirectory(DirText);
            return Task.CompletedTask;
        }

        public virtual void ChangeSelectedScanner(object value) { }

        public virtual Task Scan(string imageFormat)
        {
            this.imageFormat = imageFormat.ToLower().Trim();
            switch (this.imageFormat)
            {
                case "tiff":
                    FileExtention = ".tif";
                    imageExtension = ".tif";
                    break;
                case "png":
                    FileExtention = ".png";
                    imageExtension = ".png";
                    break;
                case "pdf":
                    FileExtention = ".pdf";
                    imageExtension = ".jpeg";
                    break;
                case "jpeg":
                    FileExtention = ".jpeg";
                    imageExtension = ".jpeg";
                    break;
            }
            return Task.CompletedTask;
        }

        public virtual void Save()
        {
            switch (imageFormat)
            {
                case "tiff": Save_TIFF(); break;
                case "png":  SavePNG();   break;
                case "jpeg": SaveJPEG();  break;
                case "pdf":  SavePDF();   break;
            }

            Console.WriteLine();
            Console.WriteLine(" " + FileExtention + " ");
            Console.WriteLine();
            Console.WriteLine("SAVED");
            Console.Out.Flush();
        }

        protected string PadNumbers(string input)
        {
            string smallName = Path.GetFileNameWithoutExtension(input);
            return Regex.Replace(smallName, "[0-9]+", match => match.Value.PadLeft(10, '0'));
        }

        private void Save_TIFF()
        {
            string[] sa = Directory.GetFiles(DirText, "tmpscan*.tif");
            sa = new List<string>(sa).OrderBy(p => PadNumbers(p)).ToArray();

            ImageCodecInfo info = ImageCodecInfo.GetImageEncoders().FirstOrDefault(p => p.MimeType == "image/tiff");
            var enc = Encoder.SaveFlag;
            var ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(enc, Convert.ToInt64(EncoderValue.MultiFrame));

            Bitmap pages = null;
            int frame = 0;

            foreach (string s in sa)
            {
                if (frame == 0)
                {
                    pages = (Bitmap)System.Drawing.Image.FromFile(s);
                    pages.Save(Path.Combine(DirText, "tmpScan.tif"), info, ep);
                }
                else
                {
                    ep.Param[0] = new EncoderParameter(enc, Convert.ToInt64(EncoderValue.FrameDimensionPage));
                    using (Bitmap bm = (Bitmap)System.Drawing.Image.FromFile(s))
                        pages.SaveAdd(bm, ep);
                }
                if (frame == sa.Length - 1)
                {
                    ep.Param[0] = new EncoderParameter(enc, Convert.ToInt64(EncoderValue.Flush));
                    pages.SaveAdd(ep);
                }
                frame++;
            }
        }

        private void SavePNG()
        {
            string[] inputFiles = Directory.GetFiles(DirText, "tmpscan*.png");
            System.Drawing.Image[] images = new System.Drawing.Image[inputFiles.Length];
            int height = 0, width = 0;

            try
            {
                for (int i = 0; i < inputFiles.Length; i++)
                {
                    images[i] = System.Drawing.Image.FromFile(inputFiles[i]);
                    height = Math.Max(height, images[i].Height);
                    width += images[i].Width;
                }

                var image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(image))
                {
                    g.Clear(System.Drawing.Color.Transparent);
                    width = 0;
                    for (int i = 0; i < images.Length; i++)
                    {
                        double scale = (double)height / images[i].Height;
                        int scaledWidth = (int)(images[i].Width * scale);
                        g.DrawImage(images[i],
                            new System.Drawing.Rectangle(width, 0, scaledWidth, height),
                            new System.Drawing.Rectangle(0, 0, images[i].Width, images[i].Height),
                            GraphicsUnit.Pixel);
                        width += scaledWidth;
                        images[i].Dispose();
                    }
                }
                image.Save(Path.Combine(DirText, "tmpScan.png"));
            }
            finally
            {
                for (int i = 0; i < inputFiles.Length; i++)
                    images[i]?.Dispose();
            }
        }

        private void SavePDF()
        {
            var predefinedSizes = new[]
            {
                new { Size = iTextSharp.text.PageSize.A4,   Width = 21f,    Height = 29.7f  },
                new { Size = iTextSharp.text.PageSize.A3,   Width = 29.7f,  Height = 42f    },
                new { Size = new iTextSharp.text.Rectangle(21.59f * 28.35f, 27.94f * 28.35f), Width = 21.59f, Height = 27.94f },
                new { Size = new iTextSharp.text.Rectangle(21.59f * 28.35f, 35.56f * 28.35f), Width = 21.59f, Height = 35.56f },
                new { Size = new iTextSharp.text.Rectangle(15.24f * 28.35f,  6.99f * 28.35f), Width = 15.24f, Height =  6.99f },
                new { Size = new iTextSharp.text.Rectangle( 8f    * 28.35f, 30f    * 28.35f), Width =  8f,    Height = 30f    },
            };

            string[] sa = Directory.GetFiles(DirText, "tmpscan*.jpeg");
            sa = new List<string>(sa).OrderBy(p => PadNumbers(p)).ToArray();
            string outputPdfPath = Path.Combine(DirText, "tmpscan.pdf");

            using var fs = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write, FileShare.None);
            using var doc = new Document(iTextSharp.text.PageSize.A4);
            using var writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            foreach (string imagePath in sa)
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                using var ms = new MemoryStream(imageBytes);
                using var sysImage = System.Drawing.Image.FromStream(ms);

                float dpiX = sysImage.HorizontalResolution;
                float dpiY = sysImage.VerticalResolution;
                float widthInches = sysImage.Width / dpiX;
                float heightInches = sysImage.Height / dpiY;
                float imageWidthCm = widthInches * 2.54f;
                float imageHeightCm = heightInches * 2.54f;

                bool isLandscape = imageWidthCm > imageHeightCm;
                float actualWidth  = isLandscape ? imageHeightCm : imageWidthCm;
                float actualHeight = isLandscape ? imageWidthCm  : imageHeightCm;

                float bestDiff = float.MaxValue;
                var bestPaper = predefinedSizes[0];
                foreach (var paper in predefinedSizes)
                {
                    float pw = paper.Width, ph = paper.Height;
                    if (isLandscape && pw < ph) { float t = pw; pw = ph; ph = t; }
                    float diff = Math.Abs(actualWidth - pw) + Math.Abs(actualHeight - ph);
                    if (diff < bestDiff) { bestDiff = diff; bestPaper = paper; }
                }

                doc.SetPageSize(bestPaper.Size);
                doc.NewPage();

                iTextSharp.text.Image pdfImage = iTextSharp.text.Image.GetInstance(imageBytes);
                pdfImage.ScaleToFit(bestPaper.Size.Width, bestPaper.Size.Height);
                pdfImage.SetAbsolutePosition(0, 0);
                doc.Add(pdfImage);
            }

            doc.Close();
        }

        private void SaveJPEG()
        {
            string[] inputFiles = Directory.GetFiles(DirText, "tmpscan*.jpeg");
            System.Drawing.Image[] images = new System.Drawing.Image[inputFiles.Length];
            int height = 0, width = 0;

            try
            {
                for (int i = 0; i < inputFiles.Length; i++)
                {
                    images[i] = System.Drawing.Image.FromFile(inputFiles[i]);
                    height = Math.Max(height, images[i].Height);
                    width += images[i].Width;
                }

                var image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(image))
                {
                    g.Clear(System.Drawing.Color.Transparent);
                    width = 0;
                    for (int i = 0; i < images.Length; i++)
                    {
                        double scale = (double)height / images[i].Height;
                        int scaledWidth = (int)(images[i].Width * scale);
                        g.DrawImage(images[i],
                            new System.Drawing.Rectangle(width, 0, scaledWidth, height),
                            new System.Drawing.Rectangle(0, 0, images[i].Width, images[i].Height),
                            GraphicsUnit.Pixel);
                        width += scaledWidth;
                        images[i].Dispose();
                    }
                }
                image.Save(Path.Combine(DirText, "tmpScan.jpeg"));
            }
            finally
            {
                for (int i = 0; i < inputFiles.Length; i++)
                    images[i]?.Dispose();
            }
        }

        public void DeleteFiles()
        {
            if (string.Equals(DirText, Path.Combine(Environment.GetEnvironmentVariable("TMP") ?? "", "TownSuite", "TwainScanner"),
                StringComparison.InvariantCultureIgnoreCase))
            {
                LoopFiles("*.bmp");
                LoopFiles("*.jpeg");
                LoopFiles("*.tif");
                LoopFiles("*.png");
                LoopFiles("*.pdf");
                LoopFiles("*.txt");
            }
            else
            {
                LoopFiles("tmpscan*.bmp");
                LoopFiles("tmpscan*.jpeg");
                LoopFiles("tmpscan*.tif");
                LoopFiles("tmpscan*.png");
                LoopFiles("tmpscan*.pdf");
                LoopFiles("tmpscan*.txt");
            }
        }

        private void LoopFiles(string fileext)
        {
            foreach (string s in Directory.GetFiles(DirText, fileext))
                File.Delete(s);
        }

        protected void RunOcr(string origPath, bool ocrCheckboxChecked)
        {
            if (!ocr.Enabled || !ocrCheckboxChecked) return;

            ocr.GetText(origPath,
                onOcrStarting: () => Dispatcher.UIThread.Post(() =>
                    ParentView?.SetOcrStatus(true, I18N.GetString("OcrProcessing"))),
                onOcrComplete: (fp, text) => Dispatcher.UIThread.Post(() =>
                {
                    ParentView?.MarkOcrComplete(fp, text);
                    ParentView?.SetOcrStatus(false);
                }),
                onOcrError: (fp, ex) => Dispatcher.UIThread.Post(() =>
                {
                    _ = ParentView?.ShowErrorAsync(ex.Message, I18N.GetString("OcrError"));
                    ParentView?.SetOcrStatus(false);
                })
            );
        }

        protected bool OcrEnabled() => ocr.Enabled;

        public virtual string GetBackendType() => string.Empty;

        public virtual void Dispose() { }
    }
}
