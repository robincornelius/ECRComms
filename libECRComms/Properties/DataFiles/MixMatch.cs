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

// ******************* PROGRAM 12 Mix and Match Data ************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libECRComms.DataFiles
{
    class MaxMatchDataFactory
    {
        public static MaxMatchData Get(MachineIDs id)
        {
            switch (id)
            {
                case MachineIDs.ER230:
                case MachineIDs.ER380M_UK:
                    return new MaxMatchData1();

                default:
                    return null;

            }

        }

    }

    public abstract class MaxMatchData : data_serialisation
    {

        public MaxMatchData()
        {

        }

        public override void decode()
        {
        }

        public override void encode()
        {
        }
    }


    public class MaxMatchData1 : MaxMatchData
    {
        public MaxMatchData1()
        {

        }
    }
}
