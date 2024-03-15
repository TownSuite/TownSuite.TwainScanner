using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TownSuite.TwainScanner
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var settings = new List<String>();
            bool ScanList = false;
            bool enableOcr = false;
            string ocrApiUrl = "";
            string ocrBearerToken = "";
            string workingDir = Environment.GetEnvironmentVariable("TMP");

            List<string> scanlist = new List<string>();
            for (int i = 0; i <= Environment.GetCommandLineArgs().Length - 1; i++)
            {
                if (Environment.GetCommandLineArgs()[i] == "-scanlist")
                {
                    ScanList = true;
                    var scnlst = new ScannerList();
                    scanlist = scnlst.ScanList();
                }

                if (Environment.GetCommandLineArgs()[i] == "-scansettings")
                {
                    foreach (string s in Environment.GetCommandLineArgs())
                    {
                        settings.Add(s);
                    }
                }

                if (Environment.GetCommandLineArgs()[i] == "-enableocr")
                {
                    enableOcr = true;
                }
                if (Environment.GetCommandLineArgs()[i] == "-ocrapiurl")
                {
                    ocrApiUrl = Environment.GetCommandLineArgs()[i+1];
                }
                if (Environment.GetCommandLineArgs()[i] == "-ocrbearertoken")
                {
                    ocrBearerToken = Environment.GetCommandLineArgs()[i + 1];
                }
                if (Environment.GetCommandLineArgs()[i] == "-tempdir")
                {
                    workingDir = Environment.GetCommandLineArgs()[i + 1];
                }

            }

            if (ScanList == false)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainFrame(settings, new Ocr(enableOcr, ocrApiUrl, ocrBearerToken), workingDir));
            }
            else
            {
                foreach (string i in scanlist)
                {
                    Console.WriteLine(i);
                }
                Console.WriteLine("ScanListEnd");
                Console.Out.Flush();
            }



        }
    }
}
