using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace TownSuite.TwainScanner
{
    public partial class MainFrame : Form
    {
        private Twain32 _twain;
        string DirText;
        bool saved = false;

        public MainFrame()
        {
            InitializeComponent();
        }

        private void MainFrame_Load(object sender, EventArgs e)
        {
            try
            {
                _twain = new Twain32();
                this._twain.AcquireCompleted += new System.EventHandler(this._twain_AcquireCompleted);
                this._twain.OpenDSM();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace), "SAMPLE1", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            DirText = Environment.GetEnvironmentVariable("TMP");

            GC.Collect();
            GC.WaitForPendingFinalizers();


            //New Stuff
            //---------------------------
            string[] sa = null;
            string s = null;
            sa = Directory.GetFiles(DirText, "tmpscan*.bmp");

            foreach (string s_loopVariable in sa)
            {
                s = s_loopVariable;
                File.Delete(s);
            }
        }

        private void MenuItem5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuSelect_Click(object sender, EventArgs e)
        {
            try
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    using (SelectSourceForm _dlg = new SelectSourceForm { Twain = this._twain })
                    {
                        if (_dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            this._twain.SetDefaultSource(_dlg.SourceIndex);
                            this._twain.SourceIndex = _dlg.SourceIndex;
                        }
                    }
                }
                else
                {
                    this._twain.CloseDataSource();
                    this._twain.SelectSource();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SAMPLE1", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mnuAcquire_Click(object sender, EventArgs e)
        {
            try
            {
                this._twain.Acquire();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SAMPLE1", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        int picnumber = 0;
        private void _twain_AcquireCompleted(object sender, EventArgs e)
        {
            try
            {
                var newpic = new PictureBox();

                if (this._twain.ImageCount > 0)
                {
                    newpic.Image = this._twain.GetImage(0);
                    newpic.Size = new Size(newpic.Image.Width, newpic.Image.Height);
                    newpic.Refresh();
                    flowLayoutPanel1.Controls.Add(newpic);

                    // *************************************

                    picnumber += 1;

                    int i = 0;
                    for (i = 0; i <= this._twain.ImageCount - 1; i += 1)
                    {
                        var img = this._twain.GetImage(i);
                        int picnum = i + 1;
                        newpic.Text = "ScanPass" + picnumber.ToString() + "_Pic" + picnum.ToString();
                        newpic.Image.Save(DirText + @"\tmpScan" + picnumber.ToString() + "_" + i.ToString() + ".bmp", ImageFormat.Bmp);
                        // newpic.doTmpSave(DirText + "\\tmpScan" + picnumber.ToString() + "_" + i.ToString() + ".bmp");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "TownSuite Scan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            string[] sa = null;
            sa = Directory.GetFiles(DirText, "tmpscan*.bmp");

            //get the codec for tiff files
            ImageCodecInfo info = ImageCodecInfo.GetImageEncoders().Where(p => p.MimeType == "image/tiff").FirstOrDefault();

            //use the save encoder
            var enc = System.Drawing.Imaging.Encoder.SaveFlag;
            EncoderParameters ep = new EncoderParameters(1);

            ep.Param[0] = new EncoderParameter(enc, Convert.ToInt64(EncoderValue.MultiFrame));

            Bitmap pages = null;
            int frame = 0;

            foreach (string s in sa)
            {
                if (frame == 0)
                {
                    pages = (Bitmap)Image.FromFile(s);
                    //save the first frame
                    pages.Save(DirText + "\\tmpScan.tif", info, ep);
                }
                else
                {
                    //save the intermediate frames
                    ep.Param[0] = new EncoderParameter(enc, Convert.ToInt64(EncoderValue.FrameDimensionPage));
                    Bitmap bm = (Bitmap)Image.FromFile(s);
                    pages.SaveAdd(bm, ep);
                }
                if (frame == sa.Length - 1)
                {
                    //flush and close.
                    ep.Param[0] = new EncoderParameter(enc, Convert.ToInt64(EncoderValue.Flush));
                    pages.SaveAdd(ep);

                }
                frame += 1;

            }

            saved = true;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void MainFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (saved)
            {
                System.Console.WriteLine("SAVED");
            }
            else
            {
                System.Console.WriteLine("NO IMAGE");
            }
        }
    }
}
