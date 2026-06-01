using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

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
                try
                {
                    onOcrStarting?.Invoke();

                    using var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_bearerToken}");
                    client.DefaultRequestHeaders.Add("User-Agent",
                        "TownSuiteScanner/1.0 Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                    byte[] byteImage = PrepareImageBytes(filepath);

                    using var content = new ByteArrayContent(byteImage);
                    var response = await client.PostAsync(_apiUrl, content);
                    response.EnsureSuccessStatusCode();
                    string text = await response.Content.ReadAsStringAsync();

                    File.WriteAllText($"{filepath}.txt", text);
                    onOcrComplete?.Invoke(filepath, text);
                }
                catch (Exception ex)
                {
                    onOcrError?.Invoke(filepath, ex);
                }
            });
        }

        private static byte[] PrepareImageBytes(string filepath)
        {
            using var img = Image.Load(filepath);

            // Downscale large images (> letter-size at 300 DPI) before sending to OCR
            bool needsResize = img.Width > 2550 || img.Height > 3300;
            if (needsResize)
            {
                using var resized = img.Clone(ctx => ctx.Resize(
                    Math.Min(img.Width,  2550),
                    Math.Min(img.Height, 3300)));
                return EncodeJpeg(resized);
            }

            string ext = Path.GetExtension(filepath).ToLowerInvariant();
            if (ext == ".jpg" || ext == ".jpeg")
                return File.ReadAllBytes(filepath);

            return EncodeJpeg(img);
        }

        private static byte[] EncodeJpeg(Image img)
        {
            using var ms = new MemoryStream();
            img.SaveAsJpeg(ms, new JpegEncoder { Quality = 90 });
            return ms.ToArray();
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
                    if (idleMs >= 2000) { _threadStarted = false; return; }
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
