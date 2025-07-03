using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TownSuite.TwainScanner.Backends;
using System.Threading.Tasks;
using NAPS2.Scan;
using System.Linq;
namespace TownSuite.TwainScanner
{
    internal partial class MainFrame : Form
    {
        private List<String> lstscansettings;
        private string UserImageType;
        private string UserScanner;
        readonly Ocr ocr;
        ScannerBackends[] backends;
        readonly string dirText;



        public MainFrame(List<String> lstScanSet, Ocr ocr, string dirText)
        {
            lstscansettings = lstScanSet;
            this.ocr = ocr;
            this.dirText = dirText;

            InitializeComponent();
        }

        private async void MainFrame_Shown(object sender, EventArgs e)
        {
            try
            {
                cmbColor.DataSource = Colors.GetColors();
                cmbColor.DisplayMember = "Name";
                cmbColor.ValueMember = "Color";
                cmbColor.SelectedIndex = 2;

                cmbResolution.DataSource = DPIs.GetDPI();
                cmbResolution.DisplayMember = "Name";
                cmbResolution.ValueMember = "DPI";
                cmbResolution.SelectedIndex = 3;

                cmbImageType.DataSource = ImageFormats.GetImageFormats();
                cmbImageType.DisplayMember = "Description";
                cmbImageType.ValueMember = "Name";
                cmbImageType.SelectedIndex = 3;


                for (int i = 0; i <= lstscansettings.Count - 1; i++)
                {
                    switch (i)
                    {
                        case 2:
                            UserImageType = lstscansettings[i];
                            break;
                        case 3:
                            UserScanner = lstscansettings[i];
                            break;
                    }
                }
                await LoadBackends();
                this.backends[0].DeleteFiles();



            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace), I18N.GetString("LoadError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            checkboxOcr.Visible = ocr.Enabled;
        }

        private void MenuItem5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnScan_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                btnScan.Enabled = false;
                if (sourceListBox.SelectedItem == null)
                {
                    MessageBox.Show(I18N.GetString("SelectScanner"), I18N.GetString("ScanDocument"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (cmbImageType.SelectedItem == null)
                {
                    MessageBox.Show(I18N.GetString("SelectImageType"), I18N.GetString("ScanDocument"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string imageFormat = GetSelectedImageFormat();
                var backend = GetSelectedBackend();
                await backend.Scan(imageFormat);
            }
            catch (NAPS2.Scan.Exceptions.ScanDriverUnknownException)
            {
                // FIXME: what to do here?  the images seem to be scanning fine but the twian worker crashes when the last image was scanned
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, I18N.GetString("ScanningError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnScan.Enabled = true;
            }
        }

        private void mnuAcquire_Click(object sender, EventArgs e)
        {
            try
            {
                var backend = GetSelectedBackend();
                string imageFormat = GetSelectedImageFormat().ToLower().Trim();
                backend.Scan(imageFormat);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, I18N.GetString("ScanningError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Save Scan Files

        private void mnuSave_Click(object sender, EventArgs e)
        {
            try
            {
                var backend = GetSelectedBackend();
                backend.Save();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(I18N.GetString("ErrorOccured") + "\r\n" + ex.Message,
                    I18N.GetString("ScanDocument"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        private void SourceListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            // wia
            var backend = GetSelectedBackend();
            backend.ChangeSelectedScanner(GetSourceList());
            var device = sourceListBox.SelectedItem as ScanDevice;
        }

        private void MainFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var scanner in backends)
            {
                scanner.Dispose();
            }
        }

        private string GetSelectedImageFormat()
        {
            string format = (cmbImageType.SelectedItem as ImageFormats).Name;
            return format;
        }

        private ScannerBackends CreateBackend(string backend)
        {
           
            return new Naps2Backend(dirText, ocr, NewScannerList.GetDriver(backend))
            {
                ParentForm = this
            };

        }

        private ScannerBackends GetSelectedBackend()
        {
            var device = sourceListBox?.SelectedItem as ScanDevice;
            if (device == null) return null;

            var backend = backends.FirstOrDefault(b => string.Equals(b.GetBackendType(), device.Driver.ToString(), StringComparison.OrdinalIgnoreCase));

            return backend;
        }

        private void ChangeStatus(string message, bool visible)
        {
            toolStripStatusLabel1.Text = message;
            if (visible)
            {
                toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                toolStripProgressBar1.Style = ProgressBarStyle.Blocks;
            }
            toolStripProgressBar1.Visible = visible;
            toolStripStatusLabel1.Visible = visible;
        }

        private async Task LoadBackends()
        {
            ChangeStatus("Loading scanner list", true);

            var twainBackend = CreateBackend("twain");
            var wiaBackend = CreateBackend("wia");

            backends = new[] { twainBackend, wiaBackend };

            await twainBackend.ConfigureSettings();
            await wiaBackend.ConfigureSettings();

            cmbImageType.SelectedItem = UserImageType;
            foreach (ImageFormats imgformat in cmbImageType.Items)
            {
                if (string.Equals(imgformat.Name?.Trim(), UserImageType?.Trim(),StringComparison.OrdinalIgnoreCase))
                {
                    cmbImageType.SelectedItem = imgformat;
                    break;
                }
            }

            foreach (ScanDevice source in sourceListBox.Items)
            {
                if (source.Name?.Trim() == UserScanner?.Trim())
                {
                    sourceListBox.SelectedItem = source;
                    break;
                }
            }

            ChangeStatus("", false);

            checkboxOcr.Visible = ocr.Enabled;
        }

        #region "helper methods to help refactor the source code"
        public ToolStripProgressBar GetProgressBar()
        {
            return toolStripProgressBar1;
        }

        public ToolStripStatusLabel GetStatusLabel()
        {
            return toolStripStatusLabel1;
        }

        public ListBox GetSourceList()
        {
            return sourceListBox;
        }

        public ComboBox GetResolution()
        {
            return cmbResolution;
        }

        public ComboBox GetColor()
        {
            return cmbColor;
        }

        public FlowLayoutPanel GetFlowLayoutPanel()
        {
            return flowLayoutPanel1;
        }

        #endregion

    }

    internal class DPIs
    {
        public int DPI { get; set; }
        public string Name { get; set; }
        public DPIs(int dpi, string name)
        {
            DPI = dpi;
            Name = name;
        }

        public static List<DPIs> GetDPI()
        {
            return new List<DPIs>
            {
                new DPIs(100, "100 DPI"),
                new DPIs(150, "150 DPI"),
                new DPIs(200, "200 DPI"),
                new DPIs(300, "300 DPI"),
                new DPIs(600, "600 DPI"),
                new DPIs(900, "900 DPI"),
                new DPIs(1200, "1200 DPI"),
            };
        }
    }

    internal class Colors
    {
        public int Color { get; set; }
        public string Name { get; set; }
        public Colors(int color, string name)
        {
            Color = color;
            Name = name;
        }

        public static List<Colors> GetColors()
        {
            return new List<Colors>
            {
                new Colors(0, "Black and White"),
                new Colors(1, "Gray Scale"),
                new Colors(2, "Color")
            };
        }
    }

    internal class ImageFormats
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ImageFormats(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public static List<ImageFormats> GetImageFormats()
        {
            return new List<ImageFormats>
            {
                new ImageFormats("TIFF", "TIFF"),
                new ImageFormats("PDF", "PDF"),
                new ImageFormats("PNG", "PNG"),
                new ImageFormats("JPEG", "JPEG")
            };
        }
    }
}
