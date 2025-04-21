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
namespace TownSuite.TwainScanner
{
    internal partial class MainFrame : Form
    {
        private List<String> lstscansettings;
        private string UserTwainImageType;
        private string UserTwainScanner;
        readonly Ocr ocr;
        IBackend backend;
        readonly string dirText;

#if INCLUDE_WIA
        bool removeWia = false;
#else
        bool removeWia = true;
#endif

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

        private void MainFrame_Shown(object sender, EventArgs e)
        {
            try
            {
                if (removeWia)
                {
                    tabScanDrivers.TabPages.RemoveByKey("tpWIAScan");
                }

                ChangeBackend();
                this.backend.DeleteFiles();
                // backend.ConfigureSettings();


            }
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

        private void btnWIAScan_Click(object sender, EventArgs e)
        {
#if INCLUDE_WIA
            //Start Scanning using a Thread
            //Task.Factory.StartNew(StartScanning).ContinueWith(result => TriggerScan());

            string imageFormat = GetSelectedWiaImageFormat();
            backend.Scan(imageFormat);
#endif

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

        //private void SourceTwianListBox_SelectedValueChanged(object sender, EventArgs e)
        //{

        //    //this._twain.SetDefaultSource(sourceTwianListBox.SelectedIndex);
        //    if (sourceTwianListBox.SelectedIndex >= 0)
        //    {
        //        ChangeBackend();
        //    }
        //}
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

        private void ButtonTwainScan_Click(object sender, EventArgs e)
        {
            try
            {
                backend.Scan(GetSelectedTwainImageFormat());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, I18N.GetString("ScanningError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private IBackend GetBackend(string backend)
        {
            switch (backend)
            {
                case "originaltwain":
                    //return new OriginalTwainBackend(dirText, ocr)
                    //{
                    //    ParentForm = this
                    //};
                    return new Naps2Backend(dirText, ocr)
                    {
                        ParentForm = this
                    };
#if INCLUDE_WIA
                case "originalwia":
                    return new OriginalWiaBackend(dirText, ocr)
                    {
                        ParentForm = this
                    };
#endif
                case "naps2":
                    return new Naps2Backend(dirText, ocr)
                    {
                        ParentForm = this
                    };
                default:
                    return new OriginalTwainBackend(dirText, ocr)
                    {
                        ParentForm = this
                    };
            }
        }

        private void ChangeBackend()
        {
            backend?.Dispose();
            switch (tabScanDrivers.SelectedTab.Name)
            {
                case "tpTWAINScan":
                    backend = GetBackend("originaltwain");
                    backend.ConfigureSettings();
                    cmbTwainImageType.SelectedItem = UserTwainImageType;
                    sourceTwianListBox.SelectedItem = UserTwainScanner;
                    break;
                case "tpWIAScan":
                    backend = GetBackend("originalwia");
                    //WIA Settings
                    backend.ConfigureSettings();
                    cmbImageType.SelectedIndex = 0;
                    cmbColor.SelectedIndex = 0;

                    cmbColor.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbResolution.DropDownStyle = ComboBoxStyle.DropDownList;

                    break;
            }

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

        private void tabScanDrivers_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeBackend();
        }

    }
}
