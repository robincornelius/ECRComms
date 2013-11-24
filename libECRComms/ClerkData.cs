using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libECRComms
{
    public class ClerkData : data_serialisation
    {

        //ER380M
        //public const int Length = 25; extra length is longer name

        //ER230
        public const int Length = 19;

        public string name;
        int clerk_code;
        int draw_assign;

        public override void decode()
        {
            //ER230
            name = Encoding.UTF8.GetString(data, 1, 13);
     
            //ER380
            //name = Encoding.UTF8.GetString(data, 1, 19);

            Console.WriteLine("Found Clerk {0}\n", name);
        }

        public override void encode()
        {

        }
    }
}
