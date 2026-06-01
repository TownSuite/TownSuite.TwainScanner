using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using NAPS2.Scan;

namespace TownSuite.TwainScanner
{
    internal interface IMainView
    {
        void AddThumbnail(string filePath, Bitmap thumbnail);
        void SetScanningStatus(bool running, string message = "");
        void SetOcrStatus(bool running, string message = "");
        void MarkOcrComplete(string filePath, string ocrText);
        Task ShowErrorAsync(string message, string title);
        int GetResolutionDpi();
        ScanDevice GetSelectedDevice();
        List<ScanDevice> GetDeviceList();
        void SetDeviceList(List<ScanDevice> devices, bool append = false);
        bool IsOcrChecked { get; }
    }
}
