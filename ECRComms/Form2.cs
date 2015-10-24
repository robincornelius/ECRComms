/*

    This file is part of ECRComms.

    ECRComms is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    ECRComms is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with ECRComms.  If not, see <http://www.gnu.org/licenses/>.

    Copyright (c) 2013 - 2015 Robin Cornelius <robin.cornelius@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace libECRComms
{
    public partial class Form_Editor : Form
    {
        public PLUcommon plu;
        BitArray ba;

        public Form_Editor(PLUcommon p)
        {
            InitializeComponent();

            if (p == null)
            {
                //FIX ME we need a dymanic way of selecting which PLUcommon to instance
                p = new ER230_PLU();
                plu = p;
                return;
            }

            plu = p;
         
            Int32[] status = new Int32[1];
            status[0] = p.status;
       
            ba = new BitArray(status);

            this.checkBox_N1_1.Checked = ba.Get(0);
            this.checkBox_N1_2.Checked = ba.Get(1);
            this.checkBox_N1_4.Checked = ba.Get(2);

            this.checkBox_N2_1.Checked = ba.Get(3);
            this.checkBox_N2_2.Checked = ba.Get(4);
            this.checkBox_N2_4.Checked = ba.Get(5);

            this.checkBox_N3_1.Checked = ba.Get(6);
            this.checkBox_N3_2.Checked = ba.Get(7);
            this.checkBox_N3_4.Checked = ba.Get(8);

            this.checkBox_N4_1.Checked = ba.Get(9);
            this.checkBox_N4_2.Checked = ba.Get(10);
            this.checkBox_N4_4.Checked = ba.Get(11);

            this.checkBox_N5_1.Checked = ba.Get(12);
            this.checkBox_N5_2.Checked = ba.Get(13);
            //this.checkBox_N5_4.Checked = ba.Get(14);

            //this.checkBox_N6_1.Checked = ba.Get(15);
            this.checkBox_N6_2.Checked = ba.Get(16);
            this.checkBox_N6_4.Checked = ba.Get(17);

            this.checkBox_N7_1.Checked = ba.Get(18);
            //this.checkBox_N7_2.Checked = ba.Get(19);
            this.checkBox_N7_4.Checked = ba.Get(20);

            this.checkBox_N8_1.Checked = ba.Get(21);
            this.checkBox_N8_2.Checked = ba.Get(22);
            this.checkBox_N8_4.Checked = ba.Get(23);

            this.checkBox_N9_1.Checked = ba.Get(24);
            this.checkBox_N9_2.Checked = ba.Get(25);
            this.checkBox_N9_4.Checked = ba.Get(26);

            textBox_description.Text = plu.description;
            textBox_PLU.Text = plu.PLUcode.scode;
            textBox_price1.Text = String.Format("{0}",plu.price);
            textBox_price2.Text =  String.Format("{0}",plu.price2);

            numericUpDown_group1.Value = plu.groups[0];
            numericUpDown_group2.Value = plu.groups[1];
            numericUpDown_group3.Value = plu.groups[2];


            textBox_linkPLU.Text = plu.linkPLUcode.scode;

            numericUpDown_autotare.Value = plu.autotare;
            numericUpDown_mixmatch.Value = plu.mixandmatch;

        }

        private void button_ok_Click(object sender, EventArgs e)
        {
                plu.description=textBox_description.Text;
                plu.PLUcode.fromtext(textBox_PLU.Text);
                
                //FIX ME - exception if bad data
                int price;
                if (!int.TryParse(textBox_price1.Text, out price))
                {
                    MessageBox.Show("Price 1 not in correct format");
                    return;
                }

                plu.price = price;

                plu.encode();
               
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button_cancle_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
