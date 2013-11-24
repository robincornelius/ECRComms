/*

    This file is part of ECRComms.

    ECRComms is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

    Copyright (c) 2013 Robin Cornelius <robin.cornelius@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO.Ports;
using System.Collections;
using System.IO;

namespace libECRComms
{
    public partial class Form_PLUList : Form
    {


        List<PLUcommon> plus = new List<PLUcommon>();

        BitArray status;

        public Form_PLUList(List<byte> data)
        {
         
            InitializeComponent();
            if (data != null)
                getPLUs(data);

        }

        void writePLUs()
        {

            List<data_serialisation> ds = new List<data_serialisation>();

            foreach (PLUcommon p in plus)
            {
                ds.Add(p);
            }

            Application.UseWaitCursor = true;

            Program.ecr.setprogram(ECRComms.program.PLU,ds);

            Application.UseWaitCursor = false;
        }

        void getPLUs(List<byte> data)
        {
            Application.UseWaitCursor = true;

            plus.Clear();
            listView1.Items.Clear();

            //List<byte> data = Program.ecr.getprogram(ECRComms.program.PLU);
            int offset = 0;
            byte[] allbytes =  data.ToArray();

            int plucount = 0;
            while (offset <= allbytes.Length-ER230_PLU.Length)
            {

                ER230_PLU ep = new ER230_PLU();
                byte[] pludata = new byte[ER230_PLU.Length];
                Buffer.BlockCopy(allbytes, offset, pludata, 0, pludata.Length);
                ep.frombytes(pludata);

                addPLUtolist(ep);

                offset += ep.data.Length; //ER-230 PLU is 46 // 380M is 55;

                plucount++;

            }

            Application.UseWaitCursor = false;
      
        }

        void addPLUtolist(PLUcommon p)
        {
            plus.Add(p);

            Console.WriteLine("PLU Code is :" + p.PLUcode.scode);
            Console.WriteLine("PLU Description :" + p.description);

            ListViewItem item = new ListViewItem(p.PLUcode.scode);
            item.Tag = p;

            item.SubItems.Add(p.description);
            item.SubItems.Add(String.Format("£{0:0.00}", (double)p.price / 100));
            listView1.Items.Add(item);

        }

        ListViewHitTestInfo loc;
        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                loc = listView1.HitTest(e.Location);
                if (loc.Item != null)
                {               
                    if (loc.Item != null) contextMenuStrip1.Show(listView1, e.Location);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            writePLUs();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (loc.Item != null)
            {
                Form_Editor f = new Form_Editor((PLUcommon)loc.Item.Tag);
                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                     PLUcommon p =(PLUcommon)loc.Item.Tag;
                    loc.Item.SubItems[1].Text = p.description;
                    loc.Item.SubItems[2].Text = (String.Format("£{0:0.00}", (double)p.price / 100));
                }
            }
          
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loc.Item != null)
            {
                PLUcommon p = (PLUcommon)loc.Item.Tag;
                plus.Remove(p);
            }
        }

        private void button_addnew_Click(object sender, EventArgs e)
        {
            Form_Editor f = new Form_Editor(null);
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                addPLUtolist(f.plu);
            }
        }
    }

}
