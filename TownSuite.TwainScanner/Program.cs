using Avalonia;
using NAPS2.Scan;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TownSuite.TwainScanner
{
    static class Program
    {
        internal static List<string> Settings { get; private set; } = new List<string>();
        internal static Ocr OcrInstance { get; private set; }
        internal static string DirText { get; private set; }

        [STAThread]
        static int Main(string[] args)
        {
            bool scanList = false;
            bool enableOcr = false;
            string ocrApiUrl = "";
            string ocrBearerToken = "";
            string workingDir = Environment.GetEnvironmentVariable("TMP");
            List<string> settings = new List<string>();

            try
            {
                var cmdArgs = Environment.GetCommandLineArgs();
                List<string> scanListOutput = new List<string>();

                for (int i = 0; i < cmdArgs.Length; i++)
                {
                    if (cmdArgs[i] == "-scanlist")
                    {
                        scanList = true;
                        var drivers = new Driver[] { Driver.Wia, Driver.Twain };
                        Task.Run(async () =>
                        {
                            foreach (var driver in drivers)
                            {
                                var scnlst = new NewScannerList(driver);
                                using var context = scnlst.GetScanContext();
                                var names = await scnlst.ScanList(context);
                                scanListOutput.AddRange(names);
                            }
                        }).GetAwaiter().GetResult();
                    }

                    if (cmdArgs[i] == "-scansettings")
                    {
                        foreach (string s in cmdArgs)
                            settings.Add(s);
                    }

                    if (cmdArgs[i] == "-enableocr")
                    {
                        Console.WriteLine("Enabling OCR");
                        enableOcr = true;
                    }
                    if (cmdArgs[i] == "-ocrapiurl" && i + 1 < cmdArgs.Length)
                        ocrApiUrl = cmdArgs[i + 1];
                    if (cmdArgs[i] == "-ocrbearertoken" && i + 1 < cmdArgs.Length)
                        ocrBearerToken = cmdArgs[i + 1];
                    if (cmdArgs[i] == "-tempdir" && i + 1 < cmdArgs.Length)
                        workingDir = cmdArgs[i + 1];
                }

                string dirText = Path.Combine(workingDir ?? Path.GetTempPath(), "TownSuiteScanner");

                if (scanList)
                {
                    foreach (string name in scanListOutput)
                        Console.WriteLine(name);
                    Console.WriteLine("ScanListEnd");
                    Console.Out.Flush();
                    return 0;
                }

                Settings = settings;
                OcrInstance = new Ocr(enableOcr, ocrApiUrl, ocrBearerToken);
                DirText = dirText;

                return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Here is the error {e}");
                Console.Error.Flush();
                return 1;
            }
        }

        static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}
