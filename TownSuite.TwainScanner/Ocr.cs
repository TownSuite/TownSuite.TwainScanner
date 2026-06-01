using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TownSuite.TwainScanner
{
    internal class Ocr
    {
        public bool Enabled { get; set; }
        private readonly string _apiUrl;
        private readonly string _bearerToken;

        public Ocr(bool enabled, string apiUrl, string bearerToken)
        {
            Enabled = enabled;
            _apiUrl = apiUrl;
            _bearerToken = bearerToken;
        }

        public void GetText(
            string filepath,
            Action onOcrStarting,
            Action<string, string> onOcrComplete,
            Action<string, Exception> onOcrError)
        {
            QueueThread(async () =>
            {
                string text = string.Empty;
                try
                {
                    onOcrStarting?.Invoke();

                    using var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_bearerToken}");
                    client.DefaultRequestHeaders.Add("User-Agent",
                        "TownSuiteScanner/1.0 Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                    byte[] byteImage;
                    using (var bmp = (Bitmap)Bitmap.FromFile(filepath))
                    {
                        if (bmp.Width > 2550 && bmp.Height > 3300)
                        {
                            using var resized = new Bitmap(bmp, new System.Drawing.Size(2550, 3300));
                            byteImage = ImageToByte(resized);
                        }
                        else if (!string.Equals(Path.GetExtension(filepath), ".jpg", StringComparison.OrdinalIgnoreCase))
                        {
                            byteImage = ImageToByte(bmp);
                        }
                        else
                        {
                            byteImage = File.ReadAllBytes(filepath);
                        }
                    }

                    using var content = new ByteArrayContent(byteImage);
                    var response = await client.PostAsync(_apiUrl, content);
                    response.EnsureSuccessStatusCode();
                    text = await response.Content.ReadAsStringAsync();

                    File.WriteAllText($"{filepath}.txt", text);
                    onOcrComplete?.Invoke(filepath, text);
                }
                catch (Exception ex)
                {
                    onOcrError?.Invoke(filepath, ex);
                }
            });
        }

        public static byte[] ImageToByte(Image img)
        {
            using var stream = new MemoryStream();
            img.Save(stream, ImageFormat.Jpeg);
            return stream.ToArray();
        }

        private readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();
        private bool _threadStarted = false;

        private void QueueThread(Action work)
        {
            _queue.Enqueue(work);

            if (!_threadStarted)
            {
                _threadStarted = true;
                var t = new Thread(DrainQueue);
                t.Start();
            }
        }

        private void DrainQueue()
        {
            while (true)
            {
                int idleMs = 0;
                while (_queue.IsEmpty)
                {
                    Thread.Sleep(500);
                    idleMs += 500;
                    if (idleMs >= 2000)
                    {
                        _threadStarted = false;
                        return;
                    }
                }

                while (_queue.TryDequeue(out var work))
                {
                    try { work(); }
                    catch (Exception ex) { Console.Error.WriteLine(ex); }
                }
            }
        }
    }
}
