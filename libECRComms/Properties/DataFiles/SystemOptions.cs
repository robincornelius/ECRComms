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

// ******************* PROGRAM 3 System Data ************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libECRComms
{
    public class SystemDataFactory
    {
        public static SystemData Get(MachineIDs id)
        {
            switch (id)
            {
                case MachineIDs.ER230:
                    return new SystemData_ER230();

                case MachineIDs.ER380M_UK:
                default:
                    return null;

            }

        }

    }

    public abstract class SystemData : data_serialisation
    {

        public SystemData()
        {

        }

        public override void decode()
        {
        }

        public override void encode()
        {
        }
    }


    public class SystemData1 : SystemData
    {
        public SystemData1()
        {

        }
    }

    public class SystemData_ER230 : SystemData
    {

        public ER230_SYSTEM_CONFIG config;

        public SystemData_ER230()
        {
            data = new byte[30];
        }

        public override void decode()
        {

            PrimitiveConversion.SetFromArray(config, data);

            for (uint x = 0; x < 30; x++)
            {
               PrimitiveConversion.Dump(config,x);
            }

        }

        public override void encode()
        {
            PrimitiveConversion.setarray(config, data);
        }
    }


    [global::System.AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    sealed class BitfieldLengthAttribute : Attribute
    {
        uint length;
        uint bytepos;
        string name;

        public BitfieldLengthAttribute(uint bytepos, uint length,string name="")
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

        public static void setarray<T>(T t,byte[] data) where T : struct
        {
            for (uint x = 0; x < data.Length; x++)
            {
                data[x] = GetByte(t, x);
            }

        }

        public static byte GetByte<T>(T t,uint bytepos) where T : struct
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
                        mask |= (byte) (1 << i);

                    r |= (byte)(((byte)f.GetValue(t) & mask) << (byte)offset);

                    offset += (int)fieldLength;

                   
                }
            }

            return r;
        }

        public static void Dump<T>(T t, uint bytepos) where T : struct
        {
            byte r = 0;
            int offset = 0;
            string name;

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

                    Console.WriteLine(String.Format("{0} = {1}",((BitfieldLengthAttribute)attrs[0]).Name,f.GetValue(t)));

                }
            }

           

        }
    }

    //This is a merge of at least the UK and AUS versions as i am sure the UK manual misses supported features

    public struct ER230_SYSTEM_CONFIG
    {
        [BitfieldLength(1, 1, "NOT_Beeper_is_active")]
        public byte NOT_Beeper_is_active;

        [BitfieldLength(2, 1,"clerk_sign_on_code_entry")]
        public byte clerk_sign_on_code_entry;

        [BitfieldLength(3, 1,"clerk_popup")]
        public byte clerk_popup;

        [BitfieldLength(4, 1, "Draw shut to operate ")]
        public byte Draw_shut_to_operate;
        [BitfieldLength(4, 1, "Activate open drawer alarm?")]
        public byte Activate_open_drawer_alarm;

        [BitfieldLength(5, 8, "Draw open alarm time (s)")]
        public byte open_drawer_warning_sounds_time;

        [BitfieldLength(6, 1,"allow_post_tender")]
        public byte allow_post_tender;
        [BitfieldLength(6, 1,"open drawer on post tender")]
        public byte drawer_open_post_tender;
        [BitfieldLength(6, 1,"Allow multiple recipts")]
        public byte Allow_multiple_recipts;

        [BitfieldLength(7, 1,"Cash_Declaration_Required_before_Z")]
        public byte Cash_Declaration_Required_before_Z;

        [BitfieldLength(8, 1, "Consecutive_number_is_reset_after_a_financial_report")]
        public byte Consecutive_number_is_reset_after_a_financial_report;

        [BitfieldLength(9, 1,"Reset_Grand_Total_after_Z_financial")]
        public byte Reset_Grand_Total_after_Z_financial;
        [BitfieldLength(9, 1,"Cash_drawer_will_open_with_reports")]
        public byte Cash_drawer_will_open_with_reports;
        [BitfieldLength(9, 1,"Open_drawer_during_training_mode")]
        public byte Open_drawer_during_training_mode;

        [BitfieldLength(10, 2,"Decimal_places")]
        public byte Decimal_places;

        [BitfieldLength(11, 2,"Date_format")]
        public byte Date_format;  //MMDDYY 0 , DDMMYY=1, YYMMDD=2

        [BitfieldLength(12, 2,"Percentage_and_Tax_calculations_will")]
        public byte Percentage_and_Tax_calculations_will; //0- roundup at 0.005 1-always round up, 2-always round down

        [BitfieldLength(13, 2,"Split_Price_calculations_will")]
        public byte Split_Price_calculations_will; //0- roundup at 0.005 1-always round up, 2-always round down

        [BitfieldLength(14, 1,"Hash_is")]
        public byte Hash_is;  //0 = normal, 1=non-add //WARNING DIFFERENT EPROMS CHANGE THIS

        [BitfieldLength(15, 1,"Reset_the_Financial_report_Z_counter_after_a_Z1_Financial_report")]
        public byte Reset_the_Financial_report_Z_counter_after_a_Z1_Financial_report;
        [BitfieldLength(15, 1,"Reset_the_Time_report_Z_counter_after_a_Z1_Time_report")]
        public byte Reset_the_Time_report_Z_counter_after_a_Z1_Time_report;
        [BitfieldLength(15, 1,"Reset_the_PLU_report_Z_counter_after_a_Z1_PLU_report")]
        public byte Reset_the_PLU_report_Z_counter_after_a_Z1_PLU_report;


        [BitfieldLength(16, 1,"Reset_the_Clerk_report_Z_counter_after_a_Z1_Clerk_report")]
        public byte Reset_the_Clerk_report_Z_counter_after_a_Z1_Clerk_report;
        [BitfieldLength(16, 1,"Reset_the_Group_report_Z_counter_after_a_Z1_Group_report")]
        public byte Reset_the_Group_report_Z_counter_after_a_Z1_Group_report;

        [BitfieldLength(17, 1,"Reset_the_Daily_sale_report_Z_counter_after_a_Z2_Daily_sale_report")]
        public byte Reset_the_Daily_sale_report_Z_counter_after_a_Z2_Daily_sale_report;
        [BitfieldLength(17, 1,"Paper_sensor_is_enabled")]
        public byte Paper_sensor_is_enabled;
        [BitfieldLength(17, 1,"Deactivate_Split_Pricing")]
        public byte Deactivate_Split_Pricing;

        [BitfieldLength(18, 1,"Allow_Direct_Multiply")]
        public byte Allow_Direct_Multiply;
        [BitfieldLength(18, 1,"Inventory_stock_counter_programming")]
        public byte Inventory_stock_counter_programming; // 0 =replace current level 2 = add to current level 

        [BitfieldLength(19, 1,"The_number_of_numeric_digits")]
        public byte The_number_of_numeric_digits;  //0 is no limit  0-14


        [BitfieldLength(20, 1,"Direct_multiply_more_than_one_digit")]
        public byte Direct_multiply_more_than_one_digit;
        [BitfieldLength(20, 1,"Tender_Validation_amount_is")]
        public byte Tender_Validation_amount_is; // 0=amt of sale 2==amt tendered

        [BitfieldLength(21, 1,"Display_add_price_1_of_linked_item")]
        public byte Display_add_price_1_of_linked_item;
        [BitfieldLength(21, 1,"Allow_sale_when_stock_0")]
        public byte Allow_sale_when_stock_0;

        [BitfieldLength(22, 1, "Allow rounding (AUS)")]
        public byte Allow_rounding_AUS;
        [BitfieldLength(22, 1, "Allow rounding cash (AUS)")]
        public byte Allow_rounding_cash_AUS;
        [BitfieldLength(22, 1,"Allow_Z_stock_report")]
        public byte Allow_Z_stock_report; 

        [BitfieldLength(23, 1,"Training_mode")]
        public byte Training_mode;

        [BitfieldLength(24, 1,"Use spool")]
        public byte Use_Spool ;

        [BitfieldLength(25, 1,"Keyboard_pgm_Method")]
        public byte Descriptor_Keyboard_pgm_Method ;
        [BitfieldLength(25, 1,"pct_is_not_affect_to_net_sale")]
        public byte pct_is_not_affect_to_net_sale;
        [BitfieldLength(25, 1,"Disable_Cash_Declaration")]
        public byte Disable_Cash_Declaration;

        [BitfieldLength(26, 1,"Disable_level_keys")]
        public byte Disable_level_keys; 

        [BitfieldLength(27, 1,"Price_level_is")]
        public byte Price_level_is;

        [BitfieldLength(28, 1,"Modifier_is")]
        public byte Modifier_is;

        [BitfieldLength(29, 1, "Price Embedded Barcode type")]
        public byte Price_Embedded_Barcode; 
       
        [BitfieldLength(30, 1,"Electronic_Journal_Enabled")]
        public byte Electronic_Journal_Enabled;
        [BitfieldLength(30, 1,"Prompt_Operator_When_EJ_buffer_is_full")]
        public byte Prompt_Operator_When_EJ_buffer_is_full;

        [BitfieldLength(31, 1,"Print_Density")]
        public byte Print_Density; //0-2

        //32.1 Allow New Zealand round on subtotal? 
        //32.2 Allow New Zealand round on cash? 
        //32.3 Active beep sound for saving battery? (EPROM v1.006) 

        //33 Use Mode password?
        // (WARNING! Set password before turn ON this feature   (EPROM v1.008)

 

    };

   

}
