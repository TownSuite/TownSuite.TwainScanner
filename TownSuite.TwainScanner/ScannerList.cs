using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TownSuite.TwainScanner
{
   public class ScannerList
    {
        private Twain32 _twain;
        public List<string> ScanList()
        {
            List<string> lstscan = new List<string>(); 
            try
            {
                _twain = new Twain32();
               this._twain.OpenDSM();

                if (_twain != null && _twain.SourcesCount > 0)
                {
                    for (int i = 0; i < _twain.SourcesCount; i++)
                    {
                        bool twn2 = _twain.GetIsSourceTwain2Compatible(i);
                        lstscan.Add(_twain.GetSourceProductName(i)); 
                    }
                }
                return lstscan;
            }
            catch (TwainException ex)
            {
                if (ex.Message == "It worked!")
                {
                    Console.WriteLine("Failed to find a scanner");
                    Console.Out.Flush();
                }
                return lstscan;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace), "TownSuite Scanner Load Error");
                return lstscan;
            }

        }

    }
}
