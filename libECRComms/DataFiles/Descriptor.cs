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

// ******************* PROGRAM 7 DESCRIPTOR ************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libECRComms
{

    public class Descriptor : data_serialisation
    {
        //Descriptors are 3 sections, preamble, postable and endorsements
        //each line can be 32 characters long
        //preamble and postamble are 6 lines
        //endorsements are 10 lines

        public string[] preamble = new string[6];
        public string[] postamble = new string[6];
        public string[] endorsements = new string[10];

        public Descriptor()
        {
        }

        public Descriptor(byte [] data)
        {
            this.data = data;
            decode();
        }


        public override void decode()
        {
          
            int p;

            for (p = 0; p < 6; p++)
            {
                preamble[p] = System.Text.Encoding.ASCII.GetString(data.SubArray(32*p, 32));
                preamble[p] = preamble[p].Replace("\0", string.Empty);
            }

            for (p = 0; p < 6; p++)
            {
                postamble[p] = System.Text.Encoding.ASCII.GetString(data.SubArray(32*6+32*p, 32));
                postamble[p] = postamble[p].Replace("\0", string.Empty);
            }

            for (p = 0; p < 10; p++)
            {
                endorsements[p] = System.Text.Encoding.ASCII.GetString(data.SubArray(32 * 12 + 32 * p, 32));
                endorsements[p] = endorsements[p].Replace("\0", string.Empty);
            
            }
        }

        public override void encode()
        {
            int p;

            for (p = 0; p < 6; p++)
            {
                Array.Copy(Encoding.ASCII.GetBytes(preamble[p]),0, data, p*32,preamble[p].Length);
            }

            for (p = 0; p < 6; p++)
            {
                Array.Copy(Encoding.ASCII.GetBytes(postamble[p]), 0, data, 6*32+ p * 32, postamble[p].Length);
            }

            for (p = 0; p < 6; p++)
            {
                Array.Copy(Encoding.ASCII.GetBytes(endorsements[p]), 0, data, 12 * 32 + p * 32, endorsements[p].Length);
            }
        }


        public void dump()
        {
            int p;

            Console.Write("** PREAMBLE **\n");          
            for (p = 0; p < 6; p++)
            {
                Console.Write(preamble[p]);
                Console.Write("\n");
            }

            Console.Write("** POSTAMBLE **\n");          
            for (p = 0; p < 6; p++)
            {
                Console.Write(postamble[p]);
                Console.Write("\n");
            }

            Console.WriteLine("** ENDORSEMENT **\n");
            for (p = 0; p < 6; p++)
            {
                Console.Write(endorsements[p]);
                Console.Write("\n");
            }
           

        }

    }
}
