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

// ******************* PROGRAM 4 Function Key Data ************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libECRComms.DataFiles
{
    class FunctionKeyDataFactory
    {
        public static FunctionKeyData Get(MachineIDs id)
        {
            switch (id)
            {
                case MachineIDs.ER230:
                    return new FunctionKeyData_ER230();

                case MachineIDs.ER380M_UK:
                default:
                    return null;

            }

        }

    }

    public abstract class FunctionKeyData : data_serialisation
    {

        public int length;
        public int name_length;
        public int status_pos;

        public FunctionKeyData()
        {

        }

        public override void decode()
        {
        }

        public override void encode()
        {
        }
    }



  

    //21 Long
    //Name starts from byte 0 and is 12 long
    //Byte 16 in each block is the start of the settings
    public class FunctionKeyData_ER230 : FunctionKeyData
    {
        ER230_FN_Non_Add_No_Sale a;
        ER230_FN_PCT[] pct = new ER230_FN_PCT[5];
        //XTIME **
        //ADD CHK
        //CANCEL
        //CASH
        //CHARGE1...CHARGE8
        //CHKCASH
        //CHKENDOR
        //CHEQUE
        //NOT USED
        //CLEAR **
        //CLERK **
        //CONV1...CONV4
        //NOT USED
        //NOT USED
        //ERRCORR
        //FSHIFT **
        //F/S SUB
        //F/S TEND
        //NOT USED
        //NOT USED
        //NOT USED
        //PLU
        //LEVEL 1
        //LEVEL 2
        //NOT USED
        //NOT USED
        //NOT USED
        //MACRO1-10
        //RETURN
        //MOD 1-5
        //NOT USED
        //NOT USED
        //NOT USED
        //PO1-3
        //PFEED
        //NOT USED
        //NOT USED
        //PROMO
        //RA1-RA3
        //SUBTOTAL
        //SCALE
        //NOT USED
        //NOT USED
        //TARE
        //TAXEMIT
        //TAX1-4
        //NOT_USED
        //NOT USED
        //VOID
        //WASTE
        //NOT USED
        //VALIDATION
        //PRICE_INQ **

        //NOT USED x 42
        //RCPT ON/OFF **
        //MODE KEY **

        public struct functions
        {
            public functions(string name,placeholder data)
            {
                this.name = name;
                this.data = data;

            }

            string name;
            placeholder data;
        }

        //MyStruct[] x = new MyStruct[] { new MyStruct(1), new MyStruct(2) };

        functions[] fn = new functions[]
        {
            //new functions("XTIME",null), 
        };


        public FunctionKeyData_ER230()
        {
            length=21;
            name_length=13;
            status_pos=16;
        }

        public override void decode()
        {
             //ECRComms.chunk(data.ToList(),length);

            
        }

        public override void encode()
        {
        }

        public struct placeholder
        {

        }

        public struct ER230_FN_ADDCHECK
        {

            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Compulsory before tendering? ")]
            public byte Compulsory_before_tendering;
            [BitfieldLength(1, 1, "Advance the consecutive # when this function is used?")]
            public byte Advance_the_consecutive_no_when_this_function_is_used;

            [BitfieldLength(1, 1, "Delete the pre/postamble when this function is used?")]
            public byte Delete_the_pre_postamble_when_this_function_is_used;
            [BitfieldLength(1, 1, "Exempt tax 1?")]
            public byte Exempt_tax_1;
            [BitfieldLength(1, 1, "Exempt tax 2? ")]
            public byte Exempt_tax_2 ;

            [BitfieldLength(1, 1, "Exempt tax 3? ")]
            public byte Exempt_tax_3;
            [BitfieldLength(1, 1, "Exempt tax 4? ")]
            public byte Exempt_tax_4;
            [BitfieldLength(2, 1, "Validation is compulsory? ?")]
            public byte Validation_is_compulsory;
        }

        public struct ER230_FN_CANCEL
        {
            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "Validation is compulsory?")]
            public byte Validation_is_compulsory;
        
        }

        public struct ER230_FN_CASH
        {

            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "Disable under tendering?")]
            public byte Disable_under_tendering;

            [BitfieldLength(1, 1, "Open cash drawer?")]
            public byte Open_cash_drawer;
            [BitfieldLength(1, 1, "Exempt tax 1?")]
            public byte Exempt_tax_1;
            [BitfieldLength(1, 1, "Exempt tax 2? ")]
            public byte Exempt_tax_2;

            [BitfieldLength(1, 1, "Exempt tax 3? ")]
            public byte Exempt_tax_3;
            [BitfieldLength(1, 1, "Exempt tax 4? ")]
            public byte Exempt_tax_4;
            [BitfieldLength(2, 1, "Validation is compulsory?")]
            public byte Validation_is_compulsory;
        }

        public struct ER230_FN_CHARGE
        {
            [BitfieldLength(1, 1, "Amount tendered is compulsory?")]
            public byte Amount_tendered_is_compulsory;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "Disable under tendering?")]
            public byte Disable_under_tendering;

            [BitfieldLength(1, 1, "Open cash drawer?")]
            public byte Open_cash_drawer;
            [BitfieldLength(1, 1, "Allow over tendering?")]
            public byte allow_over_tendering;
            [BitfieldLength(1, 1, "Non-add # entry compulsory? ")]
            public byte Non_add_no_entry_compulsory;

            [BitfieldLength(1, 1, "Exempt tax 1?")]
            public byte Exempt_tax_1;
            [BitfieldLength(1, 1, "Exempt tax 2? ")]
            public byte Exempt_tax_2;
            [BitfieldLength(2, 1, "Exempt tax 3? ")]
            public byte Exempt_tax_3;

            [BitfieldLength(2, 1, "Exempt tax 4? ")]
            public byte Exempt_tax_4;
            [BitfieldLength(2, 1, "Validation is compulsory?")]
            public byte Validation_is_compulsory;
            [BitfieldLength(2, 1, "Send to EFT?")]
            public byte Send_to_EFT;

            [BitfieldLength(3, 8, "EFT port")]
            public byte eft_no; //0-2
        }

         public struct ER230_FN_CHEQUE
        {
            [BitfieldLength(1, 1, "Amount tendered is compulsory?")]
            public byte Amount_tendered_is_compulsory;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "Disable under tendering?")]
            public byte Disable_under_tendering;

            [BitfieldLength(1, 1, "Open cash drawer?")]
            public byte Open_cash_drawer;
            [BitfieldLength(1, 1, "Exempt tax 1?")]
            public byte Exempt_tax_1;
            [BitfieldLength(1, 1, "Exempt tax 2? ")]
            public byte Exempt_tax_2;

            [BitfieldLength(1, 1, "Exempt tax 3? ")]
            public byte Exempt_tax_3;
            [BitfieldLength(1, 1, "Exempt tax 4? ")]
            public byte Exempt_tax_4;

            [BitfieldLength(2, 1, "Check endorsement compulsory?")]
            public byte Check_endorsement_compulsory;
            [BitfieldLength(2, 1, "Validation is compulsory?")]
            public byte Validation_is_compulsory;
        }

        public struct ER230_FN_CHEQUE_CASHING
        {

            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "Validation is compulsory?")]
            public byte Validation_is_compulsory;
        }

        public struct ER230_FN_CHEQUE_ENDORSEMENT 
        {
            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Print the amount of the check and endorsement message?")]
            public byte Print_the_amount_of_the_check_and_endorsement_message;
            [BitfieldLength(1, 1, "Print date?")]
            public byte Print_date;

            [BitfieldLength(1, 1, "Print time?")]
            public byte Print_time;
            [BitfieldLength(1, 1, "Print clerk?")]
            public byte Print_clerk;
            [BitfieldLength(1, 1, "Print consecutive number? ")]
            public byte Print_consecutive_number;

        }

        public struct ER230_FN_ERROR_CORRECT 
        {
            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "Validation is compulsory?")]
            public byte Validation_is_compulsory;
        }

        public struct ER230_FN_FS_SUB
        {
             [BitfieldLength(1, 1, "Key is inactive?")]
             public byte Key_is_inactive;
        }

        public struct ER230_FN_FS_TEND 
        {
            [BitfieldLength(1, 1, "Exempt tax 1?")]
            public byte Exempt_tax_1;
            [BitfieldLength(1, 1, "Exempt tax 2? ")]
            public byte Exempt_tax_2;
            [BitfieldLength(1, 1, "Exempt tax 3? ")]
            public byte Exempt_tax_3;

            [BitfieldLength(1, 1, "Exempt tax 4? ")]
            public byte Exempt_tax_4;
            [BitfieldLength(1, 1, "The tender is allowed in any amount?")]
            public byte The_tender_is_allowed_in_any_amount;
            [BitfieldLength(1, 1, "Food stamp change Is issued in ")]
            public byte Food_stamp_change_Is_issued_in;

            [BitfieldLength(1, 1, "Open cash drawer?")]
            public byte Open_cash_drawer;
            [BitfieldLength(1, 1, "Validation is compulsory?")]
            public byte Validation_is_compulsory;

        }

        public struct ER230_FN_LEVEL
        {
            [BitfieldLength(1, 1, "Print level description at the KP? ")]
            public byte Print_level_description_at_the_KP;
        }

        public struct ER230_FN_MACRO
        {
            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "Validation is compulsory?")]
            public byte Validation_is_compulsory;
        }

        public struct ER230_FN_MODIFIER
        {
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "Modify PLU#?")]
            public byte Modify_PLU;

            [BitfieldLength(1, 1, "Print modifier descriptor on the guest check? ")]
            public byte Print_modifier_descriptor_on_the_guest_check;
            [BitfieldLength(1, 1, "Print modifier descriptor on the receipt? ")]
            public byte Print_modifier_descriptor_on_the_receipt;

            [BitfieldLength(2, 8, "Value of affected digit (0-9) ")]
            public byte Value_of_affected_digit;

        }

       public struct ER230_FN_Non_Add_No_Sale
       {

            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "No Sale inactive after non-add # entry?")]
            public byte No_Sale_inactive_after_non_add_no_entry;

            [BitfieldLength(1, 1, "Enforce non-add # entry at start of sale? ")]
            public byte Enforce_non_add_no_entry_at_start_of_sale;
            [BitfieldLength(1, 1, "Print when a NO SALE is performed? ")]
            public byte Print_when_a_NO_SALE_is_performed;
            [BitfieldLength(1, 1, "Non-add # entries are prohibited? ")]
            public byte Non_add_no_entries_are_prohibited;

            [BitfieldLength(1, 1, "Compulsory non-add entry must match number of digits set in the MAX DIGIT flag below? ")]
            public byte Compulsory_non_add_entry_must_match_number_of_digits_set_in_the_MAX_DIGIT_flag_below;
            [BitfieldLength(1, 1, "Print non-add on guest check? ")]
            public byte Print_non_add_on_guest_check;

            [BitfieldLength(2, 8, "Enter maximum number of digits for non-add number entry. (0-8; Zero (0) means no limit)")]
            public byte Enter_maximum_number_of_digits_for_non_add_number_entry;
       }


        public struct ER230_FN_PAID_OUT
        {
            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "Validation is compulsory?")]
            public byte Validation_is_compulsory;
        }

        public struct ER230_FN_PROMO
        {
            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "tax 1?")]
            public byte tax_1;

            [BitfieldLength(1, 1, "tax 2? ")]
            public byte tax_2;
            [BitfieldLength(1, 1, "tax 3? ")]
            public byte tax_3;
            [BitfieldLength(1, 1, "tax 4? ")]
            public byte tax_4;

        }

        public struct ER230_FN_RECD_ON_ACCT
        {
            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "Validation is compulsory?")]
            public byte Validation_is_compulsory;
        }

        public struct ER230_FN_SCALE
        {
            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "Allow manual entry of weight?")]
            public byte Allow_manual_entry_of_weight;

            [BitfieldLength(1, 1, "Subtract tare weight on the scale entry? ")]
            public byte Subtract_tare_weight_on_the_scale_entry;
            [BitfieldLength(1, 1, "Weight symbol for manual entry is")]
            public byte Weight_symbol_for_manual_entry_is; // 0 =lb 1=kg
            [BitfieldLength(1, 1, "Allow register scaleable items by weight extension or by price entry? ")]
            public byte Allow_register_scaleable_items_by_weight_extension_or_by_price_entry;
        }

        public struct ER230_FN_SUBTOTAL 
        {
            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
        }

        public struct ER230_FN_TABLE
        {
            [BitfieldLength(1, 1, "Table number entry compulsory before opening a new check? ")]
            public byte Table_number_entry_compulsory_before_opening_a_new_check;
            [BitfieldLength(1, 1, "Table number entry compulsory for all sales?")]
            public byte Table_number_entry_compulsory_for_all_sales;
            [BitfieldLength(1, 1, "Print table# at the remote printer? ")]
            public byte Print_table_no_at_the_remote_printer;

        }

        public struct ER230_FN_TARE 
        {
            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "Using tare number five to manually enter a tare weight? ")]
            public byte Using_tare_number_five_to_manually_enter_a_tare_weight;
        }


        public struct ER230_FN_TAX_EXEMPT 
        {
            [BitfieldLength(1, 1, "Exempt tax 1?")]
            public byte Exempt_tax_1;
            [BitfieldLength(1, 1, "Exempt tax 2? ")]
            public byte Exempt_tax_2;
            [BitfieldLength(1, 1, "Exempt tax 3? ")]
            public byte Exempt_tax_3;

            [BitfieldLength(1, 1, "Exempt tax 4? ")]
            public byte Exempt_tax_4;
            [BitfieldLength(1, 1, "Compulsory non-add number before this key is used? ")]
            public byte Compulsory_non_add_number_before_this_key_is_used;
            [BitfieldLength(1, 1, "Validation is compulsory?")]
            public byte Validation_is_compulsory;
        }

         public struct ER230_FN_VALIDATE 
         {
            [BitfieldLength(1, 8, "Enter Port Number. Enter 0 (zero) if validation is not used. (0-2) ")]
            public byte Enter_Port_Number;

            [BitfieldLength(2, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(2, 1, "Allow multiple validations? ")]
            public byte Allow_multiple_validations;
         }

         public struct ER230_FN_VOID 
         {
            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "Validation is compulsory?")]
            public byte Validation_is_compulsory;
         }

        public struct ER230_FN_WASTE 
        {
            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;
            [BitfieldLength(1, 1, "Validation is compulsory?")]
            public byte Validation_is_compulsory;
        }

        public struct ER230_FN_PCT
        {
            //N1
            [BitfieldLength(1, 1, "Apply an: ")]
            public byte Apply_an; //1 amount //0 pct
            [BitfieldLength(1, 1, "Key is inactive?")]
            public byte Key_is_inactive;
            [BitfieldLength(1, 1, "Reserved")]
            public byte Reserved;

            //N2 but still first byte
            [BitfieldLength(1, 1, "% key is")]
            public byte pct_key_is; //1 open //0 preset
            [BitfieldLength(1, 1, "% pct_key_acts_on")]
            public byte pct_key_acts_on; //1 sale //0 item
            [BitfieldLength(1, 1, "Allow % key override preset?")]
            public byte Allow_pct_key_override_preset; //1 sale //0 item

            //N3
            [BitfieldLength(1, 1, "% key polarity")]
            public byte pct_key_polarity; //1 positive //0 negative
            [BitfieldLength(1, 1,"% Amount taxable tax 1?")]
            public byte pct_Amount_taxable_tax_1; //1 sale //0 item

            //N4
            [BitfieldLength(2, 1,"% Amount taxable tax 2?")]
            public byte pct_Amount_taxable_tax_2; //1 sale //0 item
            [BitfieldLength(2, 1,"% Amount taxable tax 3?")]
            public byte pct_Amount_taxable_tax_3; //1 sale //0 item
            [BitfieldLength(2, 1,"% Amount taxable tax 4?")]
            public byte pct_Amount_taxable_tax_4; //1 sale //0 item

            //N5
            [BitfieldLength(2, 1, "Reduce (or increase) the food stamp subtotal by % entry? ")]
            public byte Reduce_or_increase_the_food_stamp_subtotal_by_pct_entry;
            [BitfieldLength(2, 1, "Allow only one time subtotal entry? ")]
            public byte Allow_only_one_time_subtotal_entry;
            [BitfieldLength(2, 1, "Allow multiple amount discounts (coupons) without pressing subtotal? ")]
            public byte Allow_multiple_amount_discounts_coupons_without_pressing_subtotal;

            //N6
            [BitfieldLength(2, 1, "Allow % key preset override active in X control lock position only?")]
            public byte Allow_pct_key_preset_override_active_in_X_control_lock_position_only;
            [BitfieldLength(2, 1, "Validation is compulsory?")]
            public byte Validation_is_compulsory;       
        }

      
    }
}
