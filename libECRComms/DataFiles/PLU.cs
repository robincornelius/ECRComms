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


// ******************* PROGRAM 0 PLU DATA ************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

// ER380M PLU format 54 bytes on my eeprom anyway i know others are different
// The ER380M PC app makes 52 byte PLUS with a different layout

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

/*
 * 00-08 (8) Barcode
 * 09-27 (18) Description
 * 28-30 (3) Groups
 * 31-34 (4) Status
 * 35 (1) Tare
 * 36-39 (4) ??????
 * 40 (1) Link
 * 41-45 (5) ???
 * 46 (1) Mix and Match ?????
 * 47-49 (3) Price Halo
 * 50-52 (3) Price Level 2 > ??? Guess ??? 
 * 53 (1) NULL
 * 
 * Mix and match is missing (single byte)
 * Stock is this in  the PLU file or a seperate table only?
 * Price2 seems to be around but not accessable via till keyboard it should live around 50-53 area
 * 
 * 
/* ER-230 PLU format 46 bytes
 
 * |----BARCODE ------------|---------------DESCRIPTION---------------------------|
 * 00 3F 42 0F 00 FF E0 F5 05 42 42 42 42 42 42 42 42 42 42 42 42 42 42 42 42 42 42 
 
 * // Price end is +32 from start of description
 * // 41 40
  
 
 */

/* Status format 
 * 
 * IGNORE THE FUCKING MANUAL
 * 
 * BYTE 1 0x03
 * 0x01 PLU is preset *
 * 0x02 PLU is override *
 * 0x04 PLU is tax rate 1
 * 0x08 PLU is tax rate 2
 * 0x10 PLU is tax rate 3
 * 0x20 PLU is tax rate 4
 * 0x40 PLU is food stamp eligable
 * 0x80 NEGATIVE ITEM
 * 
 * BYTE 2 0x00
 * 0x01 HASH
 * 0x02 SINGLE ITEM
 * 0x04 COMP NON ADD NUMBER
 * 0x08 GALLONAGE
 * 0x10 INVENTORY
 * 0x20 INACTIVE

 * BYTE 3 0x7c
 * 0x01 Condiment
 * 0x02 Compulsory Condiment entry
 * 0x04 PLU on recipt *
 * 0x08 N/A
 * 0x10 PLU on check *
 * 0x20 Price on recipt *
 * 0x40 Price on check *
 * 0x80 disable promo
 * 
 * BYTE 4 0x0c
 * 0x01 Allow discounts/Counter not reset
 * 0x02 PLU is preset override MGR
 * 0x04 Price change item *?? 
 * 0x08 ??????? *
 * TBH all seems like bullshit at this point just set to 0x0c
*/




namespace libECRComms
{

    public enum statusbytes
    {
        status_PLU_preset = 0x01,
        status_PLU_overide = 0x02,
        status_PLU_tax1 = 0x04,
        status_PLU_tax2 = 0x08,
        status_PLU_tax3 = 0x10,
        status_PLU_tax4 = 0x20,
        status_PLU_foodstamp = 0x40,
        status_PLU_neg = 0x80,

        status_hash = 0x0100,
        status_singleitem = 0x0200,
        status_nonadd = 0x0400,
        status_gallomnage = 0x0800,
        status_inventory = 0x1000,
        status_inactive = 0x2000,

        status_condiment = 0x010000,
        status_compulsary_condiment = 0x020000,
        status_PLU_on_rcp = 0x040000,
        status_PLU_on_check = 0x100000,
        status_price_on_rcp = 0x200000,
        status_price_on_check = 0x400000,
        status_disable_promo = 0x800000,

        status_preset_override_mgr  = 0x02000000,
        status_price_change_item    = 0x04000000,
        status_something_important  = 0x08000000,
        status_allow_discounts      = 0x10000000,

    }

    public static class extension
    {
       public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }

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

        public void savetofile(String filename)
        {

            System.IO.StreamWriter myFile = new System.IO.StreamWriter(filename);

            foreach (byte b in data)
            {
                myFile.Write(String.Format("{0:x2}", b));

            }

            myFile.Close();

        }

        public void loadfromfile(String filename)
        {
            System.IO.StreamReader myFile = new System.IO.StreamReader(filename);
            string myString = myFile.ReadToEnd();
            myFile.Close();
            data = ECRComms.StringToByteArrayFastest(myString);
            myFile.Close();

            decode();
        }

        public abstract void decode();
        public abstract void encode();

    }

    class PLUFactory
    {
        public static PLUcommon Get(MachineIDs id)
        {
            switch (id)
            {
                case MachineIDs.ER230:
                    return new ER230_PLU();

                case MachineIDs.ER380M_UK:
                    return new ER380M_PLU();

                default:
                    return null;

            }

        }

    }

    public abstract class PLUcommon : data_serialisation
    {

        public string description;
        public barcode PLUcode = new barcode();
        public barcode linkPLUcode = new barcode();
        public decimal price;
        public decimal price2;
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

            try
            {
                code = long.Parse(scode);
            }
            catch
            {
                Console.WriteLine("Failed to parse to valid number");
            }
            
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
            status = 0x0c7c0003; //default status

            //status = (int) (statusbytes.status_PLU_preset | statusbytes.status_PLU_overide | statusbytes.status_PLU_on_rcp | statusbytes.status_PLU_on_check | statusbytes.status_price_on_rcp | statusbytes.status_price_on_check | statusbytes.status_price_change_item | statusbytes.status_something_important);
        }

        public override void decode()
        {
            Buffer.BlockCopy(data, 1, PLUcode.data, 0, barcode.Length);
            PLUcode.decode();

            char[] desc = new char[18 - 9];

            int posx=0;

            for (int pos = 9; pos < 18; pos++,posx++)
            {
                if (data[pos] > 0x20 && data[pos] < 0x7A)
                {
                    desc[posx] = (char)data[pos];
                }
                else
                {
                    desc[posx] = (char) 0x20;
                }
            }

            description = new string(desc);

            groups[0] = data[22];
            groups[1] = data[23];
            groups[2] = data[24];

            //fixme check byte order 
            status = (data[25] << 24) + (data[26] << 16) + (data[27] << 8) + data[28];

            autotare = data[29];

            Buffer.BlockCopy(data, 30, linkPLUcode.data, 0, barcode.Length);
            linkPLUcode.decode();

            mixandmatch = data[38];

            int iprice;
            int iprice2;

            iprice = data[39] + (data[40] << 8) + (data[41] << 16) + (data[42] << 24);
            iprice2 = data[43] + (data[44] << 8) + (data[45] << 16); // One assumes price2 has a lower max limit than price due to no more packet

            price = (decimal)(iprice / 100.0);
            price2 = (decimal)(iprice2 / 100.0);

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

           // status = (data[28] << 24) + (data[27] << 16) + (data[26] << 8) + data[25];

            data[28] = (byte)status;
            data[27] = (byte)(status >> 8);
            data[26] = (byte)(status >> 16);
            data[25] = (byte)(status >> 24);

            data[29] = (byte)autotare;

            Buffer.BlockCopy(linkPLUcode.data, 0,data, 30, barcode.Length);
            linkPLUcode.encode();

            data[38] = (byte)mixandmatch;

            int iprice = (int)(price * (decimal)100.0);

            data[39] = (byte)iprice;
            data[40] = (byte)(iprice>>8);
            data[41] = (byte)(iprice>>16);
            data[42] = (byte)(iprice>>24);

            data[43] = (byte)iprice;
            data[44] = (byte)(iprice>>8);
            data[45] = (byte)(iprice>>16);

        }
    }

    public class ER380M_PLU : PLUcommon
    {
        public const int Length = 54;

        public ER380M_PLU()
        {
            data = new byte[Length];

            status = 0x107c0003; //default status
        }

        public override void decode()
        {
          
            //* 00-08 (8) Barcode

            Buffer.BlockCopy(data, 1, PLUcode.data, 0, barcode.Length);
            PLUcode.decode();

            //* 09-27 (18) Description

            description = Encoding.UTF8.GetString(data, 9, 18);

            //* 28-30 (3) Groups
            groups[0] = data[28];
            groups[1] = data[29];
            groups[2] = data[30];

            //* 31-34 (4) Status
            status = (data[34] << 24) + (data[33] << 16) + (data[32] << 8) + data[31];

            //* 35 (1) Tare

            autotare = data[35];

            //* 36 LINK 

            //Dont think this fits correctly one byte short at the start because of tare is in the way

            Buffer.BlockCopy(data, 36, linkPLUcode.data, 0, barcode.Length);
            PLUcode.decode();

            //44,45,46 empty

            int iprice;
            int iprice2;

            iprice = data[47] + (data[48] << 8) + (data[49] << 16);
            price = (decimal)(iprice / 100.0);

            //* 50-52 (3) Price Level 2 > ??? Guess ??? 

            iprice2 = data[50] + (data[51] << 8) + (data[52] << 16);
            price2 = (decimal)(iprice2 / 100.0);

            //* 53 (1) NULL
         
        }

        int[] GetIntArray(int num)
        {
            List<int> listOfInts = new List<int>();
            while (num > 0)
            {
                listOfInts.Add(num % 10);
                num = num / 10;
            }
            listOfInts.Reverse();
            return listOfInts.ToArray();
        }

        public override void encode()
        {

             //* 00-08 (8) Barcode

            PLUcode.encode();
            Buffer.BlockCopy(PLUcode.data, 0, data, 1, barcode.Length);

             //* 09-27 (18) Description

            byte[] b2 = System.Text.Encoding.ASCII.GetBytes(description);
            System.Buffer.BlockCopy(b2, 0, data, 9, description.Length < 18 ? description.Length : 18);

             //* 28-30 (3) Groups

            data[28] = (byte)groups[0];
            data[29] = (byte)groups[1];
            data[30] = (byte)groups[2];

             //* 31-34 (4) Status
 
            data[31] = (byte)status;
            data[32] = (byte)(status >> 8);
            data[33] = (byte)(status >> 16);
            data[34] = (byte)(status >> 24);

             //* 35 (1) Tare

            data[35] = (byte)autotare;

            //* LINK 

            //Dont think this fits correctly one byte short at the start because of tare is in the way
            linkPLUcode.encode();
            Buffer.BlockCopy(linkPLUcode.data, 0, data, 36, barcode.Length);
           
            //44,45,46 empty

            int iprice = (int)(price * (decimal)100.0);
            int iprice2 = (int)(price2 * (decimal)100.0);

            data[47] = (byte)iprice;
            data[48] = (byte)(iprice >> 8);
            data[49] = (byte)(iprice >> 16);

             //* 50-52 (3) Price Level 2 > ??? Guess ??? 

            data[50] = (byte)iprice2;
            data[51] = (byte)(iprice2 >> 8);
            data[52] = (byte)(iprice2 >> 16);

             //* 53 (1) NULL
            data[53] = 0;
         

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
