using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TownSuite.TwainScanner
{
    internal class Ocr
    {
        public bool Enabled { get; private set; }
        string _apiUrl;
        string _bearerToken;

        public Ocr(bool enabled, string apiUrl, string bearerToken)
        {
            Enabled = enabled;
            _apiUrl = apiUrl;
            _bearerToken = bearerToken;
        }

        public async Task<string> GetText(string filepath)
        {
            var client = new WebClient();
            client.Headers.Add("Authorization", $"Bearer {_bearerToken}");
            client.Headers.Add("User-Agent", "TownSuiteScanner/1.0 Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:109.0) Gecko/20100101 Firefox/113.0");

#if DEBUG
            if (ServicePointManager.ServerCertificateValidationCallback == null)
            {
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
            }
#endif

            byte[] byteImage = System.IO.File.ReadAllBytes(filepath);
            var output = await client.UploadDataTaskAsync(_apiUrl, byteImage);

            return System.Text.Encoding.UTF8.GetString(output);

        }
    }
}
