using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public void GetText(string filepath, PictureBox newpic)
        {
            QueueThread(async () =>
            {
                try
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

                    string text = System.Text.Encoding.UTF8.GetString(output);
                    newpic.Invoke((MethodInvoker)delegate
                    {
                        // Running on the UI thread
                        var tt = new ToolTip();
                        tt.SetToolTip(newpic, text);
                        System.IO.File.WriteAllText($"{filepath}.txt", text);
                    });

                }
                catch (Exception ex)
                {
                    newpic.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show(ex.Message, "OCR Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });

        }

        private ConcurrentQueue<Action> _theQueueAction = new ConcurrentQueue<Action>();
        private bool threadStarted = false;
        // Use with caution.  When using this you must handle any thrown exceptions.   Any unhandled exceptions will generally crash the running application.
        private void QueueThread(Action work)
        {
            _theQueueAction.Enqueue(work);

            if (threadStarted == false)
            {
                threadStarted = true;
                var t = new Thread(DoThreadness);
                t.Start();
            }
        }

        private void DoThreadness()
        {
            while (true)
            {
                const int halfSecond = 500;
                const int twoSecond = 2000;
                var totalTimeEmpty = 0;
                while (_theQueueAction.IsEmpty)
                {
                    var timeAsleep = halfSecond;
                    Thread.Sleep(timeAsleep);
                    totalTimeEmpty += timeAsleep;

                    if (totalTimeEmpty >= twoSecond)
                    {
                        threadStarted = false;
                        return;
                    }
                }

                while (_theQueueAction.TryDequeue(out Action work))
                {
                    try
                    {
                        work();
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                    }
                }

            }
        }
    }
}
