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
                this.progressBar1.Value = this.progressBar1.Maximum;
                this.label1.Text = "ALL DONE";
                Application.DoEvents(); //FIXME use threading else where to remove this abomination
                System.Threading.Thread.Sleep(1000); //pause before close
                Close();
            }

            if (ee.state == ProgressEventArgs.ProgressState.PROGRESS_DOWNLOAD)
            {
                if (ee.blockstotal > maxblocks)
                {
                    maxblocks = ee.blockstotal;
                    this.progressBar1.Maximum = maxblocks;
                    this.progressBar1.Minimum = 0;
                    this.progressBar1.Value = 0;
                }

                this.progressBar1.Value = ee.blocksdone+1;
                this.label1.Text = "Downloading";     
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
    }
}
