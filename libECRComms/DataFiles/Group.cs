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

// ******************* PROGRAM 1 Group Data ************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libECRComms.DataFiles
{
    class GroupDataFactory
    {
        public static GroupData Get(MachineIDs id)
        {
            switch (id)
            {
                case MachineIDs.ER230:
                case MachineIDs.ER380M_UK:
                    return new GroupData1();

                default:
                    return null;

            }

        }

    }

    public abstract class GroupData : data_serialisation
    {

        protected int Length;

        protected int NameLength;

        protected int flags_pos;

        public int MaxCount;

        public string[] name;

        public int[] status;

        public GroupData()
        {

        }

        public override void decode()
        {
            for (int n = 0; n < MaxCount; n++)
            {
                name[n] = ECRComms.gettext(data, n * Length, NameLength);
                status[n] = ECRComms.extractint1(data, n * Length + 0x01);
            }
        }

        public override void encode()
        {
            for (int n = 0; n < MaxCount; n++)
            {
                ECRComms.puttext(data, n * Length, NameLength, name[n]);
                ECRComms.putint1(data, n * Length + 0x01, status[n]);
            }
        }
    }


     public class GroupData1 : GroupData
     {
         enum flags //valid on ER230 so far confirmed
         {
             add_group_total_pos = 0x01,
             send_to_kp = 0x02,
             port1 = 0x04,
             port2 = 0x08,
             recipt = 0x10,
             print_red_kp = 0x20,
         }

        public GroupData1()
        {
            Length = 17; //ER230M confirmed
            NameLength = 12;
            flags_pos = 13;

            MaxCount = 30;

            name = new string[MaxCount];
            status = new int[MaxCount];
        }
     }
}
