#if INCLUDE_TELERIK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WIA;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;

namespace WIAScanner
{
    class Scanner
    {
        private readonly DeviceInfo _deviceInfo;
        public int resolution = 100;
        public int width_pixel = 850;  //1250;
        public int height_pixel = 1169;  //1700;
        public int color_mode;  //= 1;

        public int left_pixel = 0;
        public int top_pixel = 0;

        public int brightness = 0;
        public int contrast = 0;
        public WIA.Device deviceConn; 

        public Scanner(DeviceInfo deviceInfo)
        {
            this._deviceInfo = deviceInfo;
        }
 
        public void ScannerProperties(DeviceInfo deviceproInfo)
        {
            var device = this._deviceInfo.Connect();
            var item = device.Items[1];

        }

        /// <summary>
        /// Scan a image with PNG Format
        /// </summary>
        /// <returns></returns>
        public ArrayList ScanPNG()
        {
            // Connect to the device and instruct it to scan
            // Connect to the device
            var device = this._deviceInfo.Connect();

            // Select the scanner
            CommonDialogClass dlg = new CommonDialogClass(); 
            
            var item = device.Items[1];
            ArrayList arrImages = new ArrayList();
                       

            try
            {
                AdjustScannerSettings(item, resolution, 0, 0, width_pixel, height_pixel, 0, 0, color_mode);

                //object scanResult = dlg.ShowTransfer(item, WIA.FormatID.wiaFormatPNG, true);

                ImageFile image = (ImageFile)dlg.ShowTransfer(item, WIA.FormatID.wiaFormatTIFF, true);

                // Multiple Pages Scan
                var imageBytes = (byte[])image.FileData.get_BinaryData();
                var ms = new MemoryStream(imageBytes);
                Bitmap bitmap = (Bitmap)System.Drawing.Image.FromStream(ms);

                //tiff images can contain multiple images named as frame 
                int count = bitmap.GetFrameCount(FrameDimension.Page);
                for (int idx = 0; idx < count; idx++)
                {
                    // save each frame to a bytestream
                    bitmap.SelectActiveFrame(FrameDimension.Page, idx);
                    MemoryStream byteStream = new MemoryStream();
                    bitmap.Save(byteStream, ImageFormat.Jpeg);

                    // and then create a new Image from it
                    System.Drawing.Image newImage = System.Drawing.Image.FromStream(byteStream);

                    arrImages.Add(newImage);

                }

                if (arrImages != null)
                {
                    //var imageFile = image;

                    // Return the imageFile
                    return arrImages;
                }

            }
            catch (COMException e)
            {
                // Display the exception in the console.
                Console.WriteLine(e.ToString());

                uint errorCode = (uint)e.ErrorCode;

                // Catch 2 of the most common exceptions
                if (errorCode ==  0x80210006)
                {
                    MessageBox.Show("The scanner is busy or isn't ready");
                }else if(errorCode == 0x80210064)
                {
                    MessageBox.Show("The scanning process has been cancelled.");
                }else
                {
                    MessageBox.Show("A non catched error occurred, check the console","Error",MessageBoxButtons.OK);
                }
            }

            return new ArrayList();
        }

        /// <summary>
        /// Scan a image with JPEG Format
        /// </summary>
        /// <returns></returns>
        public ArrayList ScanJPEG()
        {
            // Connect to the device and instruct it to scan
            // Connect to the device
            var device = this._deviceInfo.Connect();

            // Select the scanner
            CommonDialogClass dlg = new CommonDialogClass();

            var item = device.Items[1];
            ArrayList arrImages = new ArrayList();
            try
            {
                AdjustScannerSettings(item, resolution, 0, 0, width_pixel, height_pixel, 0, 0, color_mode);

                ImageFile image = (ImageFile)dlg.ShowTransfer(item, WIA.FormatID.wiaFormatJPEG, true);

                // Multiple Pages Scan
                var imageBytes = (byte[])image.FileData.get_BinaryData();
                var ms = new MemoryStream(imageBytes);
                Bitmap bitmap = (Bitmap)System.Drawing.Image.FromStream(ms);

                //tiff images can contain multiple images named as frame 
                int count = bitmap.GetFrameCount(FrameDimension.Page);
                for (int idx = 0; idx < count; idx++)
                {
                    // save each frame to a bytestream
                    bitmap.SelectActiveFrame(FrameDimension.Page, idx);
                    MemoryStream byteStream = new MemoryStream();
                    bitmap.Save(byteStream, ImageFormat.Jpeg);

                    // and then create a new Image from it
                    System.Drawing.Image newImage = System.Drawing.Image.FromStream(byteStream);

                    arrImages.Add(newImage); 

                }
                
                if (arrImages != null)
                {
                    //var imageFile = image;

                    // Return the imageFile
                    return arrImages;
                }
            }
            catch (COMException e)
            {
                // Display the exception in the console.
                Console.WriteLine(e.ToString());

                uint errorCode = (uint)e.ErrorCode;

                // Catch 2 of the most common exceptions
                if (errorCode == 0x80210006)
                {
                    MessageBox.Show("The scanner is busy or isn't ready");
                }
                else if (errorCode == 0x80210064)
                {
                    MessageBox.Show("The scanning process has been cancelled.");
                }
                else
                {
                    MessageBox.Show("A non catched error occurred, check the console", "Error", MessageBoxButtons.OK);
                }
            }

            return new ArrayList();
        }

        public static void SetDeviceProperty(Device device, int propertyId, object value)
        {
            Property property = FindProperty(device.Properties, propertyId);
            if (property != null)
                property.set_Value(ref value);
        }

        public static Property FindProperty(WIA.Properties properties, int propertyId)
        {
            foreach (Property property in properties)
                if (property.PropertyID == propertyId)
                    return property;
            return null;
        }

        //private static void SetWIAPropertyValue(IProperties properties, object propName, object propValue)
        //{
        //    Property prop = properties.get_Item(ref propName);
        //    prop.set_Value(ref propValue);
        //}

        /// <summary>
        /// Scan a image with TIFF Format
        /// </summary>
        /// <returns></returns>
        public ArrayList ScanTIFF()
        {
            // Connect to the device and instruct it to scan
            // Connect to the device
            var device = this._deviceInfo.Connect();

            // Select the scanner
            CommonDialogClass dlg = new CommonDialogClass();

            var item = device.Items[1];
            ArrayList arrImages = new ArrayList();

            try
            {
                double pval = 100;
                

                AdjustScannerSettings(item, resolution, 0, 0, width_pixel, height_pixel, 0, 0, color_mode);

                //SetDeviceProperty(device, 3097, pval);

                ImageFile image = (ImageFile)dlg.ShowTransfer(item, WIA.FormatID.wiaFormatTIFF, true);

                // Multiple Pages Scan
                var imageBytes = (byte[])image.FileData.get_BinaryData();
                var ms = new MemoryStream(imageBytes);
                Bitmap bitmap = (Bitmap)System.Drawing.Image.FromStream(ms);

                //tiff images can contain multiple images named as frame 
                int count = bitmap.GetFrameCount(FrameDimension.Page);
                for (int idx = 0; idx < count; idx++)
                {
                    // save each frame to a bytestream
                    bitmap.SelectActiveFrame(FrameDimension.Page, idx);
                    MemoryStream byteStream = new MemoryStream();
                    bitmap.Save(byteStream, ImageFormat.Jpeg);

                    // and then create a new Image from it
                    System.Drawing.Image newImage = System.Drawing.Image.FromStream(byteStream);

                    arrImages.Add(newImage);

                }

                if (arrImages != null)
                {
                    //var imageFile = image;

                    // Return the imageFile
                    return arrImages;
                }
                
            }
            catch (COMException e)
            {
                // Display the exception in the console.
                Console.WriteLine(e.ToString());

                uint errorCode = (uint)e.ErrorCode;

                // Catch 2 of the most common exceptions
                if (errorCode == 0x80210006)
                {
                    MessageBox.Show("The scanner is busy or isn't ready");
                }
                else if (errorCode == 0x80210064)
                {
                    MessageBox.Show("The scanning process has been cancelled.");
                }
                else
                {
                    MessageBox.Show("A non catched error occurred, check the console", "Error", MessageBoxButtons.OK);
                }
            }

            return new ArrayList();
        }

        /// <summary>
        /// Adjusts the settings of the scanner with the providen parameters.
        /// </summary>
        /// <param name="scannnerItem">Expects a </param>
        /// <param name="scanResolutionDPI">Provide the DPI resolution that should be used e.g 150</param>
        /// <param name="scanStartLeftPixel"></param>
        /// <param name="scanStartTopPixel"></param>
        /// <param name="scanWidthPixels"></param>
        /// <param name="scanHeightPixels"></param>
        /// <param name="brightnessPercents"></param>
        /// <param name="contrastPercents">Modify the contrast percent</param>
        /// <param name="colorMode">Set the color mode</param>
        private static void AdjustScannerSettings(IItem scannnerItem, int scanResolutionDPI, int scanStartLeftPixel, int scanStartTopPixel, int scanWidthPixels, int scanHeightPixels, int brightnessPercents, int contrastPercents, int colorMode)
        {
            const string WIA_SCAN_COLOR_MODE = "6146";
            const string WIA_HORIZONTAL_SCAN_RESOLUTION_DPI = "6147";
            const string WIA_VERTICAL_SCAN_RESOLUTION_DPI = "6148";
            const string WIA_HORIZONTAL_SCAN_START_PIXEL = "6149";
            const string WIA_VERTICAL_SCAN_START_PIXEL = "6150";
            const string WIA_HORIZONTAL_SCAN_SIZE_PIXELS = "6151";
            const string WIA_VERTICAL_SCAN_SIZE_PIXELS = "6152";
            const string WIA_SCAN_BRIGHTNESS_PERCENTS = "6154";
            const string WIA_SCAN_CONTRAST_PERCENTS = "6155";
            //const string WIA_PAGE_LETTER = "100";
            //const string WIA_IPS_PAGE_SIZE = "3097";

            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_START_PIXEL, scanStartLeftPixel);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_START_PIXEL, scanStartTopPixel);
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_SIZE_PIXELS, scanWidthPixels);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_SIZE_PIXELS, scanHeightPixels);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_BRIGHTNESS_PERCENTS, brightnessPercents);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_CONTRAST_PERCENTS, contrastPercents);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_COLOR_MODE, colorMode);
            //SetWIAProperty(scannnerItem.Properties, WIA_PAGE_LETTER, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        private static void SetWIAProperty(IProperties properties, object propName, object propValue)
        {
            Property prop = properties.get_Item(ref propName);
            prop.set_Value(ref propValue);
        }

        /// <summary>
        /// Declare the ToString method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return (string) this._deviceInfo.Properties["Name"].get_Value();
        }
         
    }
}

#endif