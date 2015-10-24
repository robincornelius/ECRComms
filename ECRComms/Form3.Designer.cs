namespace libECRComms
{
    partial class Form3
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
            this.button_download = new System.Windows.Forms.Button();
            this.comboBox_ECRfiles = new System.Windows.Forms.ComboBox();
            this.hexBox1 = new Be.Windows.Forms.HexBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.button_backup = new System.Windows.Forms.Button();
            this.button_load = new System.Windows.Forms.Button();
            this.button_save = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button_edit = new System.Windows.Forms.Button();
            this.button_upload = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serialSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testCommsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.button_parse = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_download
            // 
            this.button_download.Location = new System.Drawing.Point(191, 33);
            this.button_download.Name = "button_download";
            this.button_download.Size = new System.Drawing.Size(134, 23);
            this.button_download.TabIndex = 6;
            this.button_download.Text = "Download (ECR -> PC)";
            this.button_download.UseVisualStyleBackColor = true;
            this.button_download.Click += new System.EventHandler(this.button_download_Click);
            // 
            // comboBox_ECRfiles
            // 
            this.comboBox_ECRfiles.FormattingEnabled = true;
            this.comboBox_ECRfiles.Items.AddRange(new object[] {
            "PLU",
            "Group",
            "Tax",
            "System_Option",
            "Print_Option",
            "Function_Key",
            "Clerk",
            "Logo",
            "FINANCIAL_REPORT_LOGO",
            "CLERK_REPORT_LOGO",
            "STOCK",
            "MACRO",
            "MISC",
            "MIXANDMATCH"});
            this.comboBox_ECRfiles.Location = new System.Drawing.Point(58, 33);
            this.comboBox_ECRfiles.Name = "comboBox_ECRfiles";
            this.comboBox_ECRfiles.Size = new System.Drawing.Size(117, 21);
            this.comboBox_ECRfiles.TabIndex = 7;
            this.comboBox_ECRfiles.SelectedIndexChanged += new System.EventHandler(this.comboBox_ECRfiles_SelectedIndexChanged);
            // 
            // hexBox1
            // 
            this.hexBox1.ColumnInfoVisible = true;
            this.hexBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexBox1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexBox1.ImeMode = System.Windows.Forms.ImeMode.AlphaFull;
            this.hexBox1.InfoForeColor = System.Drawing.Color.Empty;
            this.hexBox1.LineInfoVisible = true;
            this.hexBox1.Location = new System.Drawing.Point(0, 0);
            this.hexBox1.Name = "hexBox1";
            this.hexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox1.Size = new System.Drawing.Size(862, 366);
            this.hexBox1.TabIndex = 8;
            this.hexBox1.VScrollBarVisible = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.button_parse);
            this.splitContainer1.Panel1.Controls.Add(this.button_backup);
            this.splitContainer1.Panel1.Controls.Add(this.button_load);
            this.splitContainer1.Panel1.Controls.Add(this.button_save);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.button_edit);
            this.splitContainer1.Panel1.Controls.Add(this.button_upload);
            this.splitContainer1.Panel1.Controls.Add(this.button_download);
            this.splitContainer1.Panel1.Controls.Add(this.comboBox_ECRfiles);
            this.splitContainer1.Panel1.Controls.Add(this.menuStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Panel2.Controls.Add(this.hexBox1);
            this.splitContainer1.Size = new System.Drawing.Size(862, 567);
            this.splitContainer1.SplitterDistance = 197;
            this.splitContainer1.TabIndex = 9;
            // 
            // button_backup
            // 
            this.button_backup.Location = new System.Drawing.Point(508, 33);
            this.button_backup.Name = "button_backup";
            this.button_backup.Size = new System.Drawing.Size(134, 23);
            this.button_backup.TabIndex = 14;
            this.button_backup.Text = "Backup all files";
            this.button_backup.UseVisualStyleBackColor = true;
            this.button_backup.Click += new System.EventHandler(this.button_backup_Click);
            // 
            // button_load
            // 
            this.button_load.Location = new System.Drawing.Point(340, 62);
            this.button_load.Name = "button_load";
            this.button_load.Size = new System.Drawing.Size(134, 23);
            this.button_load.TabIndex = 13;
            this.button_load.Text = "Load from Disk";
            this.button_load.UseVisualStyleBackColor = true;
            this.button_load.Click += new System.EventHandler(this.button_load_Click);
            // 
            // button_save
            // 
            this.button_save.Enabled = false;
            this.button_save.Location = new System.Drawing.Point(340, 33);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(134, 23);
            this.button_save.TabIndex = 12;
            this.button_save.Text = "Save to Disk";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "File";
            // 
            // button_edit
            // 
            this.button_edit.Enabled = false;
            this.button_edit.Location = new System.Drawing.Point(191, 91);
            this.button_edit.Name = "button_edit";
            this.button_edit.Size = new System.Drawing.Size(134, 23);
            this.button_edit.TabIndex = 10;
            this.button_edit.Text = "Edit";
            this.button_edit.UseVisualStyleBackColor = true;
            this.button_edit.Click += new System.EventHandler(this.button_edit_Click);
            // 
            // button_upload
            // 
            this.button_upload.Enabled = false;
            this.button_upload.Location = new System.Drawing.Point(191, 62);
            this.button_upload.Name = "button_upload";
            this.button_upload.Size = new System.Drawing.Size(134, 23);
            this.button_upload.TabIndex = 9;
            this.button_upload.Text = "Upload (PC -> ECR)";
            this.button_upload.UseVisualStyleBackColor = true;
            this.button_upload.Click += new System.EventHandler(this.button_upload_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(862, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serialSetupToolStripMenuItem,
            this.testCommsToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // serialSetupToolStripMenuItem
            // 
            this.serialSetupToolStripMenuItem.Name = "serialSetupToolStripMenuItem";
            this.serialSetupToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.serialSetupToolStripMenuItem.Text = "Serial Setup";
            this.serialSetupToolStripMenuItem.Click += new System.EventHandler(this.serialSetupToolStripMenuItem_Click);
            // 
            // testCommsToolStripMenuItem
            // 
            this.testCommsToolStripMenuItem.Name = "testCommsToolStripMenuItem";
            this.testCommsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.testCommsToolStripMenuItem.Text = "Test Comms";
            this.testCommsToolStripMenuItem.Click += new System.EventHandler(this.testCommsToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 344);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(862, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // button_parse
            // 
            this.button_parse.Location = new System.Drawing.Point(32, 91);
            this.button_parse.Name = "button_parse";
            this.button_parse.Size = new System.Drawing.Size(134, 23);
            this.button_parse.TabIndex = 15;
            this.button_parse.Text = "parse";
            this.button_parse.UseVisualStyleBackColor = true;
            this.button_parse.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 567);
            this.Controls.Add(this.splitContainer1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form3";
            this.Text = "ECR Control";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_download;
        private System.Windows.Forms.ComboBox comboBox_ECRfiles;
        private Be.Windows.Forms.HexBox hexBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serialSetupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem testCommsToolStripMenuItem;
        private System.Windows.Forms.Button button_edit;
        private System.Windows.Forms.Button button_upload;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_load;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_backup;
        private System.Windows.Forms.Button button_parse;
    }
}