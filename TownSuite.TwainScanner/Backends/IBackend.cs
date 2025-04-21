using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text;
using NAPS2.Images;
using PdfSharpCore.Drawing;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace TownSuite.TwainScanner.Backends
{
    internal class IBackend : IDisposable
    {
        protected string imageFormat;
        protected readonly string DirText;
        protected string FileExtention;
        protected string imageExtension;
        readonly Ocr ocr;
        public MainFrame ParentForm
        {
            get; set;
        }

        public IBackend(string dirText, Ocr ocr)
        {
            this.DirText = dirText;
            this.ocr = ocr;
        }

        public virtual Task ConfigureSettings()
        {
            if (!System.IO.Directory.Exists(DirText))
            {
                System.IO.Directory.CreateDirectory(DirText);
            }

            return Task.CompletedTask;
        }

        public virtual void ChangeSelectedScanner(object value)
        {

        }

        public virtual void HandleErrors()
        {

        }

        public virtual Task Scan(string imageFormat)
        {
            this.imageFormat = imageFormat.ToLower().Trim();
            switch (imageFormat)
            {
                case "tiff":
                    FileExtention = ".tif";
                    imageExtension = ".tif";
                    break;
                case "png":
                    // PDFs will internally be png images
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
                case "tiff":
                    //Save tiff
                    Save_TIFF();
                    break;
                case "png":
                    SavePNG();
                    break;
                case "jpeg":
                    SaveJPEG();
                    break;
                case "pdf":
                    //Save pdf
                    SavePDF();
                    break;
            }

            Console.WriteLine();
            Console.WriteLine(" " + FileExtention + " ");
            Console.WriteLine();
            Console.WriteLine("SAVED");
            Console.Out.Flush();
        }

        protected string PadNumbers(string input)
        {
            string smallName = System.IO.Path.GetFileNameWithoutExtension(input);
            // Default ordering for a string is standard alpha numeric dictionary ordering.
            // when individual files like tmpScan1_0.bmp , tmpScan2_1.bmp .....tmpScan10_9.bmp, tmpScan11_10
            //Then tmpScan1_0.bmp will be first and order the files and tmpScan10_9.bmp, tmpScan11_10 after
            return Regex.Replace(smallName, "[0-9]+", match => match.Value.PadLeft(10, '0'));
        }

        private void Save_TIFF()
        {
            string[] sa = null;
            sa = Directory.GetFiles(DirText, "tmpscan*.tif");

            List<string> UnSortList = new List<string>(sa);
            List<string> SortedList = UnSortList.OrderBy(p => PadNumbers(p)).ToList();
            sa = SortedList.ToArray();

            //get the codec for tiff files
            ImageCodecInfo info = ImageCodecInfo.GetImageEncoders().Where(p => p.MimeType == "image/tiff").FirstOrDefault();

            //use the save encoder
            var enc = System.Drawing.Imaging.Encoder.SaveFlag;
            EncoderParameters ep = new EncoderParameters(1);

            ep.Param[0] = new EncoderParameter(enc, Convert.ToInt64(EncoderValue.MultiFrame));

            Bitmap pages = null;
            int frame = 0;

            foreach (string s in sa)
            {
                if (frame == 0)
                {
                    pages = (Bitmap)System.Drawing.Image.FromFile(s);

                    //save the first frame
                    pages.Save(Path.Combine(DirText, "tmpScan.tif"), info, ep);
                }
                else
                {
                    //save the intermediate frames
                    ep.Param[0] = new EncoderParameter(enc, Convert.ToInt64(EncoderValue.FrameDimensionPage));
                    using (Bitmap bm = (Bitmap)System.Drawing.Image.FromFile(s))
                    {
                        pages.SaveAdd(bm, ep);
                    }
                }
                if (frame == sa.Length - 1)
                {
                    //flush and close.
                    ep.Param[0] = new EncoderParameter(enc, Convert.ToInt64(EncoderValue.Flush));
                    pages.SaveAdd(ep);

                }
                frame += 1;

            }
        }

        private string SavePNG()
        {
            string[] inputFiles = Directory.GetFiles(DirText, "tmpscan*.png");

            // TODO: You may want to add input checking here.
            System.Drawing.Image[] images = new System.Drawing.Image[inputFiles.Length];
            System.Drawing.Image image;
            int height = 0, width = 0;

            try
            {
                for (int i = 0; i < inputFiles.Length; ++i)
                {
                    images[i] = image = System.Drawing.Image.FromFile(inputFiles[i]);
                    height = Math.Max(height, image.Height);
                    width += image.Width;
                }

                image = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                using (Graphics g = Graphics.FromImage(image))
                {
                    // Clear the background to transparent.
                    g.Clear(System.Drawing.Color.Transparent);

                    width = 0;

                    for (int i = 0; i < images.Length; ++i)
                    {
                        double scale = (double)height / images[i].Height;
                        int scaledWidth = (int)(images[i].Width * scale);
                        // Draw the image scaled to match the maxHeight
                        g.DrawImage(images[i],
                                    new System.Drawing.Rectangle(width, 0, scaledWidth, height),
                                    new System.Drawing.Rectangle(0, 0, images[i].Width, images[i].Height),
                                    GraphicsUnit.Pixel);
                        width += scaledWidth;

                        // free memory immediately to avoid out of memory exception as 'image' grows.
                        if (images[i] != null)
                            images[i].Dispose();
                    }
                }

                // You don't need to save this in order to use the in-memory object.
                // img3.Save(finalImage, System.Drawing.Imaging.ImageFormat.Jpeg);

                image.Save(Path.Combine(DirText, "tmpScan.png"));
                //imageLocation.Image = image;
            }
            finally
            {
                // in case of exception dispose everything properly
                for (int i = 0; i < inputFiles.Length; ++i)
                    images[i]?.Dispose();
            }

            return "";
        }

        private void SavePDF()
        {
            var predefinedSizes = new[]
        {
            new { Size = iTextSharp.text.PageSize.A4, Width = 21f, Height = 29.7f },
            new { Size = iTextSharp.text.PageSize.A3, Width = 29.7f, Height = 42f },
            new { Size = new iTextSharp.text.Rectangle(21.59f * 28.35f, 27.94f * 28.35f), Width = 21.59f, Height = 27.94f },  // Letter (8.5" x 11")
            new { Size = new iTextSharp.text.Rectangle(21.59f * 28.35f, 35.56f * 28.35f), Width = 21.59f, Height = 35.56f },  // Legal (8.5" x 14")
            new { Size = new iTextSharp.text.Rectangle(15.24f * 28.35f, 6.99f * 28.35f), Width = 15.24f, Height = 6.99f },  // Cheque: 6" x 2.75"
            new { Size = new  iTextSharp.text.Rectangle(8f * 28.35f, 30f * 28.35f), Width = 8f, Height = 30f }  // Receipt: 80mm x 300mm (approx)
        };

            string[] sa = Directory.GetFiles(DirText, "tmpscan*.jpeg");

            List<string> UnSortList = new List<string>(sa);
            List<string> SortedList = UnSortList.OrderBy(p => PadNumbers(p)).ToList();
            sa = SortedList.ToArray();
            string outputPdfPath = Path.Combine(DirText, "tmpscan.pdf");

            using (FileStream fs = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (Document doc = new Document(iTextSharp.text.PageSize.A4)) // Default to A4, will resize per page
            using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
            {
                doc.Open();

                foreach (string imagePath in sa)
                {
                    byte[] imageBytes = File.ReadAllBytes(imagePath);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    using (System.Drawing.Image sysImage = System.Drawing.Image.FromStream(ms))
                    {
                        float dpiX = sysImage.HorizontalResolution;
                        float dpiY = sysImage.VerticalResolution;
                        float widthInches = sysImage.Width / dpiX;
                        float heightInches = sysImage.Height / dpiY;
                        float imageWidthCm = widthInches * 2.54f;
                        float imageHeightCm = heightInches * 2.54f;

                        bool isLandscape = imageWidthCm > imageHeightCm;
                        float actualWidth = imageWidthCm;
                        float actualHeight = imageHeightCm;
                        if (isLandscape)
                        {
                            actualWidth = imageHeightCm;
                            actualHeight = imageWidthCm;
                        }

                        // Choose the best predefined paper size
                        float bestDiff = float.MaxValue;
                        var bestPaper = predefinedSizes[0];

                        foreach (var paper in predefinedSizes)
                        {
                            float paperWidth = paper.Width;
                            float paperHeight = paper.Height;

                            if (isLandscape && paperWidth < paperHeight)
                            {
                                float temp = paperWidth;
                                paperWidth = paperHeight;
                                paperHeight = temp;
                            }

                            float diff = Math.Abs(actualWidth - paperWidth) + Math.Abs(actualHeight - paperHeight);
                            if (diff < bestDiff)
                            {
                                bestDiff = diff;
                                bestPaper = paper;
                            }
                        }

                        // Create a new page for each image
                        doc.SetPageSize(bestPaper.Size);
                        doc.NewPage();

                        // Add image to the PDF
                        iTextSharp.text.Image pdfImage = iTextSharp.text.Image.GetInstance(imageBytes);
                        pdfImage.ScaleToFit(bestPaper.Size.Width, bestPaper.Size.Height);
                        pdfImage.SetAbsolutePosition(0, 0);
                        doc.Add(pdfImage);
                    }
                }

                doc.Close();
            }

        }

        private string SaveJPEG()
        {
            string[] inputFiles = Directory.GetFiles(DirText, "tmpscan*.jpeg");

            // TODO: You may want to add input checking here.
            System.Drawing.Image[] images = new System.Drawing.Image[inputFiles.Length];
            System.Drawing.Image image;
            int height = 0, width = 0;

            try
            {
                for (int i = 0; i < inputFiles.Length; ++i)
                {
                    images[i] = image = System.Drawing.Image.FromFile(inputFiles[i]);
                    height = Math.Max(height, image.Height);
                    width += image.Width;
                }

                image = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                using (Graphics g = Graphics.FromImage(image))
                {
                    // Clear the background to transparent.
                    g.Clear(System.Drawing.Color.Transparent);

                    width = 0;

                    for (int i = 0; i < images.Length; ++i)
                    {
                        double scale = (double)height / images[i].Height;
                        int scaledWidth = (int)(images[i].Width * scale);
                        // Draw the image scaled to match the maxHeight
                        g.DrawImage(images[i],
                                    new System.Drawing.Rectangle(width, 0, scaledWidth, height),
                                    new System.Drawing.Rectangle(0, 0, images[i].Width, images[i].Height),
                                    GraphicsUnit.Pixel);
                        width += scaledWidth;

                        // free memory immediately to avoid out of memory exception as 'image' grows.
                        if (images[i] != null)
                            images[i].Dispose();
                    }
                }
                image.Save(Path.Combine(DirText, "tmpScan.jpeg"));
                //imageLocation.Image = image;
            }
            finally
            {
                // in case of exception dispose everything properly
                for (int i = 0; i < inputFiles.Length; ++i)
                    if (images[i] != null)
                        images[i].Dispose();
            }

            return "";
        }

        public void DeleteFiles()
        {

            if (string.Equals(DirText, System.IO.Path.Combine(Environment.GetEnvironmentVariable("TMP"), "TownSuite", "TwainScanner"),
                StringComparison.InvariantCultureIgnoreCase))
            {
                // using a custom folder.   Delete everything.
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
            //New Stuff
            //---------------------------
            string[] sa = Directory.GetFiles(DirText, fileext);

            foreach (string s_loopVariable in sa)
            {
                string s = s_loopVariable;
                File.Delete(s);
            }
        }

        protected void RunOcr(PictureBox newpic, string origPath, bool ocrCheckboxChecked)
        {
            if (ocr.Enabled && ocrCheckboxChecked)
            {
                ocr.GetText(origPath, newpic, ParentForm.GetProgressBar(), ParentForm.GetStatusLabel());
            }
        }

        protected bool OcrEnabled() { return ocr.Enabled; }
        public virtual void EnableOcr(bool enable)
        {
            ocr.Enabled = enable;
        }

        protected void Newpic_MouseLeave(object sender, EventArgs e)
        {
            if (sender is PictureBox == false)
            {
                return;
            }
            var pb = sender as PictureBox;
            pb.BorderStyle = BorderStyle.None;
        }

        protected void Newpic_MouseEnter(object sender, EventArgs e)
        {
            if (sender is PictureBox == false)
            {
                return;
            }
            var pb = sender as PictureBox;
            pb.BorderStyle = BorderStyle.FixedSingle;
        }

        protected void Newpic_DoubleClick(object sender, EventArgs e)
        {
            if (sender is PictureBox == false)
            {
                return;
            }
            var pb = sender as PictureBox;
            if (System.IO.File.Exists(pb.Tag?.ToString() ?? ""))
            {
                System.Diagnostics.Process.Start(pb.Tag.ToString());
            }
        }

        public virtual void Dispose()
        {
            
        }
    }
}