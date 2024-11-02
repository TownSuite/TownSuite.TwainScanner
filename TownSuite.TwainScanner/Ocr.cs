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

        public void GetText(string filepath,
            PictureBox newpic,
            ToolStripProgressBar toolStrip,
            ToolStripStatusLabel statusLabel)
        {
            QueueThread(async () =>
            {
                try
                {
                    newpic.Invoke((MethodInvoker)delegate
                    {
                        // Running on the UI thread
                        toolStrip.Style = ProgressBarStyle.Marquee;
                        toolStrip.Visible = true;
                        statusLabel.Visible = true;
                    });

                    var client = new WebClient();
                    client.Headers.Add("Authorization", $"Bearer {_bearerToken}");
                    client.Headers.Add("User-Agent", "TownSuiteScanner/1.0 Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:109.0) Gecko/20100101 Firefox/113.0");

#if DEBUG
                    if (ServicePointManager.ServerCertificateValidationCallback == null)
                    {
                        ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                    }
#endif

                    byte[] byteImage;
                    using (var bmp = Bitmap.FromFile(filepath))
                    {
                        // sheet of paper hack 2550 pixels wide (300 pixels/inch * 8.5 inches) and. 3300 pixels tall (300 pixels/inch * 11 inches)
                        if (bmp.Width > 2550 && bmp.Height > 3300)
                        {
                            using (var resizedImg = new Bitmap(bmp, new Size(2550, 3300)))
                            {
                                byteImage = ImageToByte(resizedImg);
                            }
                        }
                        else if (!string.Equals(System.IO.Path.GetExtension(filepath), ".jpg", StringComparison.InvariantCultureIgnoreCase))
                        {
                            byteImage = ImageToByte(bmp);
                        }
                        else
                        {
                            byteImage = System.IO.File.ReadAllBytes(filepath);
                        }
                    }
;
                    var output = await client.UploadDataTaskAsync(_apiUrl, byteImage);

                    string text = System.Text.Encoding.UTF8.GetString(output);
                    newpic.Invoke((MethodInvoker)delegate
                    {
                        // Running on the UI thread
                        var tt = new ToolTip();
                        tt.SetToolTip(newpic, text);
                        System.IO.File.WriteAllText($"{filepath}.txt", text);
                        newpic.BackColor = Color.Gray;
                        toolStrip.Style = ProgressBarStyle.Blocks;
                        toolStrip.Visible = false;
                        statusLabel.Visible = false;
                    });
                }
                catch (Exception ex)
                {
                    newpic.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show(ex.Message, I18N.GetString("OcrError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });

        }

        public static byte[] ImageToByte(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return stream.ToArray();
            }
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
