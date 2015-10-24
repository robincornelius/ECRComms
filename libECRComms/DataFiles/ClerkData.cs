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

        public int MaxClerkCount;
        public int NameLength;
        public int ClerkCodePos;
        public int TrainPos;
        public int DrawAssignPos;

        public ClerkData()
        {
            name = new string[MaxClerkCount];
            clerk_code = new int[MaxClerkCount];
            draw_assign = new int[MaxClerkCount];
            training = new bool[MaxClerkCount];
            ClerkCount = MaxClerkCount;
        }


        public override void decode()
        {

            for (int n = 0; n < MaxClerkCount; n++)
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
            for (int n = 0; n < MaxClerkCount; n++)
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
