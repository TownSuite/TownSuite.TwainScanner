namespace TownSuite.TwainScanner
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
            MainMenu1 = new System.Windows.Forms.MenuStrip();
            menuMainFile = new System.Windows.Forms.ToolStripMenuItem();
            MenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            mnuAcquire = new System.Windows.Forms.ToolStripMenuItem();
            mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            checkboxOcr = new System.Windows.Forms.CheckBox();
            sourceListBox = new System.Windows.Forms.ListBox();
            btnScan = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label5 = new System.Windows.Forms.Label();
            lblResolution = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            cmbImageType = new System.Windows.Forms.ComboBox();
            cmbResolution = new System.Windows.Forms.ComboBox();
            cmbColor = new System.Windows.Forms.ComboBox();
            MainMenu1.SuspendLayout();
            statusStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // MainMenu1
            // 
            MainMenu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { menuMainFile, mnuAcquire, mnuSave });
            MainMenu1.Location = new System.Drawing.Point(0, 0);
            MainMenu1.Name = "MainMenu1";
            MainMenu1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            MainMenu1.Size = new System.Drawing.Size(1055, 24);
            MainMenu1.TabIndex = 0;
            // 
            // menuMainFile
            // 
            menuMainFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { MenuItem5 });
            menuMainFile.MergeAction = System.Windows.Forms.MergeAction.Insert;
            menuMainFile.MergeIndex = 0;
            menuMainFile.Name = "menuMainFile";
            menuMainFile.Size = new System.Drawing.Size(37, 20);
            menuMainFile.Text = "File";
            // 
            // MenuItem5
            // 
            MenuItem5.MergeIndex = 0;
            MenuItem5.Name = "MenuItem5";
            MenuItem5.Size = new System.Drawing.Size(92, 22);
            MenuItem5.Text = "Exit";
            MenuItem5.Click += MenuItem5_Click;
            // 
            // mnuAcquire
            // 
            mnuAcquire.MergeIndex = 1;
            mnuAcquire.Name = "mnuAcquire";
            mnuAcquire.Size = new System.Drawing.Size(60, 20);
            mnuAcquire.Text = "&Acquire";
            mnuAcquire.Click += mnuAcquire_Click;
            // 
            // mnuSave
            // 
            mnuSave.MergeIndex = 2;
            mnuSave.Name = "mnuSave";
            mnuSave.Size = new System.Drawing.Size(43, 20);
            mnuSave.Text = "&Save";
            mnuSave.Click += mnuSave_Click;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.Location = new System.Drawing.Point(217, 0);
            flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(828, 684);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripProgressBar1, toolStripStatusLabel1 });
            statusStrip1.Location = new System.Drawing.Point(0, 691);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusStrip1.Size = new System.Drawing.Size(1055, 22);
            statusStrip1.TabIndex = 8;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new System.Drawing.Size(117, 18);
            toolStripProgressBar1.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(93, 17);
            toolStripStatusLabel1.Text = "processing ocr...";
            toolStripStatusLabel1.Visible = false;
            // 
            // checkboxOcr
            // 
            checkboxOcr.AutoSize = true;
            checkboxOcr.Location = new System.Drawing.Point(25, 424);
            checkboxOcr.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkboxOcr.Name = "checkboxOcr";
            checkboxOcr.Size = new System.Drawing.Size(50, 19);
            checkboxOcr.TabIndex = 12;
            checkboxOcr.Text = "OCR";
            checkboxOcr.UseVisualStyleBackColor = true;
            checkboxOcr.Visible = false;
            // 
            // sourceListBox
            // 
            sourceListBox.FormattingEnabled = true;
            sourceListBox.ItemHeight = 15;
            sourceListBox.Location = new System.Drawing.Point(13, 39);
            sourceListBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            sourceListBox.Name = "sourceListBox";
            sourceListBox.Size = new System.Drawing.Size(170, 184);
            sourceListBox.TabIndex = 9;
            // 
            // btnWIAScan
            // 
            btnScan.Location = new System.Drawing.Point(95, 420);
            btnScan.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnScan.Name = "btnWIAScan";
            btnScan.Size = new System.Drawing.Size(88, 27);
            btnScan.TabIndex = 11;
            btnScan.Text = "Scan";
            btnScan.UseVisualStyleBackColor = true;
            btnScan.Click += btnScan_ClickAsync;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(lblResolution);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(cmbImageType);
            groupBox1.Controls.Add(cmbResolution);
            groupBox1.Controls.Add(cmbColor);
            groupBox1.Location = new System.Drawing.Point(15, 228);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(170, 185);
            groupBox1.TabIndex = 10;
            groupBox1.TabStop = false;
            groupBox1.Text = "Properties";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(8, 120);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(36, 15);
            label5.TabIndex = 14;
            label5.Text = "Color";
            // 
            // lblResolution
            // 
            lblResolution.AutoSize = true;
            lblResolution.Location = new System.Drawing.Point(8, 72);
            lblResolution.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblResolution.Name = "lblResolution";
            lblResolution.Size = new System.Drawing.Size(63, 15);
            lblResolution.TabIndex = 12;
            lblResolution.Text = "Resolution";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(13, 18);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(79, 15);
            label3.TabIndex = 8;
            label3.Text = "Image format";
            // 
            // cmbImageType
            // 
            cmbImageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbImageType.FormattingEnabled = true;
            cmbImageType.Items.AddRange(new object[] { "TIFF", "PDF", "PNG", "JPEG" });
            cmbImageType.Location = new System.Drawing.Point(9, 39);
            cmbImageType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbImageType.Name = "cmbImageType";
            cmbImageType.Size = new System.Drawing.Size(128, 23);
            cmbImageType.TabIndex = 7;
            // 
            // cmbResolution
            // 
            cmbResolution.FormattingEnabled = true;
            cmbResolution.Location = new System.Drawing.Point(9, 90);
            cmbResolution.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbResolution.Name = "cmbResolution";
            cmbResolution.Size = new System.Drawing.Size(128, 23);
            cmbResolution.TabIndex = 2;
            // 
            // cmbColor
            // 
            cmbColor.FormattingEnabled = true;
            cmbColor.Location = new System.Drawing.Point(9, 138);
            cmbColor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbColor.Name = "cmbColor";
            cmbColor.Size = new System.Drawing.Size(128, 23);
            cmbColor.TabIndex = 3;
            // 
            // MainFrame
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1055, 713);
            Controls.Add(checkboxOcr);
            Controls.Add(sourceListBox);
            Controls.Add(btnScan);
            Controls.Add(groupBox1);
            Controls.Add(MainMenu1);
            Controls.Add(statusStrip1);
            Controls.Add(flowLayoutPanel1);
            MainMenuStrip = MainMenu1;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MainFrame";
            Text = "Scan Document";
            FormClosing += MainFrame_FormClosing;
            Shown += MainFrame_Shown;
            MainMenu1.ResumeLayout(false);
            MainMenu1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        internal System.Windows.Forms.MenuStrip MainMenu1;
        internal System.Windows.Forms.ToolStripMenuItem menuMainFile;
        internal System.Windows.Forms.ToolStripMenuItem MenuItem5;
        internal System.Windows.Forms.ToolStripMenuItem mnuAcquire;
        internal System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.CheckBox checkboxOcr;
        private System.Windows.Forms.ListBox sourceListBox;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblResolution;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbImageType;
        private System.Windows.Forms.ComboBox cmbResolution;
        private System.Windows.Forms.ComboBox cmbColor;
    }
}