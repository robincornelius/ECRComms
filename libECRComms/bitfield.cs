using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libECRComms
{
    [global::System.AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    sealed class BitfieldLengthAttribute : Attribute
    {
        uint length;
        uint bytepos;
        string name;

        public BitfieldLengthAttribute(uint bytepos, uint length, string name = "")
        {
            this.length = length;
            this.bytepos = bytepos;
            this.name = name;
        }

        public uint Length { get { return length; } }
        public uint BytePos { get { return bytepos; } }
        public string Name { get { return name; } }
    }

    public static class PrimitiveConversion
    {
        public static void SetFromArray<T>(T t, byte[] data) where T : struct
        {
            int offset = 0;
            int lastabytepos = -1;

            foreach (System.Reflection.FieldInfo f in t.GetType().GetFields())
            {
                object[] attrs = f.GetCustomAttributes(typeof(BitfieldLengthAttribute), false);
                if (attrs.Length == 1)
                {
                    uint abytepos = ((BitfieldLengthAttribute)attrs[0]).BytePos;
                    uint fieldLength = ((BitfieldLengthAttribute)attrs[0]).Length;

                    if (lastabytepos != abytepos)
                        offset = 0;

                    if (abytepos > data.Length)
                        continue;

                    lastabytepos = (int)abytepos;

                    byte mask = 0;
                    for (int i = 0; i < fieldLength; i++)
                        mask |= (byte)(1 << i);

                    byte datax = (byte)data[abytepos - 1];
                    datax = (byte)(datax >> offset);
                    datax = (byte)(datax & mask);

                    object b = t;
                    f.SetValue(b, datax);
                    t = (T)b;

                    offset += (int)fieldLength;
                }

            }
        }

        public static void setarray<T>(T t, byte[] data) where T : struct
        {
            for (uint x = 0; x < data.Length; x++)
            {
                data[x] = GetByte(t, x);
            }

        }

        public static byte GetByte<T>(T t, uint bytepos) where T : struct
        {
            byte r = 0;
            int offset = 0;

            // For every field suitably attributed with a BitfieldLength
            foreach (System.Reflection.FieldInfo f in t.GetType().GetFields())
            {
                object[] attrs = f.GetCustomAttributes(typeof(BitfieldLengthAttribute), false);
                if (attrs.Length == 1)
                {
                    uint fieldLength = ((BitfieldLengthAttribute)attrs[0]).Length;
                    uint abytepos = ((BitfieldLengthAttribute)attrs[0]).BytePos;

                    if (abytepos != bytepos)
                        continue;

                    // Calculate a bitmask of the desired length
                    byte mask = 0;
                    for (int i = 0; i < fieldLength; i++)
                        mask |= (byte)(1 << i);

                    r |= (byte)(((byte)f.GetValue(t) & mask) << (byte)offset);

                    offset += (int)fieldLength;

                }
            }

            return r;
        }

        public static void Dump<T>(T t, uint bytepos) where T : struct
        {
            // For every field suitably attributed with a BitfieldLength
            foreach (System.Reflection.FieldInfo f in t.GetType().GetFields())
            {
                object[] attrs = f.GetCustomAttributes(typeof(BitfieldLengthAttribute), false);
                if (attrs.Length == 1)
                {
                    uint fieldLength = ((BitfieldLengthAttribute)attrs[0]).Length;
                    uint abytepos = ((BitfieldLengthAttribute)attrs[0]).BytePos;

                    if (abytepos != bytepos)
                        continue;

                    Console.WriteLine(String.Format("{0} = {1}", ((BitfieldLengthAttribute)attrs[0]).Name, f.GetValue(t)));

                }
            }

        }
    }

}
