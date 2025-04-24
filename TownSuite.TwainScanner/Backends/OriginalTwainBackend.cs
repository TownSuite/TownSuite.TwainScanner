#if INCLUDE_ORIGINAL
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TownSuite.TwainScanner.Backends
{
    class OriginalTwainBackend : ScannerBackends
    {
        private Twain32 _twain;

        public OriginalTwainBackend(string dirText, Ocr ocr) : base(dirText, ocr)
        {
        }

        public override async Task ConfigureSettings()
        {
            await base.ConfigureSettings();

            _twain = new Twain32();
            this._twain.OpenDSM();
            this._twain.AcquireCompleted += new System.EventHandler(this._twain_AcquireCompleted);

            LoadTwainDrivers();
        }

        public override void ChangeSelectedScanner(object value)
        {
            var sourceTwainListBox = ParentForm.GetTwainSourceList();
            if (sourceTwainListBox.SelectedIndex >= 0)
            {
                this._twain.SourceIndex = Convert.ToInt32(value);
            }
        }

        #region Load Twian Drivers

        public Twain32 Twain
        {
            get;
            set;
        }

        private void LoadTwainDrivers()
        {
            try
            {
                var sourceTwainListBox = ParentForm.GetTwainSourceList();
                sourceTwainListBox.Items.Clear();
                //this._twain.CloseDataSource();
                //this._twain.SelectSource();
                Twain = this._twain;

                //if (sourceTwainListBox.Items.Count == 0) { 
                if (this.Twain != null && this.Twain.SourcesCount > 0)
                {
                    for (int i = 0; i < this.Twain.SourcesCount; i++)
                    {
                        bool twn2 = this.Twain.GetIsSourceTwain2Compatible(i);
                        sourceTwainListBox.Items.Add(this.Twain.GetSourceProductName(i));
                    }
                    sourceTwainListBox.SelectedIndex = this.Twain.SourceIndex;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        public override Task Scan(string imageFormat)
        {
            base.Scan(imageFormat);

            this._twain.Acquire();
            return Task.CompletedTask;
        }

        int picnumber = 0;
        private void _twain_AcquireCompleted(object sender, EventArgs e)
        {
            try
            {
                if (this._twain.ImageCount > 0)
                {
                    for (int i = 0; i <= this._twain.ImageCount - 1; i += 1)
                    {
                        picnumber += 1;
                        var newpic = new PictureBox();
                        System.Drawing.Image resizedImg;
                        string origPath = Path.Combine(DirText, "tmpScan" + picnumber.ToString() + "_" + i.ToString() + Guid.NewGuid().ToString() + imageExtension);
                        newpic.Tag = origPath;
                        using (var img = this._twain.GetImage(i))
                        {
                            switch (imageFormat)
                            {
                                case "tiff":
                                    img.Save(origPath, ImageFormat.Tiff);
                                    break;
                                case "png":
                                    img.Save(origPath, ImageFormat.Png);
                                    break;
                                case "pdf":
                                case "jpeg":
                                default:
                                    // pdf is just an import of a file.  Use jpg.
                                    img.Save(origPath, ImageFormat.Jpeg);
                                    break;
                            }

                            resizedImg = new Bitmap(img, new Size(180, 180));
                        }

                        newpic.Image = resizedImg;
                        newpic.Size = new Size(newpic.Image.Width, newpic.Image.Height);
                        newpic.Refresh();
                        newpic.DoubleClick += Newpic_DoubleClick;
                        newpic.MouseEnter += Newpic_MouseEnter;
                        newpic.MouseLeave += Newpic_MouseLeave;
                        var flowLayoutPanel1 = ParentForm.GetFlowLayoutPanel();
                        newpic.Text = "ScanPass" + picnumber.ToString() + "_Pic" + picnumber.ToString();
                        flowLayoutPanel1.Controls.Add(newpic);
                        // newpic.doTmpSave(DirText + "\\tmpScan" + picnumber.ToString() + "_" + i.ToString() + ".bmp");

                        RunOcr(newpic, origPath, OcrEnabled());
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, I18N.GetString("TownSuiteScan"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            this._twain?.CloseDataSource();
            this._twain?.CloseDSM();
            this._twain.Dispose();
        }
    }
}
#endif