using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libECRComms
{

    public class PLUReportEntry
    {
        public string Description="";
        public barcode PLU;
        public double quantity;
        public double value;

        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3}", PLU.code, Description, quantity, value);

        }
    }

    public class PLUReport : data_serialisation
    {
        public List<PLUReportEntry> entries = new List<PLUReportEntry>();
        public double total;

        enum reportbytes
        {
            description = 0x01,
            barcode = 0x13,
            quantity = 0x1b,
            price = 0x23,
        }

        public PLUReport(byte[] data)
        {
            if (data != null)
            {
                this.data = data;
                decode();
            }
        }

        public PLUReport(string filename)
        {
            loadfromfile(filename);
            decode();
        }

        public override void decode()
        {
            entries = new List<PLUReportEntry>();

            List<List<byte>> report = ECRComms.chunk(data.ToList(), 0x26);

            double totalamount = 0;

            Console.WriteLine("\n\n ****************************** ");
            Console.WriteLine("STOCK REPORT\n\n");

            foreach (List<byte> lb in report)
            {

                double qty = ECRComms.extractfloat4(lb.ToArray(),0x1b);

                //Int32 iqty = (lb[0x1e] << 24) + (lb[0x1d] << 16) + (lb[0x1c] << 8) + lb[0x1b];
                //double qty = iqty / 100.0; //its not really an int its a 2 dp float

                if (qty == 0)
                    continue;

                PLUReportEntry entry = new PLUReportEntry();
                entry.quantity = qty;

                for (int pos = 1; pos < 19; pos++)
                {
                    char c = (char)lb[pos];
                    if ((c > 31) && (c < 127))
                    {
                        //Console.Write(String.Format("{0}", c));
                        entry.Description += c;
                    }
                }

                barcode b = new barcode();

                Buffer.BlockCopy(lb.ToArray(), 0x13, b.data, 0, barcode.Length);

                b.decode();

                entry.PLU = b;


                double xx = ECRComms.extractfloat3(lb.ToArray(), 0x23);

                entry.value = xx;

                totalamount += (xx);

                entries.Add(entry);

                Console.WriteLine(entry.ToString());

            }

            //Console.WriteLine("\n\n**************************************");
            //Console.WriteLine(String.Format("Total for today {0} \n\n", totalamount));
            total = totalamount;

        }

        public override void encode()
        {
        }

        public void dump()
        {
            foreach(PLUReportEntry p in entries)
            {
                Console.Write(String.Format(" = {0} {1} {2} {3} \n", p.PLU.scode,p.Description, p.quantity, p.value));

            }

        }
    }


    public class FinReportElement: data_serialisation
    {
    
         public double count;
         public double amount;

         public FinReportElement()
         {
             data = new byte[12];

         }
         public override void decode()
         {
             count = ECRComms.extractfloat4(data.ToArray(), 0);
             amount = ECRComms.extractfloat4(data.ToArray(), 8);
         }

         public override void encode()
         {
         }
    }

    public class FinReport : data_serialisation
    {
        public enum ele_names
        {
            PLU_TTL=0,
            ADJUST_TTL=2,
            NON_TAX=3,
            TAX1 = 4,//Guess
            TAX2 = 5,//Guess
            TAX3 = 6,//Guess
            TAX4 = 7,//Guess
            DISCOUNT1 = 23,
            DISCOUNT2 = 24,//Guess
            DISCOUNT3 = 25,//Guess
            DISCOUNT4 = 26,//Guess
            DISCOUNT5 = 27,//Guess
            NETSALE = 28,
            RETURN = 34,
            VOID_MODE = 37,
            CANCEL = 38,
            GROSSSALES = 39,
            CASHSALES=40,
            NOSALE = 50,//partial Guess
            CASH_IN_D=51,
            CHEQUE_IN_D=52, //Guess
            CHG1_IN_D=54,
            CHG2_IN_D = 55,
            CHG3_IN_D = 56,//Guess
            CHG4_IN_D = 57,//Guess
            CHG5_IN_D = 58,//Guess
            CHG6_IN_D = 59,//Guess
            CHG7_IN_D = 60,//Guess
            CHG8_IN_D = 61,//Guess
            CHG1_SALES = 62,
            CHG2_SALES = 63,//Guess
            CHG3_SALES = 64,//Guess
            CHG4_SALES = 65,//Guess
            CHG5_SALES = 66,//Guess
            CHG6_SALES = 67,//Guess
            CHG7_SALES = 68,//Guess
            CHG8_SALES = 69,//Guess
            DWR_TTL = 74,
            PLU_LEVEL1_TTL = 85
        }

        double grand;

        List<FinReportElement> elements = new List<FinReportElement>();

        public FinReport(byte[] data)
        {
            if (data != null)
            {
                this.data = data;
                decode();
            }
        }

        public FinReport(string filename)
        {
            loadfromfile(filename);
            decode();
        }

        public double getvalue(ele_names ele)
        {
            return elements[(int)ele].amount;
        }

        public double getcount(ele_names ele)
        {
            //Auto fudge the TTL entries for correct scalar
            if (ele > ele_names.ADJUST_TTL && ele < ele_names.PLU_LEVEL1_TTL)
            {
                return elements[(int)ele].count * 100;
            }
            else
            {
                return elements[(int)ele].count;
            }
        }

        public double getgrand()
        {
            return grand;
        }

        public override void decode()
        {

            // Remove first 00 byte then chunk by 12, not sure why we need to remove one
            // byte, makes me wonder if there is an error elsewhere generating crap?

            byte[] newArray = data.Skip(1).ToArray();

           List<List<byte>> chunks = ECRComms.chunk(newArray.ToList(), 12);

           int index = 0;

           foreach(List<byte> bs in chunks)
           {
               FinReportElement element = new FinReportElement();
               Array.Copy(bs.ToArray(),0, element.data,0,12);
               element.decode();
               elements.Add(element);
               index++;
           }

            grand = ECRComms.extractfloat3(data, 0x465); 
        }

        public override void encode()
        {
        }

        public void dump()
        {
            Console.WriteLine(String.Format("+PLU TTL {0} £{1}", getcount(ele_names.PLU_TTL),getvalue(ele_names.PLU_TTL)));
            Console.WriteLine(String.Format("+ADJUST TTL {0}£{1}", getcount(ele_names.ADJUST_TTL), getvalue(ele_names.ADJUST_TTL)));
            Console.WriteLine(String.Format("%1 {0} £{1}", getcount(ele_names.DISCOUNT1), getvalue(ele_names.DISCOUNT1)));
            Console.WriteLine(String.Format("NET SALE {0} £{1}", getcount(ele_names.NETSALE), getvalue(ele_names.NETSALE)));
            Console.WriteLine(String.Format("RETURN {0} £{1}", getcount(ele_names.RETURN), getvalue(ele_names.RETURN)));
            Console.WriteLine(String.Format("VOID {0} £{1}", getcount(ele_names.VOID_MODE), getvalue(ele_names.VOID_MODE)));
            Console.WriteLine(String.Format("CANCEL {0} £{1}", getcount(ele_names.CANCEL), getvalue(ele_names.CANCEL)));
            Console.WriteLine(String.Format("GROSS SALES £{0} ", getvalue(ele_names.GROSSSALES)));
            Console.WriteLine(String.Format("CASH SALES {0} £{1}", getcount(ele_names.CASHSALES), getvalue(ele_names.CASHSALES)));
            Console.WriteLine(String.Format("NOSALE {0} ", getcount(ele_names.NOSALE)));
            Console.WriteLine(String.Format("CASH-IN-D {0} £{1}", getcount(ele_names.CASH_IN_D), getvalue(ele_names.CASH_IN_D)));
            Console.WriteLine(String.Format("CHG1-IN-D {0} £{1}", getcount(ele_names.CHG1_IN_D), getvalue(ele_names.CHG1_IN_D)));
            Console.WriteLine(String.Format("CHG1 SALES {0} £{1}", getcount(ele_names.CHG1_SALES), getvalue(ele_names.CHG1_SALES)));
            Console.WriteLine(String.Format("PLU LEVEL1 TTL {0} £{1}", getcount(ele_names.PLU_LEVEL1_TTL), getvalue(ele_names.PLU_LEVEL1_TTL)));

            Console.WriteLine(String.Format("GRAND {0} ", grand));
        }
    }
}
