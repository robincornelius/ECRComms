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

    Copyright (c) 2013-2015 Robin Cornelius <robin.cornelius@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.IO;

// SAMS4S ER range read/write functions
// Tested with ER-230 and ER-380M this interface provides a generic read/write that does not currenly care about file size
// No idea what other tills this may support with out someone to test.

namespace libECRComms
{

    public class ProgressEventArgs : EventArgs
    {
        public enum ProgressState
        {
            PROGRESS_SEND = 0,
            PROGRESS_DOWNLOAD = 2,
            PROGRESS_DONE = 3,
            PROGRESS_ERROR = 4,
            PROGRESS_UPLOAD = 5,
            PROGRESS_UPLOAD_TICK = 6,
            PROGRESS_DOWNLOAD_TICK = 7,
        }

        public ProgressState state;

        public int blockstotal;
        public int blocksdone;
        public ProgressEventArgs(ProgressState state, int blockstotal, int blocksdone)
        {
            this.state = state;
            this.blocksdone = blocksdone;
            this.blockstotal = blockstotal;
        }
    }

    public enum MachineIDs
    {
        ER230,
        ER380M_UK
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

    public class ECRComms : IDisposable
    {

        public delegate void ProgressEventHandler(object sender, EventArgs e);
        public event ProgressEventHandler Progress;

       private SerialPort port;

       public string commport = "COM1";
       public int baud = 9600;

       public bool portopen = false;
       public bool ecrok = false;

       public bool packetdebug = false;

       int blockstotal = 0;
       int blocksdone = 0;

       returnstatus retstatus;

       enum command
       {
           UPLOAD = 3, //ECR -> PC
           DOWNLOAD = 2, // PC -> ECR
           REPORT = 1
       }

       public enum program
       {
           PLU = 0,
           Group = 1,
           Tax = 2,
           System_Option = 3,
           Print_Option = 4,
           Function_Key = 5,
           Clerk = 6,
           Logo = 7,
           FINANCIAL_REPORT_LOGO = 8,
           CLERK_REPORT_LOGO = 9,
           STOCK = 10,
           MACRO = 11,
           MISC = 12, //Inc time
           MIXANDMATCH = 13
       }

        public enum reports
        {
            FINANCIAL = 0,
            TIME = 1,
            PLU = 2,
            CLARK = 3,
            GROUP = 4,
            DAY = 5,
            STOCK = 6,
            EJ = 7,
            SALES = 8,
        }

        public enum reporttype
        {
            X1 = 0,
            X2 = 1,
            Z1 = 2,
            Z2 = 3
        }

        //don't know what other statuses there are
        enum returnstatus
        {
            OK      = 0x06,
            NAK     = 0x15
        }


        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //return val;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }


        //24bit data
        public static double convertfloat3(byte[] data)
        {

            byte sign = 0x00;

            if ((data[2] & 0x80) == 0x80)
            {
                sign = 0xff;
            }

            Int32 ret = (data[0] << 0) + (data[1] << 8) + (data[2] << 16) + (sign <<24);
            double dret = (double)ret/100.0;
            return dret;
        }

        public static double extractfloat3(byte[] data, int pos)
        {
            byte[] amount = new byte[3];
            Buffer.BlockCopy(data, pos, amount, 0, 3);
            return ECRComms.convertfloat3(amount);
        }

        public static double extractfloat4(byte[] data, int pos)
        {
            byte[] amount = new byte[4];
            Buffer.BlockCopy(data, pos, amount, 0, 4);
            int ret = (amount[0] << 0) + (amount[1] << 8) + (amount[2] << 16) + (amount[3] << 24);
            return (double)ret / 100.0;

        }

        public static void putdecimal4(byte[] data, int pos, decimal value)
        {
            int ivalue = (int) (value * 100);
            putint4(data, pos, ivalue);
        }

        public static int extractint4(byte[] data, int pos)
        {
            byte[] amount = new byte[4];

            Buffer.BlockCopy(data, pos, amount, 0, 4);
            return (amount[0] << 0) + (amount[1] << 8) + (amount[2] << 16) + (amount[3] << 24);
        }

        public static int extractint3(byte[] data, int pos)
        {
            byte[] amount = new byte[3];

            Buffer.BlockCopy(data, pos, amount, 0, 3);
            return (amount[0] << 0) + (amount[1] << 8) + (amount[2] << 16);
        }

        public static int extractint2(byte[] data, int pos)
        {
            byte[] amount = new byte[2];

            Buffer.BlockCopy(data, pos, amount, 0, 2);
            int ret = (amount[0] << 0) + (amount[1] << 8);
            return ret;
        }

        public static int extractint1(byte[] data, int pos)
        {
            byte[] amount = new byte[2];

            Buffer.BlockCopy(data, pos, amount, 0, 2);
            int ret = (amount[0] << 0);
            return ret;
        }

        public static void putint4(byte[] data, int pos, int val)
        {
            data[pos] = (byte)val;
            data[pos + 1] = (byte)(val << 8);
            data[pos + 2] = (byte)(val << 16);
            data[pos + 3] = (byte)(val << 24);
        }

        public static void putint3(byte[] data, int pos, int val)
        {
            data[pos] = (byte) val;
            data[pos+1] = (byte)(val<<8);
            data[pos+2] = (byte)(val<<16);
        }

        public static void putint2(byte[] data, int pos, int val)
        {
            data[pos] = (byte)val;
            data[pos + 1] = (byte)(val << 8);
            data[pos + 2] = (byte)(val << 16);
        }

        public static void putint1(byte[] data, int pos, int val)
        {
            data[pos] = (byte)val;
        }

        public static string gettext(byte[] data, int pos, int len)
        {
            string txt = System.Text.Encoding.ASCII.GetString(data.SubArray(pos, len));
            txt = txt.Replace("\0", string.Empty);
            return txt;
        }

        public static void puttext(byte[] data, int pos, int len,string txt)
        {
            Array.Copy(Encoding.ASCII.GetBytes(txt), 0, data, pos, txt.Length < len ? txt.Length : len);
        }


       public bool init()
       {
           portopen = false;

           if (port != null && port.IsOpen)
               port.Close();

           try
           {
               port = new SerialPort(commport, baud, Parity.None, 8, StopBits.One);
               port.ReadTimeout = 25000;
               port.WriteTimeout = 5000;
               port.Handshake = Handshake.None;
               port.Open();
               portopen = true;
           }
           catch (Exception e)
           {
               return false;
           }

           return true;
       }

        public void close()
       {
           if (port != null)
               port.Close();

           port = null;


       }

       public void Dispose()
       {
           if (port != null)
               port.Close();

           port = null;

       }

       // summing the entire packet including checksum should result in 0xff
       bool testchecksum(List<byte> data)
       {
           byte csum = 0;

           foreach (byte b in data)
           {
               csum += b;
           }

           if (csum == 0xff)
           {
               if (packetdebug)
               {
                   Console.WriteLine("\nCheck sum test passed");
               }
               return true;
           }

           if (packetdebug)
           {
               Console.WriteLine("\nCheck sum test failed we got " + csum.ToString());
           }

           return false;

       }

       byte checksumpacket(List<byte> data)
        {
           byte csum = 0;

           foreach (byte b in data)
           {
               csum += b;
           }
   
           csum = (byte)(csum ^ 0xff);

           return csum;
           
        }

       void unescapepacket(List<byte> data)
        {
            int pos=0;
            List<int> escapelocations = new List<int>();

            foreach(byte b in data)
            {
                if(pos>0 && b==0x10 && data[pos-1]==0x10) 
                {
                    //remove extra escape but we can't do it now as we are enumerating
                    escapelocations.Add(pos);
                }
                pos++;
            }

            pos = 0;
            foreach (int escpos in escapelocations)
            {
                data.RemoveAt(escpos - pos);
                pos++;
            }

        }

       //Only call on data portion of packet, do not include command header and footer
       //need to *double* check this what happens if the index is 0x10? will this cause an issue???
       //clearly we need to not escape the start of packet 0x10 0x02 but the other fields may need it
       int escapepacket(List <byte> data, int skipbytes=0)
       {

            List<int> escapelocations = new List<int>();

            int pos = 0;
            foreach (byte b in data)
            {
                if (pos >= skipbytes)
                {
                    if (b == 0x10)
                    {
                        escapelocations.Add(pos);
                    }
                }
                 pos++;
            }

            pos = 0;
            foreach (int escpos in escapelocations)
            {
                data.Insert(escpos + pos,0x10);
                pos++;
            }

            return escapelocations.Count;

        }

        List<byte> buildprogramcommand(command cmd, reporttype rpt, program pgm, List<data_serialisation> payload, int blocks=0, int totalblocks=0,int thisblock=0)
        {
           
            // Add command header
            byte[] header = null;

            switch(cmd)
            {
                case command.UPLOAD:
                case command.REPORT:
                    //TO PC
                    header = new byte[] { 0x10, 02, 01, 00, 00, 00, 00, 00, 00, 00, (byte)cmd, (byte)rpt, 00, 00, (byte)pgm, 01, 00, 00, 00, 01 };
                    break;

                case command.DOWNLOAD:
                    //TO ECR                                                                                     
                    header = new byte[] { 0x10, 02, 01, 00, 00, 00, 00, 00, 00, 00, (byte)cmd, 00, 00, 00, (byte)pgm, (byte)totalblocks, (byte)(totalblocks >> 8), (byte)thisblock, (byte)(thisblock >> 8), (byte)blocks };
                    break;
            }
     
            List<byte> packet = new List<byte>(header);

            if (payload != null && payload.Count > 0)
            {
                foreach (data_serialisation p in payload)
                {
                    packet.AddRange(p.data);
                }
            }

            // checkme should we start escaping after byte 2, bytes 0 and 1 are the 
            // SOP header 0x10,0x02 but after that its fair game???
            // changed to account for this as i'm sure this is why we got some NAKs during upload!!!
            int count = escapepacket(packet, 3);
           
            addfooter(packet,count);

            return packet;
        }

        void addfooter(List<byte> data,int escapecount)
        {
            data.Add(0x00);
            data.Add(0x10);
            data.Add(0x03); // end of packet

            byte csum = checksumpacket(data);
            csum += (byte)(escapecount * 0x10);

            data.Add(csum);
        }

        public List<byte> getprogram(program prg)
        {
         
            if (checkstatus() == false)
            {
                Exception e = new Exception("ECR status bad, aborted");
                throw e;
            }

            List<byte> returneddata = packetshakeupload(prg);

            return returneddata;
        }


        // Not all reports support X1,X2,Z1 and X2, some are X/Z only
        // EJ is special its either an X1 or a Z2 type report for keep or clear
        public List<byte> getreport(reports rpt,reporttype rt)
        {

            if (checkstatus() == false)
            {
                Exception e = new Exception("ECR status bad, aborted");
                throw e;
            }


            List<byte> returneddata = packetshakeupload((program)rpt,true,rt); //meh nasty cast of enums

            return returneddata;
        }

        public static List<List<byte>> chunk(List<byte> data, int size)
        {

            List<List<byte>> packets = new List<List<byte>>();

            int offset = 0;
            byte[] allbytes =  data.ToArray();

            while (offset <= allbytes.Length - size)
            {
                List<byte> apacket = new List<byte>();

                //Is there really not a built in way to do this?
                for(int item=offset;item<(offset+size);item++)
                {
                    apacket.Add(data[item]);
                }
                packets.Add(apacket);

                offset += size;
              
            }

            return packets;

        }

      
        int thisblock;
        int totalcount;

        public void setprogram(program prg,List <data_serialisation> payload)
        {
            
            if (checkstatus() == false)
            {
                Exception e = new Exception("ECR status bad, aborted");
                throw e;
            }

            packetshakedownload(prg, payload);
        }

        void sendACK()
        {
            byte[] data = { 0x10, 0x06 };
            port.Write(data, 0, data.Length);
        }

        public void sendNACK()
        {
            byte[] data = { 0x10, 0x15 };
            port.Write(data, 0, data.Length);
        }

        // Get data command
        // Send the upload request command then keep reading until we get a end of packet 0x00 0x10, then parse the header to see how many
        // more blocks are remaining and if not zero keep going

        int blocksremaining = 0;
        int badchecksum = 0;

        //Upload from ECR to PC
        List<byte> packetshakeupload(program prg, bool report = false, reporttype rt = reporttype.X1)
        {
            const int headersize = 20;
            badchecksum = 0;

            List<byte> totalpayload = new List<byte>();

            //Send the upload request
            port.DiscardInBuffer();


            byte[] packetb = null;
            if (report == false)
            {
                packetb = buildprogramcommand(command.UPLOAD,rt, prg, null).ToArray();
            }
            else
            {
                packetb = buildprogramcommand(command.REPORT,rt, prg, null).ToArray();
            }

            port.Write(packetb, 0, packetb.Length);

            bool first = true;

            //Read data untill we get a 0x00 0x10
            do
            {
                List<byte> rxbuffer = new List<byte>();

                byte[] bqueue = new byte[3];

                //If we find the magic code 0x10 0x03 its the end of the packet (apart from one more byte for checksum)
                //However if the 0x10 is prefixed with 0x10 it does not count its considered escaped

                while(!(bqueue[0]==0x03 && bqueue[1]==0x10 && bqueue[2]!=0x10))
                {
                    bqueue[2] = bqueue[1];
                    bqueue[1] = bqueue[0];
                    bqueue[0] = (byte)port.ReadByte();
                    rxbuffer.Add(bqueue[0]);
                }

                //If we got a 0x03 also read a check sum
                //If status is not a 0x03 we should exit here as WTF?
                if (bqueue[0] == 0x03)
                {
                    byte b = (byte)port.ReadByte();
                    rxbuffer.Add(b);
                }
                else
                {
                    Console.WriteLine("Invalid status");
                    OnProgress(ProgressEventArgs.ProgressState.PROGRESS_ERROR, 1, 1);
                    Exception e = new Exception("Invalid status " + bqueue[0].ToString() + " on get data, expecting 0x03");
                    throw e;
                }

                // unescape the packet now as we need to process the num blocks etc and this could be escaped
                unescapepacket(rxbuffer);

                // Parse the response header for number of blocks
                
                blocksremaining = (rxbuffer[16] << 8) + rxbuffer[15];
                int thisblock = (rxbuffer[18] << 8) + rxbuffer[17];

                int no_records = rxbuffer[19];

                Console.WriteLine(String.Format("NOW ON {0} remaining {2} blocksize {1}", thisblock, no_records, blocksremaining));

                if (rxbuffer[15] == 0)
                {
                    Console.WriteLine("Invalid No blocks to return 0");
                    OnProgress(ProgressEventArgs.ProgressState.PROGRESS_ERROR, 1, 1);
                    Exception e = new Exception("Invalid No blocks to return 0");
                    throw e;
                }

                if (first == true)
                {
                    first = false;
                    OnProgress(ProgressEventArgs.ProgressState.PROGRESS_UPLOAD, rxbuffer[15] - 1, 0);
                }
                else
                {
                    OnProgress(ProgressEventArgs.ProgressState.PROGRESS_UPLOAD_TICK, 0, thisblock);
                }

                if (packetdebug)
                {
                    Console.Write("\nPacket = \n");
                    int count = 0;
                    foreach (byte bx in rxbuffer)
                    {
                        Console.Write(string.Format("{0:x2} ", bx));
                        count++;
                        if (count == 32)
                        {
                            Console.Write("\n");
                            count = 0;
                        }

                    }
                }
   
                // Copy the payload out of the packet, first 20 bytes are header, last 4 are terminator status and checksum
                // we need to pass the last 4 to the checksum and escape routines for them to work correctly.

                List<byte> payload = rxbuffer;

                if (testchecksum(rxbuffer) == false)
                {
                    Console.WriteLine("*** BAD CHECKSUM **** retrying ....");

                   // OnProgress(ProgressEventArgs.ProgressState.PROGRESS_ERROR, 1, 1);

                    badchecksum++;
                    System.Threading.Thread.Sleep(250);

                    sendNACK();

                    if(badchecksum==10)
                    {
                         Exception e = new Exception("Packet checksum error x10, givving up");
                         throw e;
                    
                    }
                    
                }

                // remove last 4 bytes from data payload its the end of packet and checksum, data payload does not need this
                payload.RemoveRange(0, headersize);
                payload.RemoveRange(payload.Count - 4, 4);

                // Add payload to totalpayload;
                totalpayload.AddRange(payload);

                sendACK();

            }
            while (blocksremaining != 1);

            OnProgress(ProgressEventArgs.ProgressState.PROGRESS_DONE, 1, 1);

            return totalpayload;
        }

        // download from PC to ECR
        List<byte> packetshakedownload(program prg, List<data_serialisation> payload)
        {

            //Fix me we neen sensible decisions on download sizes, worst case is PLU
            //which on UK version is 54bytes
            int framesize = 20;
            //PLU payload is 20*54 = 1080 bytes

            //Other files may make more sense to do more at once
            if (prg == program.STOCK)
                framesize = 90; 

            int totalblocks = payload.Count / framesize;
            blockstotal = totalblocks;
            totalblocks++;

            blocksdone = 0;

            OnProgress(new ProgressEventArgs(ProgressEventArgs.ProgressState.PROGRESS_DOWNLOAD, blockstotal, blocksdone));

            List<data_serialisation> mini_payload = new List<data_serialisation>(); ;
            int count = 0;
            thisblock = 0;
            totalcount = 0;

            foreach (data_serialisation p in payload)
            {

                mini_payload.Add(p);

                count++;
                totalcount++;

                //Payload should not be more than 20 blocks on PLUS anyway
                if (count == framesize || totalcount == payload.Count)
                {
                    List<byte> packet = buildprogramcommand(command.DOWNLOAD,reporttype.X1, prg, mini_payload, count, totalblocks, thisblock);

                    if (packetdebug)
                    {
                        Console.WriteLine("\nNew packet to send :->\n");

                        foreach (byte b in packet)
                        {
                            Console.Write(String.Format("{0:X2} ", b));

                        }
                    }

                    //Small delay required here to prevent the ECR from returning NAK
                    System.Threading.Thread.Sleep(250); 

                    //Send the upload request
                    port.DiscardInBuffer();
                    port.Write(packet.ToArray(), 0, packet.Count);

                    //now look for the reply
                    if (packetdebug)
                    {
                        Console.WriteLine("\nEnd of packet\n");
                    }

                    byte[] reply = new byte[2];
                    port.Read(reply, 0, 2);

                    if (reply[0] != 0x10 && reply[1] != 0x06)
                    {
                        //FIXME we should really retry the upload when we get a NAK
                        Console.WriteLine("NAK recieved bad upload");
             
                        ProgressEventArgs ea = new ProgressEventArgs(ProgressEventArgs.ProgressState.PROGRESS_ERROR, 1, 1);
                        OnProgress(ea);

                        Exception e = new Exception("NAK recieved bad upload");
                        throw e;
                    }

                    blocksdone = thisblock;
                    thisblock++;

                    OnProgress(new ProgressEventArgs(ProgressEventArgs.ProgressState.PROGRESS_DOWNLOAD_TICK, blockstotal, blocksdone));

                    totalblocks--;
                    count = 0;
                    mini_payload.Clear();
                }

            }

            OnProgress(new ProgressEventArgs(ProgressEventArgs.ProgressState.PROGRESS_DONE, 0,0));

            return null;
        }

        public bool checkstatus()
        {
            try
            {

                Console.WriteLine("Checking status");
                                
                // technicaly this is just another command with cmd->pgm bytes set to 0x09
                byte[] status = { 0x10, 02, 01, 00, 00, 00, 00, 00, 00, 00, 09, 09, 09, 09, 09, 01, 00, 00, 00, 00, 00, 0x10, 0x03, 0xAB };


                port.DiscardInBuffer();
                port.DiscardOutBuffer();

                port.Write(status, 0, status.Length);
                System.Threading.Thread.Sleep(200);

                byte[] ret = new byte[2];

                ret[0] = (byte)port.ReadByte();
                ret[1] = (byte)port.ReadByte();

                int count = port.BytesToRead;

                retstatus = (returnstatus)ret[1];

                if (ret[0] == 0x10 && ret[1] == 0x06)
                {
                    System.Threading.Thread.Sleep(1000);
                    ecrok = true;
                    return true;
                }

                ecrok = false;
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("ECR Status exception " + e.Message);
                ecrok = false;
                OnProgress(new ProgressEventArgs(ProgressEventArgs.ProgressState.PROGRESS_ERROR, 0, 0));
                return false;
            }

        }

        // Invoke the Changed event; called whenever list changes
        protected void OnProgress(ProgressEventArgs e)
        {
            try
            {
                if (Progress != null)
                    Progress(this, e);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Shit happened .. " + ex.Message);
            }
        }

        protected void OnProgress(ProgressEventArgs.ProgressState state,int max,int count)
        {
            try
            {
                if (Progress != null)
                {
                    ProgressEventArgs e = new ProgressEventArgs(state, max, count);
                    Progress(this, e);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Shit happened .. " + ex.Message);
            }
        }

    }




}
