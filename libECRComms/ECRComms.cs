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
            PROGRESS_SEND=0,
            PROGRESS_DOWNLOAD=2,
            PROGRESS_DONE=3,
            PROGRESS_ERROR=4,
        }

        public ProgressState state;

        public int blockstotal;
        public int blocksdone;
        public ProgressEventArgs(ProgressState state,int blockstotal,int blocksdone)
        {
            this.state=state;
            this.blocksdone = blocksdone;
            this.blockstotal = blockstotal;
        }
    }

    public class ECRComms
    {
        public delegate void ProgressEventHandler(object sender, EventArgs e);
        public event ProgressEventHandler Progress;

       private SerialPort port;

       public string commport = "COM1";
       public int baud = 9600;

       public bool portopen = false;
       public bool ecrok = false;

       enum command
       {
           UPLOAD = 3, //ECR -> PC
           DOWNLOAD = 2 // PC -> ECR
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

       public bool init()
       {
           portopen = false;

           if (port != null && port.IsOpen)
               port.Close();

           try
           {
               port = new SerialPort(commport, baud, Parity.None, 8, StopBits.One);
               port.Open();
               portopen = true;
           }
           catch (Exception e)
           {
               return false;
           }

           port.ReadTimeout = 5000;
           port.WriteTimeout = 5000;

           return true;
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
               Console.Write("Check sum test passed");
               return true;
           }

           Console.Write("Check sum test failed");
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

        List<byte> buildprogramcommand(command cmd, program pgm, List<data_serialisation> payload, int blocks=0, int totalblocks=0,int thisblock=0)
        {
           
            // Add command header
            byte[] header = null;

            if (cmd == command.UPLOAD)
            {
                //TO PC
                header = new byte[] { 0x10, 02, 01, 00, 00, 00, 00, 00, 00, 00, (byte)cmd, 00, 00, 00, (byte)pgm, 01, 00, 00, 00, 01, 00 };
            }
            else //DOWNLOAD
            {
                //TO ECR                                                                                     
                header = new byte[] { 0x10, 02, 01, 00, 00, 00, 00, 00, 00, 00, (byte)cmd, 00, 00, 00, (byte)pgm, (byte)totalblocks, (byte)(totalblocks>>8), (byte)thisblock, (byte)(thisblock>>8), (byte)blocks };
            }
           
            List<byte> packet = new List<byte>(header);

            if (payload != null && payload.Count > 0)
            {
                foreach (data_serialisation p in payload)
                {
                    packet.AddRange(p.data);
                }

            }

            int count = escapepacket(packet, 20);
           
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
                return null;

            List<byte> packet = buildprogramcommand(command.UPLOAD, prg, null);

            List<byte> returneddata = packetshake(packet);

            return returneddata;
        }

        public List<List<byte>> chunk(List<byte> data, int size)
        {

            List<List<byte>> packets = new List<List<byte>>();

            int offset = 0;
            byte[] allbytes =  data.ToArray();

            int plucount = 0;
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

        public void setprogram(program prg,List <data_serialisation> payload)
        {
            if (checkstatus() == false)
                return;

            int totalblocks = payload.Count / 20;
            totalblocks++;

            List<data_serialisation> mini_payload = new List<data_serialisation>();;
            int count = 0;
            int thisblock=0;
            int totalcount = 0;
            foreach (data_serialisation p in payload)
            {

                mini_payload.Add(p);

                count++;
                totalcount++;

                //Payload should not be more than 20 blocks
                if (count == 20 || totalcount==payload.Count)
                {
                    List<byte> packet = buildprogramcommand(command.DOWNLOAD, prg, mini_payload,count,totalblocks,thisblock);

                    Console.WriteLine("\nNew packet to send :->\n");

                    foreach (byte b in packet)
                    {
                        Console.Write(String.Format("{0:X2}",b));

                    }

                    Console.WriteLine("\nEnd of packet\n");
                    List<byte> returneddata = packetshake(packet);

                    thisblock++;
                    totalblocks--;
                    count = 0;
                    mini_payload.Clear();
                }

            }

        }

        void sendACK()
        {
            byte[] data = { 0x10, 0x06 };
            port.Write(data, 0, data.Length);
        }

        void sendNACK()
        {
            byte[] data = { 0x10, 0x15 };
            port.Write(data, 0, data.Length);
        }

        //Get data command

        // This is a bit crap as its using fixed delays to wait for the RS232 buffer to fill up
        // we should really have a good idea of the side of the packets and how much data we are expecting at this point
        // or at least know how big any file should be then get the number of repeats from the first reply.
        // Will update this more once the protocol is better documentated as different read/writes may need handing differently

        List<byte> packetshake(List<byte> packet)
        {

            try
            {
                List<byte> retpayload = new List<byte>();

                byte[] packetb = packet.ToArray();

                OnProgress(new ProgressEventArgs(ProgressEventArgs.ProgressState.PROGRESS_SEND, 0, 0));

                port.Write(packetb, 0, packetb.Length);

                System.Threading.Thread.Sleep(1500);

                while (port.BytesToRead != 0)
                {

                    List<byte> allincomming = new List<byte>();

                    byte[] indata = null;

                    byte[] responseheader = null;

                    if (port.BytesToRead == 2)
                    {
                        responseheader = new byte[2];
                        port.Read(responseheader, 0, 2);
                        allincomming.AddRange(responseheader);

                        OnProgress(new ProgressEventArgs(ProgressEventArgs.ProgressState.PROGRESS_DONE, 0, 0));

                        return allincomming;
                    }

                    if (port.BytesToRead >= 20)
                    {
                        responseheader = new byte[20];
                        port.Read(responseheader, 0, 20);

                        //Parse the response header

                        //Response header = 10 02 01 00 00 00 00 00 00 00 03 00 00 00 00 03 00 00 00 14 
                        //Response header = 10 02 01 00 00 00 00 00 00 00 03 00 00 00 00 02 00 01 00 14 
                        //Response header = 10 02 01 00 00 00 00 00 00 00 03 00 00 00 00 01 00 02 00 0b

                        int blocksremaining = responseheader[15];
                        int thisblock = responseheader[17];
                        int filecount = responseheader[19];

                        OnProgress(new ProgressEventArgs(ProgressEventArgs.ProgressState.PROGRESS_DOWNLOAD, blocksremaining, thisblock));

                        Console.Write("Response header = ");

                        foreach (byte b in responseheader)
                        {
                            Console.Write(string.Format("{0:x2} ", b));
                        }

                        Console.Write("\n");
                    }

                    allincomming.AddRange(responseheader);

                    while (port.BytesToRead != 0)
                    {
                        int bytestoread = port.BytesToRead;
                        indata = new byte[bytestoread];
                        port.Read(indata, 0, bytestoread);

                        allincomming.AddRange(indata);
                        retpayload.AddRange(indata);

                        System.Threading.Thread.Sleep(250);

                    }

                    if (indata == null)
                        return null;

                    // remove last 4 bytes from data payload its the end of packet and checksum, data payload does not need this

                    retpayload.RemoveRange(retpayload.Count - 4, 4);

                    unescapepacket(allincomming);
                    testchecksum(allincomming);

                    sendACK();

                    System.Threading.Thread.Sleep(1000);

                }

                unescapepacket(retpayload);

                FileStream fs = new FileStream("packet.bin", FileMode.Create);
                BinaryWriter w = new BinaryWriter(fs);

                w.Write(retpayload.ToArray(), 0, retpayload.Count);

                fs.Close();

                OnProgress(new ProgressEventArgs(ProgressEventArgs.ProgressState.PROGRESS_DONE, 0, 0));

                return retpayload;
            }
            catch (Exception e)
            {
                OnProgress(new ProgressEventArgs(ProgressEventArgs.ProgressState.PROGRESS_ERROR, 0,0));
            }
            return null;
        }


        public bool checkstatus()
        {
            try
            {

                byte[] status = { 0x10, 02, 01, 00, 00, 00, 00, 00, 00, 00, 09, 09, 09, 09, 09, 01, 00, 00, 00, 00, 00, 0x10, 0x03, 0xAB };

                byte[] ret = new byte[2];

                port.Write(status, 0, status.Length);
                System.Threading.Thread.Sleep(100);
                port.Read(ret, 0, 2);

                if (ret[0] == 0x10 && ret[1] == 0x06)
                {
                    ecrok = true;
                    return true;
                }

                ecrok = false;
                return false;
            }
            catch (Exception e)
            {
                ecrok = false;
                OnProgress(new ProgressEventArgs(ProgressEventArgs.ProgressState.PROGRESS_ERROR, 0, 0));
                return false;
            }

        }

        // Invoke the Changed event; called whenever list changes
        protected void OnProgress(ProgressEventArgs e)
        {
            if (Progress != null)
                Progress(this, e);
        }

    }
}
