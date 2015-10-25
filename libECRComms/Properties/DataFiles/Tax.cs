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

// ******************* PROGRAM 2 Tax Data ************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libECRComms.DataFiles
{
    class TaxDataFactory
    {
        public static TaxData Get(MachineIDs id)
        {
            switch (id)
            {
                case MachineIDs.ER230:
                case MachineIDs.ER380M_UK:
                    return new TaxData1();

                default:
                    return null;

            }

        }

    }

    public abstract class TaxData : data_serialisation
    {
        protected int Length;
        public int MaxCount;

        protected int NameLength; //can we even change this????

        protected int flags_pos;
        protected int value_pos;

        public string[] name;

        public  Eflags[] flags;
        public decimal[] value;


        public enum Eflags
        {
            AddOn = 0x00,
            Vat = 0x20,

        }

        public TaxData()
        {

        }

        public override void decode()
        {
            for (int n = 0; n < MaxCount; n++)
            {
                name[n] = ECRComms.gettext(data, n * Length, NameLength);
                flags[n] = (Eflags)ECRComms.extractint1(data,n* Length + flags_pos);
                value[n] = (decimal)ECRComms.extractfloat4(data, n * Length + value_pos);
            }
        }

        public override void encode()
        {
            for (int n = 0; n < MaxCount; n++)
            {
                ECRComms.puttext(data, n * Length, NameLength, name[n]);
                ECRComms.putint1(data, n * Length + flags_pos, (int)flags[n]);
                ECRComms.putdecimal4(data, n * Length + value_pos, value[n]);
            }
        }
    }


    public class TaxData1 : TaxData
    {
        public TaxData1()
        {
            Length = 17;
            MaxCount = 4;
            NameLength = 5;

            flags_pos = 16;
            value_pos = 12;

            flags = new Eflags[MaxCount];
            name = new String[MaxCount];
            value = new decimal[MaxCount];

        }
    }
}
