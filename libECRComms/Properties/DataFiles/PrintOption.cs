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

// ******************* PROGRAM 4 Print Option Data ************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libECRComms.DataFiles
{
    class PrintOptionDataFactory
    {
        public static PrintOptionData Get(MachineIDs id)
        {
            switch (id)
            {
                case MachineIDs.ER230:
                     return new PrintOption_ER230();

                case MachineIDs.ER380M_UK:
                default:
                    return null;

            }

        }

    }

    public abstract class PrintOptionData : data_serialisation
    {

        public int print_option_length;

        public PrintOptionData()
        {

        }

        public override void decode()
        {
        }

        public override void encode()
        {
        }
    }


    public class PrintOptionData1 : PrintOptionData
    {
        public PrintOptionData1()
        {

        }
    }

    public class PrintOption_ER230 : PrintOptionData
    {
        ER230_PRINT_CONFIG config;
    
        public PrintOption_ER230()
        {
            print_option_length = 40;
            data = new byte[print_option_length];
        }

         public override void decode()
         {

            PrimitiveConversion.SetFromArray(config, data);

            for (uint x = 0; x < print_option_length; x++)
            {
               PrimitiveConversion.Dump(config,x);
            }

        }

        public override void encode()
        {
            PrimitiveConversion.setarray(config, data);
        }
    }


    public struct ER230_PRINT_CONFIG
    {
        [BitfieldLength(1, 1, "Print tax symbol?")]
        public byte Print_tax_symbol;

        [BitfieldLength(2, 1, "Void/Return totals will print on the Financial report?")]
        public byte Void_Return_totals_will_print_on_the_Financial_report;
        [BitfieldLength(2, 1, "Audaction total will print on the Financial report? ")]
        public byte Audaction_total_will_print_on_the_Financial_report;

        [BitfieldLength(3, 1, "Skip media totals with zero activity on the Financial report?")]
        public byte Skip_media_totals_with_zero_activity_on_the_Financial_report;
        [BitfieldLength(3, 1, "Skip media totals with zero activity on the Clerk report?")]
        public byte Skip_media_totals_with_zero_activity_on_the_Clerk_report;
        [BitfieldLength(3, 1, "Print Clerk report at the end of the Financial report? ")]
        public byte Print_Clerk_report_at_the_end_of_the_Financial_report;

        [BitfieldLength(4, 1, "Print sale item number (PLU number)?")]
        public byte Print_sale_item_number_PLU_number;
        [BitfieldLength(4, 1, "Print PLU with zero totals on report?")]
        public byte Print_PLU_with_zero_totals_on_report;
        [BitfieldLength(4, 1, "Subtotal is printed when the SBTL key is pressed?")]
        public byte Subtotal_is_printed_when_the_SBTL_key_is_pressed;

        [BitfieldLength(5, 1, "Print percentage of sales on the PLU report? ")]
        public byte Print_percentage_of_sales_on_the_PLU_report;
        [BitfieldLength(5, 1, "Print consecutive number counter on receipt? ")]
        public byte Print_consecutive_number_counter_on_receipt;
        [BitfieldLength(5, 1, "Print machine number on receipt?")]
        public byte Print_machine_number_on_receipt;

        [BitfieldLength(6, 1, "Print date on receipt?")]
        public byte Print_date_on_receipt;
        [BitfieldLength(6, 1, "Print time on receipt? ")]
        public byte Print_time_on_receipt;

        [BitfieldLength(7, 1, "Print clerk name on receipt? ")]
        public byte Print_clerk_name_on_receipt;
        [BitfieldLength(7, 1, "Print Z counter on reports? ")]
        public byte Print_Z_counter_on_reports;

        [BitfieldLength(8, 1, "Home Currency symbol")]
        public byte Home_Currency_symbol;

        [BitfieldLength(9, 1, "Print receipt when sign on/off")]
        public byte Print_receipt_when_sign_on_off;
        [BitfieldLength(9, 1, "Print Grand total on the X Financial report?")]
        public byte Print_Grand_total_on_the_X_Financial_report;
        [BitfieldLength(9, 1, "Print Grand total on the Z Financial report?")]
        public byte Print_Grand_total_on_the_Z_Financial_report;

        [BitfieldLength(10, 1, "Print Gross total on the X Financial report")]
        public byte Print_Gross_total_on_the_X_Financial_report;
        [BitfieldLength(10, 1, "Print Gross total on the Z Financial report")]
        public byte Print_Gross_total_on_the_Z_Financial_report;

        [BitfieldLength(11, 1, "Print the subtotal without tax on the receipt? ")]
        public byte Print_the_subtotal_without_tax_on_the_receipt;
        [BitfieldLength(11, 1, "Tax amount to print on receipt is:")]
        public byte Tax_amount_to_print_on_receipt_is;

        [BitfieldLength(12, 1, "Print the tax amount on receipt?")]
        public byte Print_the_tax_amount_on_receipt;
        [BitfieldLength(12, 1, "Print taxable totals?")]
        public byte Print_taxable_totals;
        [BitfieldLength(12, 1, "Print the tax rate?")]
        public byte Print_the_tax_rate;

        [BitfieldLength(13, 1, "Print a breakdown of the VAT eligible sale?")]
        public byte Print_a_breakdown_of_the_VAT_eligible_sale;
        [BitfieldLength(13, 1, "Print training mode message on the receipt during training mode operations?")]
        public byte Print_training_mode_message_on_the_receipt_during_training_mode_operations;

        [BitfieldLength(14, 8, "Currency Symbol 1")]
        public byte Currency_Symbol_1;

        [BitfieldLength(15, 8, "Currency Symbol 2")]
        public byte Currency_Symbol_2;

        [BitfieldLength(16, 8, "Currency Symbol 3")]
        public byte Currency_Symbol_3;

        [BitfieldLength(17, 8, "Currency Symbol 4")]
        public byte Currency_Symbol_4;

        [BitfieldLength(18, 1, "Print the kitchen printer order number on receipt")]
        public byte Print_the_kitchen_printer_order_number_on_receipt;
        [BitfieldLength(18, 1, "Print the item’s price on the kitchen printer / requisition")]
        public byte Print_the_items_price_on_the_kitchen_printer_requisition;

  
        [BitfieldLength(19, 1, "Send/print order to the kitchen printer / requisition in void mode?")]
        public byte Send_print_order_to_the_kitchen_printer_requisition_in_void_mode;
        [BitfieldLength(19, 1, "Send/print order to the kitchen printer / requisition in training mode?")]
        public byte Send_print_order_to_the_kitchen_printer_requisition_in_training_mode;

        [BitfieldLength(20, 1, "Combine like items on the kitchen printer requisition")]
        public byte Combine_like_items_on_the_kitchen_printer_requisition;
        [BitfieldLength(20, 1, "Chooses volume unit when the PLU is gallonage.")]
        public byte Chooses_volume_unit_when_the_PLU_is_gallonage;

        [BitfieldLength(21, 1, "Print preamble message on receipt?")]
        public byte Print_preamble_message_on_receipt;
        [BitfieldLength(21, 1, "Print postamble message on receipt?")]
        public byte Print_postamble_message_on_receipt;

        [BitfieldLength(22, 1, "Print average items per customer on the Financial report")]
        public byte Print_average_items_per_customer_on_the_Financial_report;
        [BitfieldLength(22, 1, "Print average sales per customer on the Financial report")]
        public byte Print_average_sales_per_customer_on_the_Financial_report;

        [BitfieldLength(23, 1, "Issue a second receipt for the same transaction? (Buffer receipt issue when receipt printer is on)")]
        public byte Issue_a_second_receipt_for_the_same_transaction;
        [BitfieldLength(23, 1, "Priority print by group on the kitchen printer")]
        public byte Priority_print_by_group_on_the_kitchen_printer;
        [BitfieldLength(23, 1, "Print PLU number on the receipt?")]
        public byte Print_PLU_number_on_the_receipt;

        [BitfieldLength(24, 1, "Not print when polling reports? ")]
        public byte Not_print_when_polling_reports;
        [BitfieldLength(24, 1, "Print PLU number on PLU report")]
        public byte Print_PLU_number_on_PLU_report;
        [BitfieldLength(24, 1, "Grand total is")]
        public byte Grand_total_is; //1 net 0//gross

        [BitfieldLength(25, 1, "Send order to the kitchen printer when the SBTL key is pressed?")]
        public byte Send_order_to_the_kitchen_printer_when_the_SBTL_key_is_pressed; 

        [BitfieldLength(26, 1, "Print preamble graphic logo on receipt?")]
        public byte Print_preamble_graphic_logo_on_receipt; 

        [BitfieldLength(27, 1, "Print preamble graphic logo")]
        public byte Print_preamble_graphic_logo; //0 default 1-user

        [BitfieldLength(28, 8, "Number of Pre-feeding lines (0-5) on receipt ")]
        public byte Number_of_Prefeeding_lines_0_5_on_receipt; //0 default 1-user

        [BitfieldLength(29, 8, "Number of Post-feeding lines (0-5) on receipt ")]
        public byte Number_of_Postfeeding_lines_0_5_on_receipt; //0 default 1-user











    }
}
