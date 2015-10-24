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

// ******************* PROGRAM 10 STOCK DATA ************************

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
