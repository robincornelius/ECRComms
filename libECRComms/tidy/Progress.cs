using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace libECRComms
{
    public partial class Progress : Form
    {
        int maxblocks;

        public Progress(ECRComms ecr)
        {
            InitializeComponent();
            ecr.Progress += new ECRComms.ProgressEventHandler(ecr_Progress);
            maxblocks = 0;
            progressBar1.Value = progressBar1.Maximum = 1;
            progressBar1.Minimum = 0;
            label1.Text = "Sending command to ecr...";
            Update();
            Application.DoEvents(); //blerhgh

        }

        public void ecr_Progress(object sender, EventArgs e)
        {

            ProgressEventArgs ee = (ProgressEventArgs)e;
            if (ee.state == ProgressEventArgs.ProgressState.PROGRESS_DONE)
            {
                this.progressBar1.Value = progressBar1.Maximum;
                this.label1.Text = "ALL DONE";
                Application.DoEvents(); //FIXME use threading else where to remove this abomination
                System.Threading.Thread.Sleep(1000); //pause before close
                Close();
            }

            if (ee.state == ProgressEventArgs.ProgressState.PROGRESS_DOWNLOAD)
            {
       
                maxblocks = ee.blockstotal;
                this.progressBar1.Maximum = maxblocks;
                this.progressBar1.Minimum = 0;
                this.progressBar1.Value = 0;
                this.progressBar1.Value = 0;
                this.label1.Text = String.Format("Downloading {0}/{1}",ee.blocksdone,maxblocks);     
            }

            if (ee.state == ProgressEventArgs.ProgressState.PROGRESS_DOWNLOAD_TICK)
            {
                this.progressBar1.Value = ee.blocksdone;
                this.label1.Text = String.Format("Downloading {0}/{1}", ee.blocksdone, maxblocks);
            }


            if (ee.state == ProgressEventArgs.ProgressState.PROGRESS_UPLOAD)
            {

                maxblocks = ee.blockstotal;
                this.progressBar1.Maximum = maxblocks;
                this.progressBar1.Minimum = 0;
                this.progressBar1.Update();
                this.progressBar1.Value = 0;
                this.label1.Text = String.Format("Uploading {0}/{1}", ee.blocksdone, maxblocks); 
            }

            if (ee.state == ProgressEventArgs.ProgressState.PROGRESS_UPLOAD_TICK)
            {
              
                this.progressBar1.Value = ee.blocksdone;
                this.label1.Text = String.Format("Uploading {0}/{1}", ee.blocksdone, maxblocks);
            }

            if (ee.state == ProgressEventArgs.ProgressState.PROGRESS_ERROR)
            {
                Close();
            }

            Application.DoEvents(); //FIXME use threading else where to remove this abomination

        }

        private void Progress_Load(object sender, EventArgs e)
        {


        }

        /*
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Progress
            // 
            this.ClientSize = new System.Drawing.Size(711, 438);
            this.Name = "Progress";
            this.ResumeLayout(false);

        }
         * */
    }
}
