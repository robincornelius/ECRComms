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
    public partial class Clerk : Form
    {
        List<ClerkData> cd = new List<ClerkData>();
        List<byte> data;

        public Clerk(List<byte> data)
        {
            InitializeComponent();

            //We may need to be more dynamic here

            listView1.Columns.Add("Clerk Name", 200);
            this.data = data;

            cd.Clear();
            listView1.Items.Clear();

           // List<List<byte>> chunks = ECRComms.chunk(data, (int)ClerkData.Length);

          //  foreach (List<byte> c in chunks)
          //  {
          //      ClerkData thecd = new ClerkData();
          //      thecd.frombytes(c.ToArray());
           //     cd.Add(thecd);
           // }


            foreach (ClerkData c in cd)
            {
                ListViewItem i = new ListViewItem(c.name);
                listView1.Items.Add(i);
            }
        }

      
        private void button_writeclerks_Click(object sender, EventArgs e)
        {

        }


    }
}
