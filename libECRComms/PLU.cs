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
    along with ECRComms.  If not, see <http://www.gnu.org/licenses/>.

    Copyright (c) 2013 Robin Cornelius <robin.cornelius@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ER380M PLU format 55 bytes??
/*
 * |----BARCODE ------------|---------------DESCRIPTION---------------------------|
 * 00 3F 42 0F 00 FF E0 F5 05 42 42 42 42 42 42 42 42 42 42 42 42 42 42 42 42 42 42 
 * 
 * N1| --Grp--|
 * 00 01 00 00 03 00 7C 0C 00 00 00 00 00 00 00 00 00 00 
 * 
 * |-Price 1--|--Price 2--| Null
 * FF E0 F5 05 FF E0 00 00  00
 * 
 */

/* ER-230 PLU format 46 bytes
 
 * |----BARCODE ------------|---------------DESCRIPTION---------------------------|
 * 00 3F 42 0F 00 FF E0 F5 05 42 42 42 42 42 42 42 42 42 42 42 42 42 42 42 42 42 42 
 
 * // Price end is +32 from start of description
 * // 41 40
  
 
 */


namespace libECRComms
{

    public abstract class data_serialisation
    {
        public byte[] data;

        public void frombytes(byte[] data)
        {
            this.data = data;
            decode();
        }

        public byte[] tobytes()
        {
            decode();
            return data;
        }

        public abstract void decode();
        public abstract void encode();

    }

    public abstract class PLUcommon : data_serialisation
    {

        public string description;
        public barcode PLUcode = new barcode();
        public barcode linkPLUcode = new barcode();
        public int price;
        public int price2;
        public int[] groups = new int[3];
        public int mixandmatch;
        public int autotare;
        public int status;
    }


    public class barcode : data_serialisation
    {
        public long code;
        public const int Length = 8;
        public string scode;

        public barcode()
        {
            data = new byte[8];
        }

        public void fromtext(string text)
        {
            if (long.TryParse(text, out code))
            {
                encode();
                decode();
            }

        }

        //8 Bytes
        public override void decode()
        {
            long upper = data[0] + (data[1] << 8) + (data[2] << 16);
            // data[4] is padding 00
            long lower = data[4] + (data[5] << 8) + (data[6] << 16) + (data[7] << 24);

            scode = String.Format("{0:D5}{1:D8}", upper, lower);

            code = long.Parse(scode);
            
        }

        public override void encode()
        {
            long hi = code / 100000000;
            long lo = code % 100000000;

            data[0] = (byte)(hi);
            data[1] = (byte)((hi >> 8));
            data[2] = (byte)((hi >> 16));

            data[4] = (byte)((lo));
            data[5] = (byte)((lo >> 8));
            data[6] = (byte)((lo >> 16));
            data[7] = (byte)((lo >> 24));

        }
    }

  
    public class ER230_PLU : PLUcommon
    {
        public const int Length = 46;

        public ER230_PLU()
        {
            data = new byte[Length];
        }

        public override void decode()
        {
            Buffer.BlockCopy(data, 1, PLUcode.data, 0, barcode.Length);
            PLUcode.decode();

            description = Encoding.UTF8.GetString(data, 9, 18);

            groups[0] = data[22];
            groups[1] = data[23];
            groups[2] = data[24];

            status = (data[28] << 24) + (data[27] << 16) + (data[26] << 8) + data[25];

            autotare = data[29];

            Buffer.BlockCopy(data, 30, linkPLUcode.data, 0, barcode.Length);
            linkPLUcode.decode();

            mixandmatch = data[38];

            price = data[39] + (data[40] << 8) + (data[41] << 16) + (data[42] << 24);
            price2 = data[43] + (data[44] << 8) + (data[45] << 16); // One assumes price2 has a lower max limit than price due to no more packet


        }

        public override void encode()
        {
            PLUcode.encode();
            Buffer.BlockCopy(PLUcode.data, 0, data, 1, barcode.Length);

            byte[] b2 = System.Text.Encoding.ASCII.GetBytes(description);
            System.Buffer.BlockCopy(b2, 0, data, 9, description.Length < 18 ? description.Length : 18);

            data[22] = (byte)groups[0];
            data[23] = (byte)groups[1];
            data[24] = (byte)groups[2];

            status = (data[28] << 24) + (data[27] << 16) + (data[26] << 8) + data[25];

            data[25] = (byte)status;
            data[26] = (byte)(status >> 8);
            data[27] = (byte)(status >> 16);
            data[28] = (byte)(status >> 24);

            data[29] = (byte)autotare;

            Buffer.BlockCopy(linkPLUcode.data, 0,data, 30, barcode.Length);
            linkPLUcode.encode();

            data[38] = (byte)mixandmatch;

            data[39] = (byte)price;
            data[40] = (byte)(price>>8);
            data[41] = (byte)(price>>16);
            data[42] = (byte)(price>>24);

            data[43] = (byte)price;
            data[44] = (byte)(price>>8);
            data[45] = (byte)(price>>16);

        }
    }

    public class ER380M_PLU : PLUcommon
    {
        public const int Length = 55;

        public ER380M_PLU()
        {
            data = new byte[Length];
        }

        public override void decode()
        {
            Buffer.BlockCopy(data, 1, PLUcode.data, 0, barcode.Length);
            PLUcode.decode();

            description = Encoding.UTF8.GetString(data, 9, 18);

            Buffer.BlockCopy(data, 36, linkPLUcode.data, 0, barcode.Length);
            PLUcode.decode();

           

        }

        public override void encode()
        {
        }

    }


    [Serializable]
    public class PLU  
    {
       public byte[] data =new byte[46];

        void setdata(byte[] indata)
        {
            data = indata;
        }

        public string getPLUcode()
        {
            //data [0] is padding 00
            long upper = data[1] + (data[2]<<8) + (data[3]<<16);
            // data[4] is padding 00
            long lower = data[5] + (data[6] << 8) + (data[7] << 16) + (data[8] << 24);

            string code = String.Format("{0}{1}",upper,lower);

            return code;
        }

        public string getLINKPLUcode()
        {
            //data [35] is padding 00 (its the auto tare flag)
            long upper = data[36] + (data[37] << 8) + (data[38] << 16);
            // data[39] is padding 00
            long lower = data[40] + (data[41] << 8) + (data[42] << 16) + (data[43] << 24);

            string code = String.Format("{0}{1}", upper, lower);

            return code;
        }

        public int getAutoTare()
        {
            return data[35];
        }

        public int getMixMatch()
        {
            return data[44];
        }

        public string getDescription()
        {
            return Encoding.UTF8.GetString(data,9, 18);
        }

        public int getPrice1()
        {
            //offset 45    
            return data[48]+(data[49]<<8)+(data[50]<<16)+(data[51]<<24);
        }

        public int getPrice2()
        {

            //return data[52] + (data[53] << 8);
            return 0;
        }

        //Groups are in 28,29,30
        //28 29 30
        public string getGroups()
        {
            return String.Format("{0} {1} {2}", data[30], data[29], data[28]); 
        }

        public int getGroup(int index)
        {
            if (index < 0 || index > 3)
                return 0;

            return data[30 - index];
        }

        //status appears to be 31,32,33,34

        public string getStatus()
        {
            return String.Format("{0:x2} {1:x2} {2:x2} {3:x2}", data[31], data[32], data[33],data[34]); 
        }

        public int getiStatus()
        {
            int x = 0;
            x = (data[34] << 24) + (data[33] << 16) + (data[32] << 8) + data[31];


            return x;
        }


        public void setDescription(string description)
        {

            byte[] b2 = System.Text.Encoding.ASCII.GetBytes(description);
            
            System.Buffer.BlockCopy(b2, 0, data, 9, description.Length<18?description.Length:18);
        }

       public void  setPLUcode(string code)
        {
            //data [0] is padding 00
           // long upper = data[1] + (data[2] << 8) + (data[3] << 16);
            // data[4] is padding 00
          //  long lower = data[5] + (data[6] << 8) + (data[7] << 16) + (data[8] << 24);


            long val;

            if (long.TryParse(code, out val))
            {

                long hi = val / 100000000;
                long lo = val % 100000000;

                data[1] = (byte)(hi);
                data[2] = (byte)((hi >> 8));
                data[3] = (byte)((hi >> 16));

                data[5] = (byte)((lo));
                data[6] = (byte)((lo >> 8));
                data[7] = (byte)((lo >> 16));
                data[8] = (byte)((lo >> 24));
            }
        }

       public void setPrice1(string price)
        {


            float fprice;
            if (float.TryParse(price, out fprice))
            {
                int iprice = (int)(fprice * 100);
                data[48] = (byte)(iprice & 0x000000FF);
                data[49] = (byte)((iprice >> 8) & 0x000000FF);
                data[50] = (byte)((iprice >> 16) & 0x000000FF);
                data[52] = (byte)((iprice >> 24) & 0x000000FF);
            }


        }

    }
}
