#if INCLUDE_ORIGINAL
using NAPS2.Images;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using WIA;
using System.Threading;
using System.Collections;
using System.Data;

namespace TownSuite.TwainScanner.Backends
{
    class OriginalWiaBackend : IBackend
    {

        private DeviceManager deviceManager;
        bool removeWia = false;


        public OriginalWiaBackend(string dirText, Ocr ocr) : base(dirText, ocr)
        {
        }

        public override async Task ConfigureSettings()
        {
            await base.ConfigureSettings();

            LoadWIADrivers();
            GetColors();
        }

        public override void ChangeSelectedScanner(object value)
        {
            var sourceTwainListBox = value as ListBox;
            LoadScanPropertyValues(sourceTwainListBox);
        }

        public override Task Scan(string imageFormat)
        {
            base.Scan(imageFormat);

            StartWIAScanning(base.imageFormat);
            return Task.CompletedTask;
        }

        public void GetColors()
        {
            DataTable dtColors = new DataTable();
            DataColumn newColType = new DataColumn("Type", typeof(String));
            DataColumn newColCode = new DataColumn("Code", typeof(int));
            dtColors.Columns.Add(newColType);
            dtColors.Columns.Add(newColCode);
            dtColors.Rows.Add(new object[] { "Color", 1 });
            dtColors.Rows.Add(new object[] { "Grayscale", 2 });
            dtColors.Rows.Add(new object[] { "Black & White", 4 });

            var cmbColor = ParentForm.GetWiaColor();
            cmbColor.DisplayMember = "Type";
            cmbColor.ValueMember = "Code";
            cmbColor.DataSource = dtColors;
        }

        private void LoadWIADrivers()
        {
            // Clear the ListBox.
            var sourceList = ParentForm.GetWiaSourceList();
            sourceList.Items.Clear();

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
                sourceList.Items.Add(
                    new WIAScanner.Scanner(deviceManager.DeviceInfos[i])
                );
            }
            // sourceList.SelectedIndex = 

        }

        private WIA.Device DoSomething(DeviceInfo deviceproInfo)
        {
            try
            {
                WIA.Device device = deviceproInfo.Connect();
                return device;
            }
            catch (ThreadAbortException)
            {
                // cleanup code, if needed...
                return null;
            }
        }

        public void LoadScanPropertyValues(ListBox sourceListBox) // async Task
        {
            var devicescanner = sourceListBox.SelectedItem as WIAScanner.Scanner;
            if (devicescanner == null)
            {
                return;
            }

            WIA.Device device = null;

            DeviceInfo deviceproInfo;
            deviceproInfo = deviceManager.DeviceInfos[sourceListBox.SelectedIndex + 1];

            Thread t = new Thread(() => { device = DoSomething(deviceproInfo); });
            t.Start();
            if (!t.Join(TimeSpan.FromSeconds(5)))
            {
                t.Abort();
                throw new Exception("More than 5 secs.");
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
                        {
                            var cmbResolution = ParentForm.GetWiaResolution();
                            cmbResolution.DataSource = arryres;
                        }
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

        public static float MmToInch(int mm)
        {
            return 0.03937f * mm;

        }

        private WIAScanner.Scanner SetScanPropertyValues()
        {
            var sourceListBox = ParentForm.GetWiaSourceList();
            var devicescanner = sourceListBox.SelectedItem as WIAScanner.Scanner;

            DeviceInfo deviceproInfo;
            deviceproInfo = deviceManager.DeviceInfos[sourceListBox.SelectedIndex + 1];

            var device = deviceproInfo.Connect();
            var deviceitem = device.Items[1];

            IProperties properties = deviceitem.Properties;

            SizeF documentSize = new SizeF(MmToInch(210), MmToInch(297));

            var cmbColor = ParentForm.GetWiaColor();
            var cmbResolution = ParentForm.GetWiaResolution();
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

        private void StartWIAScanning(string imageFormat)
        {
            WIAScanner.Scanner device = null;

            var sourceListBox = ParentForm.GetWiaSourceList();
            device = sourceListBox.SelectedItem as WIAScanner.Scanner;

            var cmbColor = ParentForm.GetWiaColor();
            var cmbResolution = ParentForm.GetWiaResolution();

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

            if (String.IsNullOrEmpty(imageFormat))
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


            ParentForm.Invoke(new MethodInvoker(delegate ()
            {
                switch (imageFormat)
                {

                    case "tiff":
                        arryimage = device.ScanTIFF();
                        imageExtension = ".tif";
                        FileExtention = imageExtension;
                        break;

                    case "pdf":
                        arryimage = device.ScanJPEG();
                        imageExtension = ".jpeg";
                        FileExtention = ".pdf";
                        break;

                    case "png":
                        arryimage = device.ScanPNG();
                        imageExtension = ".png";
                        FileExtention = imageExtension;
                        break;

                    case "jpeg":
                        arryimage = device.ScanJPEG();
                        imageExtension = ".jpeg";
                        FileExtention = imageExtension;
                        break;
                }
            }));

            int picnumber = 0;
            for (int i = 0; i <= arryimage.Count - 1; i += 1)
            {
                picnumber += 1;
                var newpic = new PictureBox();
                System.Drawing.Image resizedImg;
                string origPath = Path.Combine(DirText, "tmpScan" + picnumber.ToString() + "_" + i.ToString() + Guid.NewGuid().ToString() + imageExtension);

                using (var img = (System.Drawing.Image)arryimage[i])
                {
                    newpic.Tag = origPath;
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
                }

                newpic.Image = resizedImg;
                newpic.Size = new Size(newpic.Image.Width, newpic.Image.Height);
                newpic.Refresh();
                newpic.DoubleClick += Newpic_DoubleClick;
                newpic.MouseEnter += Newpic_MouseEnter;
                newpic.MouseLeave += Newpic_MouseLeave;
                var flowLayoutPanel1 = ParentForm.GetFlowLayoutPanel();
                flowLayoutPanel1.Controls.Add(newpic);
                newpic.Text = "ScanPass" + picnumber.ToString() + "_Pic" + picnumber.ToString();
                flowLayoutPanel1.Controls.Add(newpic);
                RunOcr(newpic, origPath, OcrEnabled());
            }
        }


    }
}
#endif