﻿namespace TownSuite.TwainScanner
{
    partial class MainFrame
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainMenu1 = new System.Windows.Forms.MenuStrip();
            this.menuMainFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAcquire = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.sourceListBox = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblResolution = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbImageType = new System.Windows.Forms.ComboBox();
            this.cmbResolution = new System.Windows.Forms.ComboBox();
            this.cmbColor = new System.Windows.Forms.ComboBox();
            this.btnWIAScan = new System.Windows.Forms.Button();
            this.tabScanDrivers = new System.Windows.Forms.TabControl();
            this.tpTWAINScan = new System.Windows.Forms.TabPage();
            this.checkBoxTwainOcr = new System.Windows.Forms.CheckBox();
            this.buttonTwainScan = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbTwainImageType = new System.Windows.Forms.ComboBox();
            this.sourceTwianListBox = new System.Windows.Forms.ListBox();
            this.tpWIAScan = new System.Windows.Forms.TabPage();
            this.checkboxWiaOcr = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainMenu1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabScanDrivers.SuspendLayout();
            this.tpTWAINScan.SuspendLayout();
            this.tpWIAScan.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu1
            // 
            this.MainMenu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMainFile,
            this.mnuAcquire,
            this.mnuSave});
            this.MainMenu1.Location = new System.Drawing.Point(0, 0);
            this.MainMenu1.Name = "MainMenu1";
            this.MainMenu1.Size = new System.Drawing.Size(904, 24);
            this.MainMenu1.TabIndex = 0;
            // 
            // menuMainFile
            // 
            this.menuMainFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem5});
            this.menuMainFile.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuMainFile.MergeIndex = 0;
            this.menuMainFile.Name = "menuMainFile";
            this.menuMainFile.Size = new System.Drawing.Size(37, 20);
            this.menuMainFile.Text = "File";
            // 
            // MenuItem5
            // 
            this.MenuItem5.MergeIndex = 0;
            this.MenuItem5.Name = "MenuItem5";
            this.MenuItem5.Size = new System.Drawing.Size(92, 22);
            this.MenuItem5.Text = "Exit";
            this.MenuItem5.Click += new System.EventHandler(this.MenuItem5_Click);
            // 
            // mnuAcquire
            // 
            this.mnuAcquire.MergeIndex = 1;
            this.mnuAcquire.Name = "mnuAcquire";
            this.mnuAcquire.Size = new System.Drawing.Size(60, 20);
            this.mnuAcquire.Text = "&Acquire";
            this.mnuAcquire.Click += new System.EventHandler(this.mnuAcquire_Click);
            // 
            // mnuSave
            // 
            this.mnuSave.MergeIndex = 2;
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.Size = new System.Drawing.Size(43, 20);
            this.mnuSave.Text = "&Save";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(186, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(710, 593);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // sourceListBox
            // 
            this.sourceListBox.FormattingEnabled = true;
            this.sourceListBox.Location = new System.Drawing.Point(6, 6);
            this.sourceListBox.Name = "sourceListBox";
            this.sourceListBox.Size = new System.Drawing.Size(146, 160);
            this.sourceListBox.TabIndex = 2;
            this.sourceListBox.SelectedValueChanged += new System.EventHandler(this.SourceListBox_SelectedValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblResolution);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmbImageType);
            this.groupBox1.Controls.Add(this.cmbResolution);
            this.groupBox1.Controls.Add(this.cmbColor);
            this.groupBox1.Location = new System.Drawing.Point(8, 170);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(146, 160);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "WIA Properties";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 104);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Color";
            // 
            // lblResolution
            // 
            this.lblResolution.AutoSize = true;
            this.lblResolution.Location = new System.Drawing.Point(6, 62);
            this.lblResolution.Name = "lblResolution";
            this.lblResolution.Size = new System.Drawing.Size(57, 13);
            this.lblResolution.TabIndex = 12;
            this.lblResolution.Text = "Resolution";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Image format";
            // 
            // cmbImageType
            // 
            this.cmbImageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImageType.FormattingEnabled = true;
            this.cmbImageType.Items.AddRange(new object[] {
            "TIFF",
            "PDF",
            "PNG",
            "JPEG"});
            this.cmbImageType.Location = new System.Drawing.Point(8, 34);
            this.cmbImageType.Name = "cmbImageType";
            this.cmbImageType.Size = new System.Drawing.Size(110, 21);
            this.cmbImageType.TabIndex = 7;
            // 
            // cmbResolution
            // 
            this.cmbResolution.FormattingEnabled = true;
            this.cmbResolution.Location = new System.Drawing.Point(8, 78);
            this.cmbResolution.Name = "cmbResolution";
            this.cmbResolution.Size = new System.Drawing.Size(110, 21);
            this.cmbResolution.TabIndex = 2;
            // 
            // cmbColor
            // 
            this.cmbColor.FormattingEnabled = true;
            this.cmbColor.Location = new System.Drawing.Point(8, 120);
            this.cmbColor.Name = "cmbColor";
            this.cmbColor.Size = new System.Drawing.Size(110, 21);
            this.cmbColor.TabIndex = 3;
            // 
            // btnWIAScan
            // 
            this.btnWIAScan.Location = new System.Drawing.Point(76, 336);
            this.btnWIAScan.Name = "btnWIAScan";
            this.btnWIAScan.Size = new System.Drawing.Size(75, 23);
            this.btnWIAScan.TabIndex = 6;
            this.btnWIAScan.Text = "WIA Scan";
            this.btnWIAScan.UseVisualStyleBackColor = true;
            this.btnWIAScan.Click += new System.EventHandler(this.btnWIAScan_Click);
            // 
            // tabScanDrivers
            // 
            this.tabScanDrivers.Controls.Add(this.tpTWAINScan);
            this.tabScanDrivers.Controls.Add(this.tpWIAScan);
            this.tabScanDrivers.Location = new System.Drawing.Point(6, 27);
            this.tabScanDrivers.Name = "tabScanDrivers";
            this.tabScanDrivers.SelectedIndex = 0;
            this.tabScanDrivers.Size = new System.Drawing.Size(174, 447);
            this.tabScanDrivers.TabIndex = 7;
            this.tabScanDrivers.SelectedIndexChanged += new System.EventHandler(this.tabScanDrivers_SelectedIndexChanged);
            // 
            // tpTWAINScan
            // 
            this.tpTWAINScan.Controls.Add(this.checkBoxTwainOcr);
            this.tpTWAINScan.Controls.Add(this.buttonTwainScan);
            this.tpTWAINScan.Controls.Add(this.label1);
            this.tpTWAINScan.Controls.Add(this.cmbTwainImageType);
            this.tpTWAINScan.Controls.Add(this.sourceTwianListBox);
            this.tpTWAINScan.Location = new System.Drawing.Point(4, 22);
            this.tpTWAINScan.Name = "tpTWAINScan";
            this.tpTWAINScan.Padding = new System.Windows.Forms.Padding(3);
            this.tpTWAINScan.Size = new System.Drawing.Size(166, 421);
            this.tpTWAINScan.TabIndex = 1;
            this.tpTWAINScan.Text = "TWAIN";
            this.tpTWAINScan.UseVisualStyleBackColor = true;
            // 
            // checkBoxTwainOcr
            // 
            this.checkBoxTwainOcr.AutoSize = true;
            this.checkBoxTwainOcr.Location = new System.Drawing.Point(10, 229);
            this.checkBoxTwainOcr.Name = "checkBoxTwainOcr";
            this.checkBoxTwainOcr.Size = new System.Drawing.Size(49, 17);
            this.checkBoxTwainOcr.TabIndex = 12;
            this.checkBoxTwainOcr.Text = "OCR";
            this.checkBoxTwainOcr.UseVisualStyleBackColor = true;
            this.checkBoxTwainOcr.Visible = false;
            // 
            // buttonTwainScan
            // 
            this.buttonTwainScan.Location = new System.Drawing.Point(72, 225);
            this.buttonTwainScan.Name = "buttonTwainScan";
            this.buttonTwainScan.Size = new System.Drawing.Size(75, 23);
            this.buttonTwainScan.TabIndex = 11;
            this.buttonTwainScan.Text = "Scan";
            this.buttonTwainScan.UseVisualStyleBackColor = true;
            this.buttonTwainScan.Click += new System.EventHandler(this.ButtonTwainScan_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 180);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Image format";
            // 
            // cmbTwainImageType
            // 
            this.cmbTwainImageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTwainImageType.FormattingEnabled = true;
            this.cmbTwainImageType.Items.AddRange(new object[] {
            "TIFF",
            "PDF",
            "JPEG",
            "PNG"});
            this.cmbTwainImageType.Location = new System.Drawing.Point(10, 198);
            this.cmbTwainImageType.Name = "cmbTwainImageType";
            this.cmbTwainImageType.Size = new System.Drawing.Size(137, 21);
            this.cmbTwainImageType.TabIndex = 9;
            // 
            // sourceTwianListBox
            // 
            this.sourceTwianListBox.FormattingEnabled = true;
            this.sourceTwianListBox.Location = new System.Drawing.Point(6, 8);
            this.sourceTwianListBox.Name = "sourceTwianListBox";
            this.sourceTwianListBox.Size = new System.Drawing.Size(146, 160);
            this.sourceTwianListBox.TabIndex = 3;
            this.sourceTwianListBox.SelectedValueChanged += new System.EventHandler(this.SourceTwianListBox_SelectedValueChanged);
            // 
            // tpWIAScan
            // 
            this.tpWIAScan.Controls.Add(this.checkboxWiaOcr);
            this.tpWIAScan.Controls.Add(this.sourceListBox);
            this.tpWIAScan.Controls.Add(this.btnWIAScan);
            this.tpWIAScan.Controls.Add(this.groupBox1);
            this.tpWIAScan.Location = new System.Drawing.Point(4, 22);
            this.tpWIAScan.Name = "tpWIAScan";
            this.tpWIAScan.Padding = new System.Windows.Forms.Padding(3);
            this.tpWIAScan.Size = new System.Drawing.Size(166, 421);
            this.tpWIAScan.TabIndex = 0;
            this.tpWIAScan.Text = "WIA";
            this.tpWIAScan.UseVisualStyleBackColor = true;
            // 
            // checkboxWiaOcr
            // 
            this.checkboxWiaOcr.AutoSize = true;
            this.checkboxWiaOcr.Location = new System.Drawing.Point(16, 340);
            this.checkboxWiaOcr.Name = "checkboxWiaOcr";
            this.checkboxWiaOcr.Size = new System.Drawing.Size(49, 17);
            this.checkboxWiaOcr.TabIndex = 7;
            this.checkboxWiaOcr.Text = "OCR";
            this.checkboxWiaOcr.UseVisualStyleBackColor = true;
            this.checkboxWiaOcr.Visible = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 596);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(904, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(93, 17);
            this.toolStripStatusLabel1.Text = "processing ocr...";
            this.toolStripStatusLabel1.Visible = false;
            // 
            // MainFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 618);
            this.Controls.Add(this.MainMenu1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabScanDrivers);
            this.Controls.Add(this.flowLayoutPanel1);
            this.MainMenuStrip = this.MainMenu1;
            this.Name = "MainFrame";
            this.Text = "Scan Document";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFrame_FormClosing);
            this.Load += new System.EventHandler(this.MainFrame_Load);
            this.Shown += new System.EventHandler(this.MainFrame_Shown);
            this.MainMenu1.ResumeLayout(false);
            this.MainMenu1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabScanDrivers.ResumeLayout(false);
            this.tpTWAINScan.ResumeLayout(false);
            this.tpTWAINScan.PerformLayout();
            this.tpWIAScan.ResumeLayout(false);
            this.tpWIAScan.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal System.Windows.Forms.MenuStrip MainMenu1;
        internal System.Windows.Forms.ToolStripMenuItem menuMainFile;
        internal System.Windows.Forms.ToolStripMenuItem MenuItem5;
        internal System.Windows.Forms.ToolStripMenuItem mnuAcquire;
        internal System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ListBox sourceListBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblResolution;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbImageType;
        private System.Windows.Forms.ComboBox cmbResolution;
        private System.Windows.Forms.ComboBox cmbColor;
        private System.Windows.Forms.Button btnWIAScan;
        private System.Windows.Forms.TabControl tabScanDrivers;
        private System.Windows.Forms.TabPage tpWIAScan;
        private System.Windows.Forms.TabPage tpTWAINScan;
        private System.Windows.Forms.ListBox sourceTwianListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbTwainImageType;
        private System.Windows.Forms.Button buttonTwainScan;
        private System.Windows.Forms.CheckBox checkBoxTwainOcr;
        private System.Windows.Forms.CheckBox checkboxWiaOcr;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}