using System;
using System.Collections.Generic;
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
            List<string> scanlist = new List<string>(); 
            for (int i = 0; i <= Environment.GetCommandLineArgs().Length - 1; i++)
            {
                if (Environment.GetCommandLineArgs()[i] == "-scanlist")
                {
                    ScanList = true;
                    var scnlst = new ScannerList();
                    scanlist = scnlst.ScanList(); 
                }

                if(Environment.GetCommandLineArgs()[i] == "-scansettings")
                {
                    foreach (string s in Environment.GetCommandLineArgs())
                    {
                        settings.Add(s);
                   }
                }
            }

            if(ScanList==false)
            { 
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainFrame(settings));
            }
            else
            {
                foreach(string i  in scanlist)
                {
                    Console.WriteLine(i);
                }
                Console.WriteLine("ScanListEnd");
                Console.Out.Flush();
            }



        }
    }
}
