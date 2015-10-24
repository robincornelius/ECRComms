using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace libECRComms
{
    public static class DataFile
    {

        public static byte [] loadbinaryfile(string filename)
        {

            FileStream r = File.Open(filename, FileMode.Open);
            System.IO.BinaryReader reader = new BinaryReader(r);
            byte[] data = reader.ReadBytes((int)r.Length);
            reader.Close();
            return data;
        }


        public static byte[] loadasciifile(string filename)
        {
            System.IO.StreamReader myFile = new System.IO.StreamReader(filename);
            string myString = myFile.ReadToEnd();
            myFile.Close();
            byte[] data = ECRComms.StringToByteArrayFastest(myString);
            return data;
        }


        public static void savebinaryfile(string filename, byte[] payload)
        {

            FileStream r = File.Open(filename, FileMode.OpenOrCreate);
            System.IO.BinaryWriter writer = new BinaryWriter(r);
            writer.Write(payload,0,payload.Length);
            writer.Close();
        }

        public static void saveasciifile(string filename, byte[] payload)
        {

            System.IO.StreamWriter myFile = new System.IO.StreamWriter(filename);
            foreach (byte b in payload)
            {
                myFile.Write(String.Format("{0:x2}", b));
            }

            myFile.Close();
        }

    }
}
