using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TownSuite.TwainScanner
{
    static internal class I18N
    {
        public static string GetString(string key)
        {
            ResourceManager rm = new ResourceManager("TownSuite.TwainScanner.Resources.Resources", typeof(I18N).Assembly);
            if (System.Globalization.CultureInfo.CurrentCulture.Name.StartsWith("fr-"))
            {
                return rm.GetString(key, new CultureInfo("fr-CA"));
            }
            return rm.GetString(key);
        }
    }
}
