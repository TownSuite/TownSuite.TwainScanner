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
            this.components = new System.ComponentModel.Container();
            this.MainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuMainFile = new System.Windows.Forms.MenuItem();
            this.MenuItem5 = new System.Windows.Forms.MenuItem();
            this.mnuSelect = new System.Windows.Forms.MenuItem();
            this.mnuAcquire = new System.Windows.Forms.MenuItem();
            this.mnuSave = new System.Windows.Forms.MenuItem();
            this.MenuItem1 = new System.Windows.Forms.MenuItem();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // MainMenu1
            // 
            this.MainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuMainFile,
            this.mnuSelect,
            this.mnuAcquire,
            this.mnuSave,
            this.MenuItem1});
            // 
            // menuMainFile
            // 
            this.menuMainFile.Index = 0;
            this.menuMainFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MenuItem5});
            this.menuMainFile.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            this.menuMainFile.Text = "File";
            // 
            // MenuItem5
            // 
            this.MenuItem5.Index = 0;
            this.MenuItem5.Text = "Exit";
            this.MenuItem5.Click += new System.EventHandler(this.MenuItem5_Click);
            // 
            // mnuSelect
            // 
            this.mnuSelect.Index = 1;
            this.mnuSelect.Text = "Se&lect Scanner";
            this.mnuSelect.Click += new System.EventHandler(this.mnuSelect_Click);
            // 
            // mnuAcquire
            // 
            this.mnuAcquire.Index = 2;
            this.mnuAcquire.Text = "&Acquire";
            this.mnuAcquire.Click += new System.EventHandler(this.mnuAcquire_Click);
            // 
            // mnuSave
            // 
            this.mnuSave.Index = 3;
            this.mnuSave.Text = "&Save";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // MenuItem1
            // 
            this.MenuItem1.Index = 4;
            this.MenuItem1.Text = "Window";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(389, 147);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // MainFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 147);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Menu = this.MainMenu1;
            this.Name = "MainFrame";
            this.Text = "Scan Document";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFrame_FormClosing);
            this.Load += new System.EventHandler(this.MainFrame_Load);
            this.ResumeLayout(false);

        }

        #endregion
        internal System.Windows.Forms.MainMenu MainMenu1;
        internal System.Windows.Forms.MenuItem menuMainFile;
        internal System.Windows.Forms.MenuItem MenuItem5;
        internal System.Windows.Forms.MenuItem mnuSelect;
        internal System.Windows.Forms.MenuItem mnuAcquire;
        internal System.Windows.Forms.MenuItem mnuSave;
        internal System.Windows.Forms.MenuItem MenuItem1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}