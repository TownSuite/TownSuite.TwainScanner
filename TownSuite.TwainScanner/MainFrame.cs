﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using WIA;
using System.Collections;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.Resources;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.Export;
using Telerik.Windows.Documents.Model;
using System.Threading;
using System.Text.RegularExpressions;

namespace TownSuite.TwainScanner
{
    public partial class MainFrame : Form
    {
        private Twain32 _twain;
        string DirText;
        private DeviceManager deviceManager;
        private string FileExtention;
        private List<String> lstscansettings;
        private string UserTwainImageType;
        private string UserTwainScanner;

        public MainFrame(List<String> lstScanSet)
        {
            lstscansettings = lstScanSet;
            InitializeComponent();
        }

        public MainFrame()
        {
            InitializeComponent();
        }

        private void MainFrame_Load(object sender, EventArgs e)
        {
           for (int i = 0; i <= lstscansettings.Count - 1; i++)
            {
                switch (i)
                {
                    case 2:
                        UserTwainImageType = lstscansettings[i];
                        break;
                    case 3:
                        UserTwainScanner = lstscansettings[i];
                        break;
                }
            }

            try
            {
                _twain = new Twain32();
                this._twain.AcquireCompleted += new System.EventHandler(this._twain_AcquireCompleted);
                this._twain.OpenDSM();
              
            }
            catch(TwainException ex)
            {
                if (ex.Message == "It worked!")
                {
                    Console.WriteLine("Failed to find a scanner");
                    Console.Out.Flush();
                    this.Close();
                    return;
                }
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace), "TownSuite Scanner Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            DirText = Environment.GetEnvironmentVariable("TMP");

            GC.Collect();
            GC.WaitForPendingFinalizers();

            DeleteFiles();
                
            LoadTwainDrivers();
            LoadWIADrivers();
            GetColors();

            //Set Default Twain Scanner Settings
            cmbTwainImageType.SelectedItem = UserTwainImageType;
            sourceTwianListBox.SelectedItem = UserTwainScanner;
            //cmbTwainImageType.SelectedIndex = 0;

            //WIA Settings
            cmbImageType.SelectedIndex = 0;
            cmbColor.SelectedIndex = 0;

            cmbColor.DropDownStyle = ComboBoxStyle.DropDownList;  
            cmbResolution.DropDownStyle = ComboBoxStyle.DropDownList;

            tabScanDrivers.TabPages.Remove(tabScanDrivers.TabPages["tpWIAScan"]); 

            }

        #region Delete Temporary Files

        private void DeleteFiles()
        {
            LoopFiles("tmpscan*.bmp");
            LoopFiles("tmpscan*.jpeg");
            LoopFiles("tmpscan*.tif");
            LoopFiles("tmpscan*.png");
            LoopFiles("tmpscan*.pdf");
        }

        private void LoopFiles(string fileext)
        {
            //New Stuff
            //---------------------------
            string[] sa = null;
            string s = null;
            sa = Directory.GetFiles(DirText, fileext);

            foreach (string s_loopVariable in sa)
            {
                s = s_loopVariable;
                File.Delete(s);
            }
        }

        #endregion

        private void MenuItem5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region Load Twian Drivers

        public Twain32 Twain
        {
            get;
            set;
        }

        private void LoadTwainDrivers()
        {
            try
            {
                this.sourceTwianListBox.Items.Clear();
                //this._twain.CloseDataSource();
                //this._twain.SelectSource();
                Twain = this._twain;

                if (this.Twain != null && this.Twain.SourcesCount > 0)
                    {
                        for (int i = 0; i < this.Twain.SourcesCount; i++)
                        {
                        bool twn2 = this.Twain.GetIsSourceTwain2Compatible(i);
                            this.sourceTwianListBox.Items.Add(this.Twain.GetSourceProductName(i));
                        }
                        this.sourceTwianListBox.SelectedIndex = this.Twain.SourceIndex;
                    }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region WIA Drivers

        private void LoadWIADrivers()
        {
            // Clear the ListBox.
            sourceListBox.Items.Clear();

            // Create a DeviceManager instance
            deviceManager = new DeviceManager();
          

            // Loop through the list of devices and add the name to the listbox
            for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
            {
                // Add the device only if it's a scanner
                if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType)
                {
                    continue;
                }

                // Add the Scanner device to the listbox (the entire DeviceInfos object)
                // Important: we store an object of type scanner (which ToString method returns the name of the scanner)
                sourceListBox.Items.Add(
                    new WIAScanner.Scanner(deviceManager.DeviceInfos[i])
                );
            }
        }

        private void GetColors()
        {
            DataTable dtColors = new DataTable();
            DataColumn newColType = new DataColumn("Type", typeof(String));
            DataColumn newColCode = new DataColumn("Code", typeof(int));
            dtColors.Columns.Add(newColType);
            dtColors.Columns.Add(newColCode);
            dtColors.Rows.Add(new object[] { "Color", 1 });
            dtColors.Rows.Add(new object[] { "Grayscale", 2 });
            dtColors.Rows.Add(new object[] { "Black & White", 4 });

            cmbColor.DisplayMember = "Type";
            cmbColor.ValueMember = "Code";
            cmbColor.DataSource = dtColors;
        }

        public WIA.Device LongRunningOperationAsync(DeviceInfo deviceproInfo, CancellationToken token, CancellationTokenSource tokenSource) // assume we return an int from this long running operation 
        {
            WIA.Device device=null;
            while (!token.IsCancellationRequested)
            {
                device = deviceproInfo.Connect();
                tokenSource.Cancel();
                //await Task.Delay(1000); // 1 second delay
            }
            return device;
        }

        public WIA.Device ConnectWIA()
        {
            DeviceInfo deviceproInfo;
            deviceproInfo = deviceManager.DeviceInfos[sourceListBox.SelectedIndex + 1];

            WIA.Device device = null;

            CancellationTokenSource tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            tokenSource.CancelAfter(TimeSpan.FromSeconds(2));
            CancellationToken token = tokenSource.Token;


            //Func<Device> function = new Func<Device>(() => LongRunningOperationAsync(deviceproInfo, token, tokenSource));
            /*
            device = await Task.Run<Device>(() =>
            {
                device = LongRunningOperationAsync(deviceproInfo, token, tokenSource);
                return device;
            },token);

            while (token.IsCancellationRequested == false && device == null)
            { }
            */

            var t = Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    //Do some work
                    device = deviceproInfo.Connect();
                    //tokenSource.Cancel();
                }
                return device;
            }, token);

            t.Wait(2);
            tokenSource.Cancel();
            //while (token.IsCancellationRequested  == false)
            //{ }

            //tokenSource.Cancel();
            device = (Device)t.Result;

            if (device == null) // t.IsCompleted == false
            {
                MessageBox.Show("Cannot Connect to the Device(may not valid WIA Driver) ",
                                "Connect WIA Device",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            return device;

        }

        private WIA.Device DoSomething(DeviceInfo deviceproInfo)
        {
            WIA.Device device = null;
            try
            {
                device = deviceproInfo.Connect();
                return device;
            }
            catch (ThreadAbortException)
            {
                // cleanup code, if needed...
                return null;
            }
        }


        public void LoadScanPropertyValues() // async Task
        {
            var devicescanner = sourceListBox.SelectedItem as WIAScanner.Scanner;
            WIA.Device device = null;

            DeviceInfo deviceproInfo;
            deviceproInfo = deviceManager.DeviceInfos[sourceListBox.SelectedIndex + 1];

            Thread t = new Thread(() => { device = DoSomething(deviceproInfo); });
            t.Start();
            if (!t.Join(TimeSpan.FromSeconds(2)))
            {
                t.Abort();
                throw new Exception("More than 2 secs.");
            }
                     

           // device = ConnectWIA();

            //--------------------------------------------------
            var deviceitem = device.Items[1];

            IProperties properties;
            properties = deviceitem.Properties;

            foreach (Property item in deviceitem.Properties)
            {
                switch (item.PropertyID)
                {

                    case 6147: //dots per inch/horizontal 
                        string s = "";
                        ArrayList arryres = new ArrayList();
                        if (item.SubType == WIA.WiaSubType.FlagSubType)
                        {
                            s += " [valid flags include: ";
                        }
                        else
                        {
                            s += " [valid values include: ";
                        }
                        int count = item.SubTypeValues.Count;
                        
                        for (int i = 1; i <= count; i++)
                        {
                            arryres.Add(item.SubTypeValues.get_Item(i));
                            if (i < count)
                            {
                                s += ", ";
                            }
                        }
                        s += "]";

                        if (arryres.Count > 0)
                            cmbResolution.DataSource = arryres;
                        else
                            devicescanner.resolution = (int)item.get_Value();
                        break;
                    case 6154: //brightness
                        devicescanner.brightness = (int)item.get_Value();
                        break;
                    case 6155: //contrast
                        devicescanner.contrast = (int)item.get_Value();
                        break;
                }
            }

        }

        private void btnWIAScan_Click(object sender, EventArgs e)
        {

            //Start Scanning using a Thread
            //Task.Factory.StartNew(StartScanning).ContinueWith(result => TriggerScan());

            StartWIAScanning();

        }

        public static float MmToInch(int mm)
        {
            return 0.03937f * mm;
            
        }

        private WIAScanner.Scanner SetScanPropertyValues()
        {

            var devicescanner = sourceListBox.SelectedItem as WIAScanner.Scanner;

            DeviceInfo deviceproInfo;
            deviceproInfo = deviceManager.DeviceInfos[sourceListBox.SelectedIndex + 1];

            var device = deviceproInfo.Connect();
            var deviceitem = device.Items[1];

            IProperties properties;
            properties = deviceitem.Properties;

            SizeF documentSize = new SizeF(MmToInch(210), MmToInch(297));

            foreach (Property item in deviceitem.Properties)
            {
                switch (item.PropertyID)
                {
                    case 6146: //4 is Black-white,gray is 2, color 1
                        if ((cmbColor.SelectedItem != null))
                            devicescanner.color_mode = (int)cmbColor.SelectedValue;
                        else
                            devicescanner.color_mode = 4;
                        break;
                    case 6147: //dots per inch/horizontal 
                        if ((cmbResolution.SelectedItem != null))
                            devicescanner.resolution = (int)cmbResolution.SelectedValue;
                        else
                            devicescanner.resolution = (int)item.get_Value();
                        break;
                    case 6148: //dots per inch/vertical 
                        if ((cmbResolution.SelectedItem != null))
                            devicescanner.resolution = (int)cmbResolution.SelectedValue;
                        else
                            devicescanner.resolution = (int)item.get_Value();
                        break;
                    case 6149: //x point where to start scan 
                        devicescanner.left_pixel = (int)item.get_Value();
                        break;
                    case 6150: //y-point where to start scan 
                        devicescanner.top_pixel = (int)item.get_Value();
                        break;
                    case 6151: //horizontal exent 
                               //devicescanner.width_pixel = (int)(619 * devicescanner.resolution);   //  //(int)item.get_Value();
                        devicescanner.width_pixel = (int)(documentSize.Width * devicescanner.resolution);
                        break;
                    case 6152: //vertical extent 
                               //devicescanner.height_pixel = (int)(876 * devicescanner.resolution); //*  //(int)item.get_Value();
                        devicescanner.height_pixel = (int)(documentSize.Height * devicescanner.resolution);
                        break;
                    case 6154: //brightness
                        devicescanner.brightness = (int)item.get_Value();
                        break;
                    case 6155: //contrast
                        devicescanner.contrast = (int)item.get_Value();
                        break;
                }
            }
            return devicescanner;
        }

        private void TriggerScan()
        {
            Console.WriteLine("Image succesfully scanned");
        }

        public void StartWIAScanning()
        {
            WIAScanner.Scanner device = null;

            this.Invoke(new MethodInvoker(delegate ()
            {
                device = sourceListBox.SelectedItem as WIAScanner.Scanner;
            }));
                     

            if (device == null)
            {
                MessageBox.Show("You need to select first an scanner device from the list",
                                "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (String.IsNullOrEmpty("FileName"))
            {
                MessageBox.Show("Provide a filename",
                                "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(String.IsNullOrEmpty(cmbImageType.Text))
            {
                MessageBox.Show("You need to select scan Image ",
                               "Type",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (String.IsNullOrEmpty(cmbResolution.Text))
            {
                MessageBox.Show("You need to select a Resolution 100 is the best by default ",
                               "Select Resolution",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (String.IsNullOrEmpty(cmbColor.Text))
            {
                MessageBox.Show("You need to select a scan color type ",
                               "Select Color",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var devicescanner = SetScanPropertyValues();

            ImageFile image = new ImageFile();
            ArrayList arryimage = new ArrayList(); 

            string imageExtension = "";

            this.Invoke(new MethodInvoker(delegate ()
            {
                switch (cmbImageType.SelectedIndex)
                {

                    case 0:
                        arryimage = device.ScanTIFF();
                        imageExtension = ".tif";
                        FileExtention = imageExtension;
                        break;

                    case 1:
                        arryimage = device.ScanTIFF();
                        imageExtension = ".tif";
                        FileExtention = ".pdf";
                        break;

                    case 2:
                        arryimage = device.ScanPNG();
                        imageExtension = ".png";
                        FileExtention = imageExtension;
                        break;

                    case 3:
                        arryimage = device.ScanJPEG();
                        imageExtension = ".jpeg";
                        FileExtention = imageExtension;
                        break;
                }
            }));

            for (int i = 0; i <= arryimage.Count - 1; i += 1)
            {
                picnumber += 1;
                var newpic = new PictureBox();
                newpic.Image = (Image)arryimage[i];
                newpic.Size = new Size(newpic.Image.Width, newpic.Image.Height);
                newpic.Refresh();
                flowLayoutPanel1.Controls.Add(newpic);
                newpic.Text = "ScanPass" + picnumber.ToString() + "_Pic" + picnumber.ToString();
                newpic.Image.Save(DirText + @"\tmpScan" + picnumber.ToString() + "_" + i.ToString() + imageExtension, ImageFormat.Jpeg);
            }
        }

        #endregion


        private void mnuSelect_Click(object sender, EventArgs e)
        {
            try
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    using (SelectSourceForm _dlg = new SelectSourceForm { Twain = this._twain })
                    {
                        if (_dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            this._twain.SetDefaultSource(_dlg.SourceIndex);
                            this._twain.SourceIndex = _dlg.SourceIndex;
                        }
                    }
                }
                else
                {
                    //this._twain.CloseDataSource();
                    //this._twain.SelectSource();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SAMPLE1", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mnuAcquire_Click(object sender, EventArgs e)
        {
            try
            {
                switch(tabScanDrivers.SelectedTab.Name)
                {
                    case "tpWIAScan":
                        StartWIAScanning(); 
                        break;

                    case "tpTWAINScan":
                        StartTWIAScanning();
                        break;
               }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SAMPLE1", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void StartTWIAScanning()
        {

            if (String.IsNullOrEmpty(cmbTwainImageType.Text))
            {
                MessageBox.Show("You need to select scan Image ",
                               "type",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            switch (cmbTwainImageType.SelectedIndex)
            {
                case 0:
                    this._twain.Acquire();
                    FileExtention = ".tif";
                    break;

                case 1:
                    this._twain.Acquire();
                    FileExtention = ".pdf";
                    break;
            }
        }


        int picnumber = 0;
        private void _twain_AcquireCompleted(object sender, EventArgs e)
        {
            try
            {
                if (this._twain.ImageCount > 0)
                {
                    for (int i = 0; i <= this._twain.ImageCount - 1; i += 1)
                    {
                        picnumber += 1;
                        var newpic = new PictureBox();
                        newpic.Image = this._twain.GetImage(i);
                        newpic.Size = new Size(newpic.Image.Width, newpic.Image.Height);
                        newpic.Refresh();
                        flowLayoutPanel1.Controls.Add(newpic);
                        newpic.Text = "ScanPass" + picnumber.ToString() + "_Pic" + picnumber.ToString();
                        newpic.Image.Save(DirText + @"\tmpScan" + picnumber.ToString() + "_" + i.ToString() + ".bmp", ImageFormat.Bmp);
                        // newpic.doTmpSave(DirText + "\\tmpScan" + picnumber.ToString() + "_" + i.ToString() + ".bmp");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "TownSuite Scan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Save Scan Files

        private void mnuSave_Click(object sender, EventArgs e)
        {
            try
            {
                switch (tabScanDrivers.SelectedTab.Name)
                {
                    case "tpTWAINScan":
                        switch (cmbTwainImageType.SelectedIndex)
                        {
                            case 0:
                                //Save tiff
                                SaveTWAIN_TIFF();
                                break;
                            case 1:
                                //Save pdf
                                SaveTWAIN_PDF();
                                break;
                        }
                        break;
                    case "tpWIAScan":
                        switch (cmbImageType.SelectedIndex)
                        {
                            case 0:
                                //Save tiff
                                SaveTIFF();
                                break;
                            case 1:
                                //Save pdf
                                SavePDF();
                                break;
                            case 2:
                                //Save PNG'
                                SavePNG();
                                break;
                            case 3:
                                //Save Jepg
                                SaveJPEG();
                                break;
                        }
                        break;
                }

                Console.WriteLine();
                Console.WriteLine(" " + FileExtention + " ");
                Console.WriteLine();
                Console.WriteLine("SAVED");
                Console.Out.Flush();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Occured on Scanning & Saving Please Re-try Scanning" + "\r\n" + ex.Message,"Scan Document" ,MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #region Save Twian Files

        public static string PadNumbers(string input)
        {
            string smallName = System.IO.Path.GetFileNameWithoutExtension(input);
            // Default ordering for a string is standard alpha numeric dictionary ordering.
            // when individual files like tmpScan1_0.bmp , tmpScan2_1.bmp .....tmpScan10_9.bmp, tmpScan11_10
            //Then tmpScan1_0.bmp will be first and order the files and tmpScan10_9.bmp, tmpScan11_10 after
            return Regex.Replace(smallName, "[0-9]+", match => match.Value.PadLeft(10, '0'));
        }

        private void SaveTWAIN_PDF()
        {
            string[] sa = null;
            sa = Directory.GetFiles(DirText, "tmpscan*.bmp");

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
                    pages = (Bitmap)Image.FromFile(s);

                    //save the first frame
                    pages.Save(DirText + "\\tmpScan.tif", info, ep);
                }
                else
                {
                    //save the intermediate frames
                    ep.Param[0] = new EncoderParameter(enc, Convert.ToInt64(EncoderValue.FrameDimensionPage));
                    Bitmap bm = (Bitmap)Image.FromFile(s);
                    pages.SaveAdd(bm, ep);
                }
                if (frame == sa.Length - 1)
                {
                    //flush and close.
                    ep.Param[0] = new EncoderParameter(enc, Convert.ToInt64(EncoderValue.Flush));
                    pages.SaveAdd(ep);

                }
                frame += 1;

            }

            RadFixedDocument document = new RadFixedDocument();

            foreach (string image in sa)
            {
                using (Stream stream = File.OpenRead(image))
                {
                    ImageSource imageSource = new ImageSource(stream);
                    RadFixedPage page = document.Pages.AddPage();
                    page.Size = new System.Windows.Size(imageSource.Width, imageSource.Height);
                    page.Content.AddImage(imageSource);
                                 
                }
            }
            
            PdfFormatProvider provider = new PdfFormatProvider();
            using (Stream output = new FileStream(DirText + "\\tmpScan.pdf", FileMode.OpenOrCreate))
            {
                provider.ExportSettings.ImageQuality = ImageQuality.High;
                provider.Export(document, output);
            }

        }

        private void SaveTWAIN_TIFF()
        {
            string[] sa = null;
            sa = Directory.GetFiles(DirText, "tmpscan*.bmp");

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
                    pages = (Bitmap)Image.FromFile(s);

                    //save the first frame
                    pages.Save(DirText + "\\tmpScan.tif", info, ep);
                }
                else
                {
                    //save the intermediate frames
                    ep.Param[0] = new EncoderParameter(enc, Convert.ToInt64(EncoderValue.FrameDimensionPage));
                    Bitmap bm = (Bitmap)Image.FromFile(s);
                    pages.SaveAdd(bm, ep);
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

        #endregion




        private string SaveJPEG()
        {
            string[] inputFiles = null;
            inputFiles = Directory.GetFiles(DirText, "tmpscan*.jpeg");

            // TODO: You may want to add input checking here.
            Image[] images = new Image[inputFiles.Length];
            Image image;
            int height = 0, width = 0;

            try
            {
                for (int i = 0; i < inputFiles.Length; ++i)
                {
                    images[i] = image = Image.FromFile(inputFiles[i]);
                    height = Math.Max(height, image.Height);
                    width += image.Width;
                }

                image = new Bitmap(width, height);
                width = 0;

                using (Graphics g = Graphics.FromImage(image))
                {
                    g.Clear(SystemColors.AppWorkspace);

                    for (int i = 0; i < images.Length; ++i)
                    {
                        g.DrawImage(images[i], new Point(width, 0));
                        width += images[i].Width;
                    }
                }

                // You don't need to save this in order to use the in-memory object.
                // img3.Save(finalImage, System.Drawing.Imaging.ImageFormat.Jpeg);

                image.Save(DirText + @"\tmpScan.jpeg");
                //imageLocation.Image = image;
            }
            finally
            {
                for (int i = 0; i < inputFiles.Length; ++i)
                    if (images[i] != null)
                        images[i].Dispose();
            }
            
            return "";
        }
        
        private void SaveTIFF()
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
                    pages = (Bitmap)Image.FromFile(s);

                    //save the first frame
                    pages.Save(DirText + "\\tmpScan.tif", info, ep);
                }
                else
                {
                    //save the intermediate frames
                    ep.Param[0] = new EncoderParameter(enc, Convert.ToInt64(EncoderValue.FrameDimensionPage));
                    Bitmap bm = (Bitmap)Image.FromFile(s);
                    pages.SaveAdd(bm, ep);
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
            string[] inputFiles = null;
            inputFiles = Directory.GetFiles(DirText, "tmpscan*.png");

            // TODO: You may want to add input checking here.
            Image[] images = new Image[inputFiles.Length];
            Image image;
            int height = 0, width = 0;

            try
            {
                for (int i = 0; i < inputFiles.Length; ++i)
                {
                    images[i] = image = Image.FromFile(inputFiles[i]);
                    height = Math.Max(height, image.Height);
                    width += image.Width;
                }

                image = new Bitmap(width, height);
                width = 0;

                using (Graphics g = Graphics.FromImage(image))
                {
                    g.Clear(SystemColors.AppWorkspace);

                    for (int i = 0; i < images.Length; ++i)
                    {
                        g.DrawImage(images[i], new Point(width, 0));
                        width += images[i].Width;
                    }
                }

                // You don't need to save this in order to use the in-memory object.
                // img3.Save(finalImage, System.Drawing.Imaging.ImageFormat.Jpeg);

                image.Save(DirText + @"\tmpScan.png");
                //imageLocation.Image = image;
            }
            finally
            {
                for (int i = 0; i < inputFiles.Length; ++i)
                    if (images[i] != null)
                        images[i].Dispose();
            }

            return "";
        }


        private void SavePDF()
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
                    pages = (Bitmap)Image.FromFile(s);

                    //save the first frame
                    pages.Save(DirText + "\\tmpScan.tif", info, ep);
                }
                else
                {
                    //save the intermediate frames
                    ep.Param[0] = new EncoderParameter(enc, Convert.ToInt64(EncoderValue.FrameDimensionPage));
                    Bitmap bm = (Bitmap)Image.FromFile(s);
                    pages.SaveAdd(bm, ep);
                }
                if (frame == sa.Length - 1)
                {
                    //flush and close.
                    ep.Param[0] = new EncoderParameter(enc, Convert.ToInt64(EncoderValue.Flush));
                    pages.SaveAdd(ep);

                }
                frame += 1;

            }

            RadFixedDocument document = new RadFixedDocument();
            
            foreach (string image in sa)
            {
                using (Stream stream = File.OpenRead(image))
                {
                    ImageSource imageSource = new ImageSource(stream);
                    RadFixedPage page = document.Pages.AddPage();
                    //page.Size = new System.Windows.Size(imageSource.Width, imageSource.Height);
                    page.Content.AddImage(imageSource);
                    if((int)(cmbResolution.SelectedValue) <=100)
                    {
                        page.Size = PaperTypeConverter.ToSize(PaperTypes.Letter);
                    }
                    else
                    {
                        page.Size = new System.Windows.Size(imageSource.Width, imageSource.Height);
                    }
                }
            }
            
                       
            /*// Old way 
            Stream stream = File.OpenRead(DirText + "\\tmpScan.tif");
            ImageSource imageSource = new ImageSource(stream);
                    
            RadFixedPage page = document.Pages.AddPage();
            page.Content.AddImage(imageSource);
            if ((int)(cmbResolution.SelectedValue) <= 100)
            {
                page.Size = PaperTypeConverter.ToSize(PaperTypes.Letter);
            }
            else
            {
                page.Size = new System.Windows.Size(imageSource.Width, imageSource.Height);
            }
            */
            

            PdfFormatProvider provider = new PdfFormatProvider();
            using (Stream output = new FileStream(DirText + "\\tmpScan.pdf", FileMode.OpenOrCreate))
            {
                provider.ExportSettings.ImageQuality = ImageQuality.High;
                provider.Export(document, output);
            }

        }

        #endregion

        private void sourceListBox_SelectedValueChanged(object sender, EventArgs e)
        {   
            //var t=Task.Run(() =>
            //{
            //    LoadScanPropertyValues();
            //});
            //t.Wait(1000);

            LoadScanPropertyValues();

        }

        private void sourceTwianListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            //this._twain.SetDefaultSource(sourceTwianListBox.SelectedIndex);
            this._twain.SourceIndex = sourceTwianListBox.SelectedIndex;
        }
    }
}
