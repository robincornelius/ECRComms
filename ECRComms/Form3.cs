using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;
using System.IO;



namespace libECRComms
{
    public partial class Form3 : Form
    {

        List<byte> data;

        public Form3()
        {
            InitializeComponent();
            initcomms();
        }

        private bool initcomms()
        {
            Application.UseWaitCursor = true;

            Program.ecr.commport = Properties.Settings.Default.comport;
            Program.ecr.baud = Properties.Settings.Default.baud;
            if (Program.ecr.init() == false)
            {
                toolStripStatusLabel1.Text = "COMMS fail check settings";
                Application.UseWaitCursor = false;
                return false;
            }

            if (Program.ecr.checkstatus())
            {
                toolStripStatusLabel1.Text = "ECR connected OK";
                Application.UseWaitCursor = false;
                return true;
            }
            else
            {
                toolStripStatusLabel1.Text = "ECR Not Found!";
                Application.UseWaitCursor = false;
                return false;
            }

        }

 

        private void button_download_Click(object sender, EventArgs e)
        {

            if(hexBox1.ByteProvider!=null)
            {
                DynamicFileByteProvider dynamicFileByteProviderx = (DynamicFileByteProvider) hexBox1.ByteProvider;
                hexBox1.ByteProvider = null;
                
                dynamicFileByteProviderx.Dispose();

            }

            Progress p = new Progress(Program.ecr);
            p.Show();

            data = Program.ecr.getprogram((ECRComms.program)comboBox_ECRfiles.SelectedIndex);

            if (data == null)
            {
                if (initcomms() == true)
                {
                    MessageBox.Show("No data recieved but comms checks out OK");
                }
                else
                {
                    MessageBox.Show("Comms fail");
                }

                return;
            }

            string tmpfile = Path.Combine(Program.RWpath,"data.dat");
            File.WriteAllBytes(tmpfile, data.ToArray());

            DynamicFileByteProvider dynamicFileByteProvider = new DynamicFileByteProvider(tmpfile);

            hexBox1.BytesPerLine = 10;
            hexBox1.StringViewVisible = true;


            hexBox1.ByteProvider = dynamicFileByteProvider;

            button_edit.Enabled = true;
            button_upload.Enabled = true;
            button_save.Enabled = true;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void serialSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options o = new Options();
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                initcomms();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void testCommsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            initcomms();
        }

        private void button_edit_Click(object sender, EventArgs e)
        {

            if (data == null)
                return;

            if (data.Count == 0)
                return;

            switch (comboBox_ECRfiles.SelectedIndex)
            {
                case 0:
                     Form_PLUList p = new Form_PLUList(data);
                     p.Show();
                    break;

                case 6:
                     Clerk c = new Clerk(data);
                     c.Show();
                     break;

                default:
                     MessageBox.Show("Sorry Editor not finished yet!");
                    break;

            }

      
        }

        private void comboBox_ECRfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            button_edit.Enabled = false;
            button_upload.Enabled = false;
            button_save.Enabled = false;
        }

        private void button_upload_Click(object sender, EventArgs e)
        {

        }

        private void button_save_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    writedata(data.ToArray(), sfd.FileName);
                }
                catch (Exception _Exception)
                {
                    Console.WriteLine("Exception caught saving file{0}",_Exception.ToString());
                }

            }

        }

        void writedata(byte[] data,string filename)
        {
            System.IO.FileStream _FileStream =
                    new System.IO.FileStream(filename, System.IO.FileMode.Create,
                                             System.IO.FileAccess.Write);

            byte[] header = new byte[6];
            header[0] = 0xFF; //
            header[1] = 0xFE; //
            header[2] = 0x06; //header size
            header[3] = 0x01; //our version
            header[4] = (byte)comboBox_ECRfiles.SelectedIndex; //PLU file in use
            header[5] = 0xFF; //Spare

            _FileStream.Write(header, 0, header.Length);

            _FileStream.Write(data, 0, data.Length);

            _FileStream.Close();

        }

        private void button_load_Click(object sender, EventArgs e)
        {
            try
            {

                OpenFileDialog lfd = new OpenFileDialog();
                if (lfd.ShowDialog() == DialogResult.OK)
                {
                    if (hexBox1.ByteProvider != null)
                    {
                        DynamicFileByteProvider dynamicFileByteProviderx = (DynamicFileByteProvider)hexBox1.ByteProvider;
                        hexBox1.ByteProvider = null;

                        dynamicFileByteProviderx.Dispose();

                    }


                    data = DataFile.loadbinaryfile(lfd.FileName).ToList();


                   // comboBox_ECRfiles.SelectedIndex = header[4];


                    string tmpfile = Path.Combine(Program.RWpath, "data.dat");
                    File.WriteAllBytes(tmpfile, data.ToArray());

                    DynamicFileByteProvider dynamicFileByteProvider = new DynamicFileByteProvider(tmpfile);

                    hexBox1.BytesPerLine = 10;
                    hexBox1.StringViewVisible = true;

                    hexBox1.ByteProvider = dynamicFileByteProvider;

                    button_edit.Enabled = true;
                    button_upload.Enabled = true;
                    button_save.Enabled = true;
   
                }
            }
            catch (Exception _Exception)
            {
                Console.WriteLine("Exception caught saving file{0}", _Exception.ToString());
            }
        }

        private void button_backup_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                int lastFoo = (int)Enum.GetValues(typeof(ECRComms.program)).Cast<ECRComms.program>().Last();

                for (int p = 0; p <= lastFoo; p++)
                {
                    Progress pr = new Progress(Program.ecr);
                    pr.Show();

                    string  name = ((ECRComms.program)p).ToString();
                    data = Program.ecr.getprogram((ECRComms.program)p);

                    pr.Close();

                    try
                    {
                        writedata(data.ToArray(),  Path.Combine(fbd.SelectedPath, name + ".dat"));
                    }
                    catch (Exception _Exception)
                    {
                        Console.WriteLine("Exception caught saving file{0}", _Exception.ToString());
                    }


                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if(comboBox_ECRfiles.SelectedIndex==7)
            {
                Descriptor d = new Descriptor(data.ToArray());
                d.dump();

            }

        }
    }
}
