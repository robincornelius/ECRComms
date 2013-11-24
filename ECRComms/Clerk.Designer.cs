namespace libECRComms
{
    partial class Clerk
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
            this.button_readclerks = new System.Windows.Forms.Button();
            this.button_writeclerks = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.SuspendLayout();

            // 
            // button_writeclerks
            // 
            this.button_writeclerks.Location = new System.Drawing.Point(235, 12);
            this.button_writeclerks.Name = "button_writeclerks";
            this.button_writeclerks.Size = new System.Drawing.Size(116, 29);
            this.button_writeclerks.TabIndex = 1;
            this.button_writeclerks.Text = "Write Clerks";
            this.button_writeclerks.UseVisualStyleBackColor = true;
            this.button_writeclerks.Click += new System.EventHandler(this.button_writeclerks_Click);
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(56, 77);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(511, 355);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // Clerk
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 526);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.button_writeclerks);
            this.Controls.Add(this.button_readclerks);
            this.Name = "Clerk";
            this.Text = "Clerk";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_readclerks;
        private System.Windows.Forms.Button button_writeclerks;
        private System.Windows.Forms.ListView listView1;
    }
}