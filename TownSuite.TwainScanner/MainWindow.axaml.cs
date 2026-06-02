using Avalonia.Controls;
using Avalonia.Input;
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
        private string? _userImageType;
        private string? _userScanner;
        private readonly Ocr _ocr;
        private readonly string _dirText;
        private ScannerBackends[]? _backends;

        // Thumbnail tracking ─ keeps card reference for delete / OCR update
        private record ThumbnailEntry(string FilePath, Border Card, TextBlock PageLabel);
        private readonly List<ThumbnailEntry> _pages = new();

        public MainWindow(List<string> scanSettings, Ocr ocr, string dirText)
        {
            _scanSettings = scanSettings;
            _ocr          = ocr;
            _dirText      = dirText;
            InitializeComponent();
        }

        protected override async void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            try
            {
                cmbColor.ItemsSource    = ScanColors.GetColors();
                cmbColor.SelectedIndex  = 2;

                cmbResolution.ItemsSource   = ScanDPIs.GetDPI();
                cmbResolution.SelectedIndex = 3;

                cmbImageType.ItemsSource   = ScanImageFormats.GetImageFormats();
                cmbImageType.SelectedIndex = 3;

                for (int i = 0; i < _scanSettings.Count; i++)
                {
                    switch (i)
                    {
                        case 2: _userImageType = _scanSettings[i]; break;
                        case 3: _userScanner   = _scanSettings[i]; break;
                    }
                }

                await LoadBackends();
                _backends![0].DeleteFiles();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"{ex.Message}\n\n{ex.StackTrace}", I18N.GetString("LoadError"));
            }

            checkboxOcr.IsVisible = _ocr.Enabled;
            UpdatePageCount();
        }

        // ── Keyboard shortcuts ────────────────────────────────────────────────

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                BtnScan_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Key == Key.S && e.KeyModifiers == KeyModifiers.Meta   // macOS ⌘S
                  || e.Key == Key.S && e.KeyModifiers == KeyModifiers.Control) // Win/Linux Ctrl+S
            {
                if (mnuSave.IsEnabled)
                    MenuItem_Save_Click(sender, e);
                e.Handled = true;
            }
        }

        // ── Menu / button handlers ────────────────────────────────────────────

        private void MenuItem_Exit_Click(object? sender, RoutedEventArgs e) => Close();

        private void MenuItem_Acquire_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var backend = GetSelectedBackend();
                if (backend == null) return;
                _ = backend.Scan(GetSelectedImageFormat());
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync(ex.Message, I18N.GetString("ScanningError"));
            }
        }

        private void MenuItem_Save_Click(object? sender, RoutedEventArgs e)
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

        private async void BtnScan_Click(object? sender, RoutedEventArgs e)
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
                var backend = GetSelectedBackend();
                if (backend != null)
                    await backend.Scan(GetSelectedImageFormat());
            }
            catch (NAPS2.Scan.Exceptions.ScanDriverUnknownException)
            {
                // TWAIN worker can crash after the last page; images are still saved
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

        private void SourceListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e) { }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (_backends != null)
                foreach (var b in _backends) b.Dispose();
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private string GetSelectedImageFormat() =>
            (cmbImageType.SelectedItem as ScanImageFormats)?.Name ?? "JPEG";

        private ScannerBackends? GetSelectedBackend()
        {
            if (_backends == null) return null;
            var device = sourceListBox.SelectedItem as ScanDevice;
            if (device == null) return null;
            return _backends.FirstOrDefault(b =>
                string.Equals(b.GetBackendType(), device.Driver.ToString(),
                              StringComparison.OrdinalIgnoreCase));
        }

        private async Task LoadBackends()
        {
            SetScanningStatus(true, "Loading scanner list…");

            _backends = Array.ConvertAll(
                NewScannerList.PlatformDrivers(),
                d => (ScannerBackends)new Naps2Backend(_dirText, _ocr, d) { ParentView = this });

            foreach (var b in _backends)
                await b.ConfigureSettings();

            foreach (ScanImageFormats fmt in cmbImageType.Items)
            {
                if (string.Equals(fmt.Name?.Trim(), _userImageType?.Trim(),
                                  StringComparison.OrdinalIgnoreCase))
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

        private void UpdatePageCount()
        {
            int n = _pages.Count;
            pageCountLabel.Text        = n == 1 ? "1 page scanned" : $"{n} pages scanned";
            Title                      = n == 0 ? "Scan Document"
                                                : $"Scan Document — {n} page{(n == 1 ? "" : "s")}";
            mnuSave.IsEnabled          = n > 0;
            emptyThumbnailOverlay.IsVisible = n == 0;
        }

        private void DeletePage(ThumbnailEntry entry)
        {
            _pages.Remove(entry);
            thumbnailPanel.Children.Remove(entry.Card);
            try { File.Delete(entry.FilePath); } catch { /* file may already be gone */ }

            // Renumber the remaining pages
            for (int i = 0; i < _pages.Count; i++)
                _pages[i].PageLabel.Text = $"Page {i + 1}";

            UpdatePageCount();
        }

        // ── IMainView ─────────────────────────────────────────────────────────

        public void AddThumbnail(string filePath, Bitmap thumbnail)
        {
            int pageNumber = _pages.Count + 1;

            var image = new Image
            {
                Source  = thumbnail,
                Width   = 180,
                Height  = 180,
                Stretch = Stretch.Uniform
            };

            // ✕ delete button overlaid on top-right of image
            var deleteBtn = new Button
            {
                Content             = "✕",
                Padding             = new Avalonia.Thickness(4, 1),
                FontSize            = 11,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment   = VerticalAlignment.Top,
                Margin              = new Avalonia.Thickness(0, 2, 2, 0),
                Opacity             = 0.85
            };

            var imageGrid = new Grid();
            imageGrid.Children.Add(image);
            imageGrid.Children.Add(deleteBtn);

            // Page label below image
            var pageLabel = new TextBlock
            {
                Text                = $"Page {pageNumber}",
                TextAlignment       = Avalonia.Media.TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin              = new Avalonia.Thickness(4, 3, 4, 4),
                FontSize            = 12
            };

            var innerStack = new StackPanel();
            innerStack.Children.Add(imageGrid);
            innerStack.Children.Add(pageLabel);

            var card = new Border
            {
                BorderThickness = new Avalonia.Thickness(1),
                BorderBrush     = Brushes.Transparent,
                CornerRadius    = new Avalonia.CornerRadius(6),
                Margin          = new Avalonia.Thickness(4),
                Padding         = new Avalonia.Thickness(0),
                Background      = new SolidColorBrush(Color.FromRgb(245, 245, 248)),
                Child           = innerStack
            };

            ToolTip.SetTip(card, filePath);

            // Hover highlight
            image.PointerEntered += (_, _) => card.BorderBrush = Brushes.SteelBlue;
            image.PointerExited  += (_, _) => card.BorderBrush = Brushes.Transparent;

            // Double-click opens the file
            image.DoubleTapped += (_, _) =>
            {
                if (File.Exists(filePath))
                    Process.Start(new ProcessStartInfo { FileName = filePath, UseShellExecute = true });
            };

            var entry = new ThumbnailEntry(filePath, card, pageLabel);
            _pages.Add(entry);

            // Wire delete after entry is created so the closure captures it
            deleteBtn.Click += (_, _) => DeletePage(entry);

            thumbnailPanel.Children.Add(card);
            UpdatePageCount();
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
            var entry = _pages.FirstOrDefault(p => p.FilePath == filePath);
            if (entry == null) return;
            ToolTip.SetTip(entry.Card, ocrText);
            entry.Card.Background = new SolidColorBrush(Color.FromRgb(210, 235, 210));
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
                Text        = message,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            });
            var btn = new Button { Content = "OK", HorizontalAlignment = HorizontalAlignment.Center };
            panel.Children.Add(btn);
            dialog.Content = panel;
            btn.Click += (_, _) => dialog.Close();
            await dialog.ShowDialog(this);
        }

        public int GetResolutionDpi() =>
            (cmbResolution.SelectedItem as ScanDPIs)?.DPI ?? 300;

        public ScanDevice? GetSelectedDevice() =>
            sourceListBox.SelectedItem as ScanDevice;

        public List<ScanDevice> GetDeviceList()
        {
            if (sourceListBox.ItemsSource is List<ScanDevice> list) return list;
            return new List<ScanDevice>();
        }

        public void SetDeviceList(List<ScanDevice> devices, bool append = false)
        {
            var combined = append
                ? GetDeviceList().Concat(devices).ToList()
                : devices;

            sourceListBox.ItemsSource = combined;
            if (combined.Count > 0 && sourceListBox.SelectedIndex < 0)
                sourceListBox.SelectedIndex = 0;

            // Update scanner count badge and empty-state overlay
            int total = combined.Count;
            scannerCountLabel.Text    = total > 0 ? $"({total} found)" : "";
            noScannersOverlay.IsVisible = total == 0;
        }

        public bool IsOcrChecked => checkboxOcr.IsChecked == true;
    }

    // ── Data models ───────────────────────────────────────────────────────────

    internal class ScanDPIs
    {
        public int    DPI  { get; set; }
        public string Name { get; set; } = "";
        public ScanDPIs(int dpi, string name) { DPI = dpi; Name = name; }

        public static List<ScanDPIs> GetDPI() => new()
        {
            new(100, "100 DPI"), new(150, "150 DPI"), new(200, "200 DPI"),
            new(300, "300 DPI"), new(600, "600 DPI"), new(900, "900 DPI"),
            new(1200, "1200 DPI"),
        };
    }

    internal class ScanColors
    {
        public int    Color { get; set; }
        public string Name  { get; set; } = "";
        public ScanColors(int color, string name) { Color = color; Name = name; }

        public static List<ScanColors> GetColors() => new()
        {
            new(0, "Black and White"), new(1, "Gray Scale"), new(2, "Color"),
        };
    }

    internal class ScanImageFormats
    {
        public string Name        { get; set; } = "";
        public string Description { get; set; } = "";
        public ScanImageFormats(string name, string description) { Name = name; Description = description; }

        public static List<ScanImageFormats> GetImageFormats() => new()
        {
            new("TIFF", "TIFF"), new("PDF", "PDF"),
            new("PNG",  "PNG"),  new("JPEG", "JPEG"),
        };
    }
}
