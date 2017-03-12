using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libECRComms
{

    public class PLUReportEntry
    {
        public string Description = "";
        public barcode PLU;
        public double quantity;
        public double value;

        public string ToString()
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

        public PLUReport(bool simulation)
        {
            if (simulation == true)
            {
                Console.WriteLine("PLU report simulation==true");
                entries = new List<PLUReportEntry>();

                PLUReportEntry entry = new PLUReportEntry();
                entry.PLU = new barcode();
                entry.PLU.fromtext("5025572020824");
                entry.quantity = 1;
                entry.value = 5.99;
                entry.Description = "TEST PRODUCT 2";
                entries.Add(entry);

                total = 5.99;
            }

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

                double qty = ECRComms.extractfloat4(lb.ToArray(), 0x1b);

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
            foreach (PLUReportEntry p in entries)
            {
                Console.Write(String.Format(" = {0} {1} {2} {3} \n", p.PLU.scode, p.Description, p.quantity, p.value));

            }

        }
    }


    public class FinReportElement : data_serialisation
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
        // ER380M UK list
        // See manual "Financial Report Message" to align items with this list
        public enum ele_names
        {
            PLU_TTL = 0,
            PLU_TTL_MINUS = 1,
            ADJUST_TTL = 2,
            NON_TAX = 3,
            TAX1_SALES = 4,
            TAX2_SALES = 5,
            TAX3_SALES = 6,
            TAX4_SALES = 7,
            TAX1 = 8,
            TAX2 = 9,
            TAX3 = 10,
            TAX4 = 11,
            NET_TAX1 = 12,
            NET_TAX2 = 13,
            NET_TAX3 = 14,
            NET_TAX4 = 15,
            XMPT1_SALES = 16,
            XMPT2_SALES = 17,
            XMPT3_SALES = 18,
            XMPT4_SALES = 19,
            ANALYSIS2 = 20,
            ANALYSIS3 = 21,
            ANALYSIS1 = 22,
            DISCOUNT1 = 23,
            DISCOUNT2 = 24,
            DISCOUNT3 = 25,
            DISCOUNT4 = 26,
            DISCOUNT5 = 27,
            NETSALE = 28,
            CREDIT_TAX1 = 29,
            CREDIT_TAX2 = 30,
            CREDIT_TAX3 = 31,
            CREDIT_TAX4 = 32,
            FDS_CREDIT = 33,
            RETURN = 34,
            ERROR_CORR = 35,
            PREVIOUS_VD = 36,
            VOID_MODE = 37,
            CANCEL = 38,
            GROSSSALES = 39,
            CASHSALES = 40,
            CHECKSALES = 41,
            RA_1 = 42,
            RA_2 = 43,
            RA_3 = 44,
            PO_1 = 45,
            PO_2 = 46,
            PO_3 = 47,
            HASH_TTL = 48,
            AUDACTION = 49,
            NOSALE = 50,
            CASH_IN_D = 51,
            CHEQUE_IN_D = 52,
            FDS_IN_D = 53,
            CHG1_IN_D = 54,
            CHG2_IN_D = 55,
            CHG3_IN_D = 56,
            CHG4_IN_D = 57,
            CHG5_IN_D = 58,
            CHG6_IN_D = 59,
            CHG7_IN_D = 60,
            CHG8_IN_D = 61,
            CHG1_SALES = 62,
            CHG2_SALES = 63,
            CHG3_SALES = 64,
            CHG4_SALES = 65,
            CHG5_SALES = 66,
            CHG6_SALES = 67,
            CHG7_SALES = 68,
            CHG8_SALES = 69,
            FOREIGN_1 = 70,
            FOREIGN_2 = 71,
            FOREIGN_3 = 72,
            FOREIGN_4 = 73,
            DWR_TTL = 74,
            PROMO = 75,
            WASTE = 76,
            TIP = 77,
            TRAIN_TTL = 78,
            BALFORWARD = 79,
            GUESTS = 80,
            PBAL = 81,
            CHECKS_PAID = 82,
            SERVICE = 83,
            MIXANDMATCH = 84,
            PLU_LEVEL1_TTL = 85,
            PLU_LEVEL2_TTL = 86,
            MOD_1_TTL = 87,
            MOD_2_TTL = 88,
            MOD_3_TTL = 89,
            MOD_4_TTL = 90,
            MOD_5_TTL = 91,
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
            if ((int)ele > elements.Count)
            {
                Console.WriteLine("Element not found");
                return 0;
            }

            return elements[(int)ele].amount;
   
        }

        public double getcount(ele_names ele)
        {

            if((int)ele>elements.Count)
            {
                Console.WriteLine("Element not found");
                return 0;
            }

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

            foreach (List<byte> bs in chunks)
            {
                try
                {
                    FinReportElement element = new FinReportElement();
                    Array.Copy(bs.ToArray(), 0, element.data, 0, 12);
                    element.decode();
                    elements.Add(element);
                    index++;
                }
                catch(Exception e)
                {
                    Console.WriteLine("Errors");
                }
            }

            try
            {
                grand = ECRComms.extractfloat3(data, 0x465);
            }
            catch(Exception e)
            {
                Console.WriteLine("Grand failed");
            }
        }

        public override void encode()
        {
        }

        public void dump()
        {

            int i = 0;

            
            foreach (FinReportElement ele in elements)
            {

                ele_names name = (ele_names)i;

                Console.WriteLine(String.Format("{0}\t{1}\t{2}",name.ToString(), getcount(name), getvalue(name)));
                i++;
            }


            
            Console.WriteLine("***********");

            Console.WriteLine(String.Format("GRAND {0} ", grand));

            Console.WriteLine("***********");
        }
    }

    public class TimeReportElement : data_serialisation
    {

        public int count;
        public double amount;

        public TimeReportElement()
        {
            data = new byte[12];

        }
        public override void decode()
        {
            count = ECRComms.extractint2(data.ToArray(), 1);
            amount = ECRComms.extractfloat3(data.ToArray(), 9);
        }

        public override void encode()
        {
        }
    }

    public class TimeReport : data_serialisation
    {

        List<TimeReportElement> elements = new List<TimeReportElement>();

        public TimeReport(byte[] data)
        {
            if (data != null)
            {
                this.data = data;
                decode();
            }
        }

        public override void decode()
        {
            List<List<byte>> chunks = ECRComms.chunk(this.data.ToList(), 12);

            foreach (List<byte> bs in chunks)
            {
                TimeReportElement element = new TimeReportElement();
                Array.Copy(bs.ToArray(), 0, element.data, 0, 12);
                element.decode();
                elements.Add(element);
            }
        }

        public override void encode()
        {
        }

        public void dump()
        {
            DateTime time = new DateTime(2000, 1, 1, 0, 0, 0);
            TimeSpan span = new TimeSpan(1, 0, 0);

            foreach(TimeReportElement t in elements)
            {
                Console.WriteLine(String.Format("{0} - {1} Count {2} Total {3}", time.ToShortTimeString(), (time + span).ToShortTimeString(), t.count, t.amount));
                time += span;
            }


        }

    }

}
