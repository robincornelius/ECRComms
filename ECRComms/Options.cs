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
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();

            string[] theSerialPortNames = System.IO.Ports.SerialPort.GetPortNames();

            foreach (string name in theSerialPortNames)
            {
                comboBox_comport.Items.Add(name);
            }

            if (theSerialPortNames.Length == 0)
            {
                MessageBox.Show("No Serial ports avaiable on the system\nIf you are using USB serial please plug in and try again");
                return;
            }


            comboBox_baud.SelectedItem = Properties.Settings.Default.baud;
            comboBox_comport.SelectedItem = Properties.Settings.Default.comport;

            if (comboBox_comport.SelectedItem == null)
            {
                MessageBox.Show("Selected serial port is not avaiable, please select another");
            }

        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.baud = int.Parse((string)comboBox_baud.SelectedItem);
            Properties.Settings.Default.comport = (string)comboBox_comport.SelectedItem;
            Properties.Settings.Default.Save();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
