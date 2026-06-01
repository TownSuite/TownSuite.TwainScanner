using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using NAPS2.Scan;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TownSuite.TwainScanner.Backends;

namespace TownSuite.TwainScanner
{
    internal partial class MainWindow : Window, IMainView
    {
        public MainWindow() { InitializeComponent(); } // required by Avalonia XAML tooling

        private readonly List<string> _scanSettings;
        private string _userImageType;
        private string _userScanner;
        private readonly Ocr _ocr;
        private readonly string _dirText;
        private ScannerBackends[] _backends;
        private readonly Dictionary<string, Border> _thumbnailBorders = new();

        public MainWindow(List<string> scanSettings, Ocr ocr, string dirText)
        {
            _scanSettings = scanSettings;
            _ocr = ocr;
            _dirText = dirText;
            InitializeComponent();
        }

        protected override async void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            try
            {
                cmbColor.ItemsSource = ScanColors.GetColors();
                cmbColor.SelectedIndex = 2;

                cmbResolution.ItemsSource = ScanDPIs.GetDPI();
                cmbResolution.SelectedIndex = 3;

                cmbImageType.ItemsSource = ScanImageFormats.GetImageFormats();
                cmbImageType.SelectedIndex = 3;

                for (int i = 0; i < _scanSettings.Count; i++)
                {
                    switch (i)
                    {
                        case 2: _userImageType = _scanSettings[i]; break;
                        case 3: _userScanner = _scanSettings[i]; break;
                    }
                }

                await LoadBackends();
                _backends[0].DeleteFiles();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"{ex.Message}\n\n{ex.StackTrace}", I18N.GetString("LoadError"));
            }

            checkboxOcr.IsVisible = _ocr.Enabled;
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e) => Close();

        private void MenuItem_Acquire_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var backend = GetSelectedBackend();
                if (backend == null) return;
                string fmt = GetSelectedImageFormat();
                _ = backend.Scan(fmt);
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync(ex.Message, I18N.GetString("ScanningError"));
            }
        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetSelectedBackend()?.Save();
                Close();
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync(I18N.GetString("ErrorOccured") + "\r\n" + ex.Message,
                    I18N.GetString("ScanDocument"));
            }
        }

        private async void BtnScan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnScan.IsEnabled = false;
                if (sourceListBox.SelectedItem == null)
                {
                    await ShowErrorAsync(I18N.GetString("SelectScanner"), I18N.GetString("ScanDocument"));
                    return;
                }
                if (cmbImageType.SelectedItem == null)
                {
                    await ShowErrorAsync(I18N.GetString("SelectImageType"), I18N.GetString("ScanDocument"));
                    return;
                }

                string fmt = GetSelectedImageFormat();
                var backend = GetSelectedBackend();
                if (backend != null)
                    await backend.Scan(fmt);
            }
            catch (NAPS2.Scan.Exceptions.ScanDriverUnknownException)
            {
                // TWAIN worker can crash on last page; images are still scanned
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex.Message, I18N.GetString("ScanningError"));
            }
            finally
            {
                btnScan.IsEnabled = true;
            }
        }

        private void SourceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var backend = GetSelectedBackend();
            // backends don't currently use the change notification
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (_backends != null)
                foreach (var b in _backends)
                    b.Dispose();
        }

        private string GetSelectedImageFormat() =>
            (cmbImageType.SelectedItem as ScanImageFormats)?.Name ?? "JPEG";

        private ScannerBackends GetSelectedBackend()
        {
            if (_backends == null) return null;
            var device = sourceListBox.SelectedItem as ScanDevice;
            if (device == null) return null;
            return _backends.FirstOrDefault(b =>
                string.Equals(b.GetBackendType(), device.Driver.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        private async Task LoadBackends()
        {
            SetScanningStatus(true, "Loading scanner list");

            _backends = Array.ConvertAll(
                NewScannerList.PlatformDrivers(),
                d => (ScannerBackends)new Naps2Backend(_dirText, _ocr, d) { ParentView = this });

            foreach (var b in _backends)
                await b.ConfigureSettings();

            foreach (ScanImageFormats fmt in cmbImageType.Items)
            {
                if (string.Equals(fmt.Name?.Trim(), _userImageType?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    cmbImageType.SelectedItem = fmt;
                    break;
                }
            }

            var devices = GetDeviceList();
            foreach (ScanDevice source in devices)
            {
                if (source.Name?.Trim() == _userScanner?.Trim())
                {
                    sourceListBox.SelectedItem = source;
                    break;
                }
            }

            SetScanningStatus(false);
            checkboxOcr.IsVisible = _ocr.Enabled;
        }

        // IMainView

        public void AddThumbnail(string filePath, Bitmap thumbnail)
        {
            var image = new Image
            {
                Source = thumbnail,
                Width = 180,
                Height = 180,
                Stretch = Stretch.Uniform,
                Tag = filePath
            };

            var border = new Border
            {
                BorderThickness = new Avalonia.Thickness(1),
                BorderBrush = Brushes.Transparent,
                Margin = new Avalonia.Thickness(2),
                Child = image
            };

            ToolTip.SetTip(border, filePath);

            image.DoubleTapped += (_, _) =>
            {
                if (File.Exists(filePath))
                    Process.Start(new ProcessStartInfo { FileName = filePath, UseShellExecute = true });
            };
            image.PointerEntered += (_, _) => border.BorderBrush = Brushes.SteelBlue;
            image.PointerExited += (_, _) => border.BorderBrush = Brushes.Transparent;

            _thumbnailBorders[filePath] = border;
            thumbnailPanel.Children.Add(border);
        }

        public void SetScanningStatus(bool running, string message = "")
        {
            progressBar.IsVisible = running;
            statusLabel.IsVisible = running;
            if (running) statusLabel.Text = message;
        }

        public void SetOcrStatus(bool running, string message = "")
        {
            progressBar.IsVisible = running;
            statusLabel.IsVisible = running;
            if (running) statusLabel.Text = message;
        }

        public void MarkOcrComplete(string filePath, string ocrText)
        {
            if (_thumbnailBorders.TryGetValue(filePath, out var border))
            {
                ToolTip.SetTip(border, ocrText);
                border.Background = new SolidColorBrush(Color.FromRgb(200, 200, 200));
            }
        }

        public async Task ShowErrorAsync(string message, string title)
        {
            var dialog = new Window
            {
                Title = title,
                Width = 440,
                Height = 200,
                CanResize = false,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            var panel = new StackPanel { Margin = new Avalonia.Thickness(16), Spacing = 12 };
            panel.Children.Add(new TextBlock
            {
                Text = message,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            });
            var btn = new Button { Content = "OK", HorizontalAlignment = HorizontalAlignment.Center };
            panel.Children.Add(btn);
            dialog.Content = panel;
            btn.Click += (_, _) => dialog.Close();
            await dialog.ShowDialog(this);
        }

        public int GetResolutionDpi() => (cmbResolution.SelectedItem as ScanDPIs)?.DPI ?? 300;

        public ScanDevice GetSelectedDevice() => sourceListBox.SelectedItem as ScanDevice;

        public List<ScanDevice> GetDeviceList()
        {
            if (sourceListBox.ItemsSource is List<ScanDevice> list)
                return list;
            return new List<ScanDevice>();
        }

        public void SetDeviceList(List<ScanDevice> devices, bool append = false)
        {
            List<ScanDevice> combined;
            if (append)
            {
                combined = GetDeviceList().Concat(devices).ToList();
            }
            else
            {
                combined = devices;
            }
            sourceListBox.ItemsSource = combined;
            if (combined.Count > 0 && sourceListBox.SelectedIndex < 0)
                sourceListBox.SelectedIndex = 0;
        }

        public bool IsOcrChecked => checkboxOcr.IsChecked == true;
    }

    internal class ScanDPIs
    {
        public int DPI { get; set; }
        public string Name { get; set; }
        public ScanDPIs(int dpi, string name) { DPI = dpi; Name = name; }

        public static List<ScanDPIs> GetDPI() => new List<ScanDPIs>
        {
            new ScanDPIs(100, "100 DPI"),
            new ScanDPIs(150, "150 DPI"),
            new ScanDPIs(200, "200 DPI"),
            new ScanDPIs(300, "300 DPI"),
            new ScanDPIs(600, "600 DPI"),
            new ScanDPIs(900, "900 DPI"),
            new ScanDPIs(1200, "1200 DPI"),
        };
    }

    internal class ScanColors
    {
        public int Color { get; set; }
        public string Name { get; set; }
        public ScanColors(int color, string name) { Color = color; Name = name; }

        public static List<ScanColors> GetColors() => new List<ScanColors>
        {
            new ScanColors(0, "Black and White"),
            new ScanColors(1, "Gray Scale"),
            new ScanColors(2, "Color"),
        };
    }

    internal class ScanImageFormats
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ScanImageFormats(string name, string description) { Name = name; Description = description; }

        public static List<ScanImageFormats> GetImageFormats() => new List<ScanImageFormats>
        {
            new ScanImageFormats("TIFF", "TIFF"),
            new ScanImageFormats("PDF",  "PDF"),
            new ScanImageFormats("PNG",  "PNG"),
            new ScanImageFormats("JPEG", "JPEG"),
        };
    }
}
