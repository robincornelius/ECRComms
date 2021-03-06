﻿/*

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

// ******************* PROGRAM 6 CLERK DATA ************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libECRComms
{
    public static class ClerkDataFactory
    {
        public static ClerkData Get(MachineIDs id)
        {
            switch(id)
            {
                case MachineIDs.ER230:
                    return new ClerkDataER230();

                case MachineIDs.ER380M_UK:
                    return new ClerkDataER380MUK();

                default:
                    return null;
             
            }
            
        }

    }

    public abstract class ClerkData : data_serialisation
    {
        public string[] name;
        public int[] clerk_code;
        public int[] draw_assign;
        public bool[] training;

        public int Length;
        public int ClerkCount;

        public int MaxCount = 10;
        public int NameLength;
        public int ClerkCodePos;
        public int TrainPos;
        public int DrawAssignPos;

        public ClerkData()
        {
            name = new string[MaxCount];
            clerk_code = new int[MaxCount];
            draw_assign = new int[MaxCount];
            training = new bool[MaxCount];
            ClerkCount = MaxCount;
        }


        public override void decode()
        {

            for (int n = 0; n < MaxCount; n++)
            {
                name[n] = ECRComms.gettext(data, n * Length, NameLength);
                clerk_code[n] = ECRComms.extractint3(data, n * Length + ClerkCodePos); //Is this really an INT4???? there is a spare 0 in the data field
                training[n] = ECRComms.extractint1(data, n * Length + TrainPos) == 1;
                draw_assign[n] = ECRComms.extractint1(data, n * Length + DrawAssignPos);
            }

            Console.WriteLine("Found Clerk {0}\n", name);
        }

        public override void encode()
        {
            for (int n = 0; n < MaxCount; n++)
            {
                ECRComms.puttext(data, n * Length, NameLength, name[n]);
                ECRComms.putint3(data, n * Length + ClerkCodePos, clerk_code[n]);
                ECRComms.putint1(data, n * Length + TrainPos, training[n] == true ? 1 : 0);
                ECRComms.putint1(data, n * Length + DrawAssignPos, draw_assign[n]);
            }

        }

    }

    public class ClerkDataER380MUK : ClerkData
    {
        public ClerkDataER380MUK()
            : base()
        {
            Length = 25;
            NameLength = 19;

            ClerkCodePos = 19;
            TrainPos = 23;
            DrawAssignPos = 24;
        }
    }

    public class ClerkDataER230 : ClerkData
    {

        public ClerkDataER230() : base()
        {
            Length = 19;
            NameLength = 13;

            ClerkCodePos = 13;
            TrainPos = 17;
            DrawAssignPos = 18;
        }


    }
}
