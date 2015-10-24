using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libECRComms
{
    public class StockData : data_serialisation
    {

        public const int Length = 12;

        public barcode PLUcode = new barcode();
        int int_quantity; //(real qty * 100)
        public decimal qty;


        public StockData()
        {
            data = new byte[Length];
        }

        public override void decode()
        {
            Buffer.BlockCopy(data, 1, PLUcode.data, 0, barcode.Length);
            PLUcode.decode();

            int_quantity = data[9] + (data[10] << 8)  + (data[11] << 16);

            qty = (decimal)int_quantity / (decimal)100.0;

            //Console.WriteLine(String.Format("Decoded {0} Qty {1:0.00} ", PLUcode.scode, qty));

        }

        public override void encode()
        {

            PLUcode.encode();
            Buffer.BlockCopy(PLUcode.data,0,data,1,barcode.Length);

            int_quantity = (int)qty;
            int_quantity *= 100;
            data[9] = (byte)int_quantity;
            data[10] = (byte)(int_quantity >> 8);
            data[11] = (byte)(int_quantity >> 16);
  


        }

    }
}
