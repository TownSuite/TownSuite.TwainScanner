using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Collections;

using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Media;
using iTextSharp.text;
using iTextSharp.text.pdf;
using TownSuite.TwainScanner.Backends;
using System.Threading.Tasks;
namespace TownSuite.TwainScanner
{
    internal partial class MainFrame : Form
    {
        private List<String> lstscansettings;
        private string UserTwainImageType;
        private string UserTwainScanner;
        readonly Ocr ocr;
        ScannerBackends backend;
        readonly string dirText;


        public MainFrame(List<String> lstScanSet, Ocr ocr, string dirText, string backend)
        {
            lstscansettings = lstScanSet;
            this.ocr = ocr;
            this.dirText = dirText;

            InitializeComponent();

            // this.backend = GetBackend(backend);
            //this.backend.DeleteFiles();

        }

        private void MainFrame_Load(object sender, EventArgs e)
        {


        }

        private async void MainFrame_Shown(object sender, EventArgs e)
        {
            try
            {
                await ChangeBackend();
                this.backend.DeleteFiles();

                for (int i = 0; i <= lstscansettings.Count - 1; i++)
                {
                    switch (i)
                    {
                        case 2:
                            UserTwainImageType = lstscansettings[i];
                            break;
                        case 3:
                            UserTwainScanner = lstscansettings[i];
                            break;
                    }
                }

            }
#if INCLUDE_ORIGINAL
            catch (TwainException ex)
            {
                if (ex.Message == "It worked!")
                {
                    Console.WriteLine("Failed to find a scanner");
                    Console.Out.Flush();
                    this.Close();
                    return;
                }
                throw;
            }
#endif
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace), I18N.GetString("LoadError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Set Default Twain Scanner Settings
            cmbTwainImageType.SelectedItem = UserTwainImageType;
            sourceTwianListBox.SelectedItem = UserTwainScanner;
            //cmbTwainImageType.SelectedIndex = 0;

            checkBoxTwainOcr.Visible = ocr.Enabled;
            checkboxWiaOcr.Visible = ocr.Enabled;
        }

        #region Delete Temporary Files



        #endregion

        private void MenuItem5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region WIA Drivers

        private async void btnWIAScan_Click(object sender, EventArgs e)
        {
            //Start Scanning using a Thread
            //Task.Factory.StartNew(StartScanning).ContinueWith(result => TriggerScan());
            try
            {
                btnWIAScan.Enabled = false;
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

                string imageFormat = GetSelectedWiaImageFormat();
                await backend.Scan(imageFormat);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, I18N.GetString("ScanningError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
               btnWIAScan.Enabled = true;
            }

        }
        #endregion


        private void mnuAcquire_Click(object sender, EventArgs e)
        {
            try
            {
                string imageFormat = GetSelectedTwainImageFormat().ToLower().Trim();
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
            this.backend.ChangeSelectedScanner(GetWiaSourceList());
        }

        private void SourceTwianListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            //this._twain.SetDefaultSource(sourceTwianListBox.SelectedIndex);
            if (sourceTwianListBox.SelectedIndex >= 0)
            {
                this.backend.ChangeSelectedScanner(sourceTwianListBox.SelectedIndex);
            }
        }

        private void MainFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            backend?.Dispose();
        }

        private string GetSelectedTwainImageFormat()
        {
            string format = cmbTwainImageType.SelectedItem.ToString().ToLower();
            return format;
        }

        private string GetSelectedWiaImageFormat()
        {
            string format = cmbImageType.SelectedItem.ToString();
            return format;
        }

        private async void ButtonTwainScan_Click(object sender, EventArgs e)
        {
            try
            {
                buttonTwainScan.Enabled = false;
                if (sourceTwianListBox.SelectedItem == null)
                {
                    MessageBox.Show(I18N.GetString("SelectScanner"), I18N.GetString("ScanDocument"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (cmbTwainImageType.SelectedItem == null)
                {
                    MessageBox.Show(I18N.GetString("SelectImageType"), I18N.GetString("ScanDocument"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                await backend.Scan(GetSelectedTwainImageFormat());
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
                buttonTwainScan.Enabled = true;
            }
        }

        private ScannerBackends GetBackend(string backend)
        {
            switch (backend)
            {
                case "twain":
                    //return new OriginalTwainBackend(dirText, ocr)
                    //{
                    //    ParentForm = this
                    //};
                    return new Naps2Backend(dirText, ocr, NAPS2.Scan.Driver.Twain)
                    {
                        ParentForm = this
                    };
                case "wia":
                    return new Naps2Backend(dirText, ocr, NAPS2.Scan.Driver.Wia)
                    {
                        ParentForm = this
                    };
                default:
                    return new Naps2Backend(dirText, ocr, NAPS2.Scan.Driver.Twain)
                    {
                        ParentForm = this
                    };
            }
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

        private async Task ChangeBackend()
        {
            backend?.Dispose();

            ChangeStatus("Loading scanner list", true);
            switch (tabScanDrivers.SelectedTab.Name)
            {
                case "tpTWAINScan":
                    backend = GetBackend("twain");
                    await backend.ConfigureSettings();
                    cmbTwainImageType.SelectedItem = UserTwainImageType;
                    sourceTwianListBox.SelectedItem = UserTwainScanner;
                    break;
                case "tpWIAScan":
                    backend = GetBackend("wia");
                    //WIA Settings
                    await backend.ConfigureSettings();
                    cmbImageType.SelectedIndex = 0;
                    //cmbColor.SelectedIndex = 0;

                    //cmbColor.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbResolution.DropDownStyle = ComboBoxStyle.DropDownList;

                    break;
            }
            ChangeStatus("", false);

            checkBoxTwainOcr.Visible = ocr.Enabled;
            checkboxWiaOcr.Visible = ocr.Enabled;
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

        public ListBox GetWiaSourceList()
        {
            return sourceListBox;
        }

        public ComboBox GetWiaResolution()
        {
            return cmbResolution;
        }

        public ComboBox GetWiaColor()
        {
            return cmbColor;
        }

        public FlowLayoutPanel GetFlowLayoutPanel()
        {
            return flowLayoutPanel1;
        }

        public ListBox GetTwainSourceList()
        {
            return sourceTwianListBox;
        }


        #endregion

        private async void tabScanDrivers_SelectedIndexChanged(object sender, EventArgs e)
        {
            await ChangeBackend();
        }

    }
}
