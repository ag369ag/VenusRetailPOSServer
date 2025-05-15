using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusRetailPOSServer
{
    public class EoD_Report
    {
        public int POSID { get; set; }
        public int Period_ID { get; set; }
        public string DT_Generated { get; set; }
        public string SI_Start { get; set; }
        public string SI_End { get; set; }
        public decimal AccSalesEnd { get; set; }
        public decimal AccSalesBegin { get; set; }
        public decimal ManualSales { get; set; }
        public decimal GrossSales { get; set; }
        public decimal VATABLE_Sales { get; set; }
        public decimal VAT_Amount { get; set; }
        public decimal VAT_Exempt { get; set; }
        public decimal ZeroRated { get; set; }
        public decimal Deduct_SC { get; set; }
        public decimal Deduct_PWD { get; set; }
        public decimal Deduct_NAAC { get; set; }
        public decimal Deduct_SP { get; set; }
        public decimal Deduct_Others { get; set; }
        public decimal Deduct_Returns { get; set; }
        public decimal Deduct_Total { get; set; }
        public decimal NetSales { get; set; }
        public int ResetCounter { get; set; }
        public int ZCounter { get; set; }
        public decimal Deduct_MoV { get; set; }
    }
    public class Reports
    {
        public class EodReport
        {
            public string Period_ID { get; set; }
            public string POS_ID { get; set; }
            public string Sales_Present { get; set; }
            public string Sales_Previous { get; set; }
            public string Daily_Sales { get; set; }
            public string Invoice_Begin { get; set; }
            public string Invoice_End { get; set; }
            public string Reset_Start { get; set; }
            public string Reset_End { get; set; }
            public string Reset_TRS_End { get; set; }
            public string Reset_TRS_Start { get; set; }
            public string Reset_Counter { get; set; }
            public string Previous_Reset_Counter { get; set; }
            public string Z_Counter { get; set; }
            public string Invoice_Qty { get; set; }
            public string Refund_Qty { get; set; }
            public string TRS_Start { get; set; }
            public string TRS_End { get; set; }



        }

        public class EodTaxHist
        {
            public int Tax_ID { get; set; }
            public int Period_ID { get; set; }
            public int POS_ID { get; set; }
            public string Tax_Name { get; set; }
            public decimal Sale_Value { get; set; }
            public decimal Discount_Value { get; set; }
            public decimal Senior_Disc_Value { get; set; }
            public decimal Refund_Value { get; set; }
            public decimal Refund_Discount_Value { get; set; }
            public decimal Refund_Senior_Disc_Value { get; set; }
            public decimal NonTax_Discount_Value { get; set; }
            public decimal NonTax_Disc_Refund { get; set; }
        }

        public class EodDiscHist
        {
            public int Discount_Preset_ID { get; set; }
            public int Period_ID { get; set; }
            public int POS_ID { get; set; }
            public decimal Discount_Value { get; set; }
            public string Discount_Qty { get; set; }
        }
        public class PeriodDetails
        {
            public string Period_ID { get; set; }
            public string Period_Create_TS { get; set; }
            public string Period_Close_DT { get; set; }
        }

        public class DeptSales
        {
            public string depName { get; set; }
            public decimal qty { get; set; }
            public decimal val { get; set; }
        }
        public class FuelSales
        {
            public string Grade_Name { get; set; }
            public int postPayQty { get; set; }
            public decimal postPayVol { get; set; }
            public decimal postPayVal { get; set; }
            public int testDelQty { get; set; }
            public decimal testDelVol { get; set; }
            public int offQty { get; set; }
            public decimal offVol { get; set; }
            public decimal offVal { get; set; }
            public int driveOffQty { get; set; }
            public decimal driveOffVal { get; set; }
            public decimal driveOffVol { get; set; }
            public decimal driveOffCost { get; set; }
        }

        public class AccDiscSales
        {
            public decimal qty { get; set; }
        }

        public class FinHistory
        {
            public int id { get; set; }
            public string name { get; set; }
            public decimal netQty { get; set; }
            public decimal netValue { get; set; }
            public decimal numSafedrop { get; set; }
            public decimal valSafedrop { get; set; }
            public decimal numCashout { get; set; }
            public decimal valCashout { get; set; }
        }

        public class CashiersTotal
        {
            public int Cashier_ID { get; set; }
            public string Cashier_Name { get; set; }
            public int Period_ID { get; set; }
            public int Cshr_Num_Trs { get; set; }
            public decimal Cshr_Val_Trs { get; set; }
            public int Cshr_Num_Void { get; set; }
            public decimal Cshr_Val_Void { get; set; }
            public int Cshr_Num_NoSales { get; set; }
            public int Cshr_Num_Refund { get; set; }
            public decimal Cshr_Val_Refund { get; set; }
        }

        public class ManualSales
        {
            public string Grade_Name { get; set; }
            public int postPayQty { get; set; }
            public decimal postPayVol { get; set; }
            public decimal postPayVal { get; set; }
            public int refundQty { get; set; }
            public decimal refundVol { get; set; }
            public decimal refundVal { get; set; }
            public decimal taxExVal { get; set; }
            public decimal taxExValRef { get; set; }
        }

        public class DeptTaxExempt
        {
            public string depName { get; set; }
            public int qty { get; set; }
            public decimal val { get; set; }
        }

        public class DeliveryTaxExempt
        {
            public string Grade_Name { get; set; }
            public decimal gradeTaxExemptValue { get; set; }
            public decimal gradeTaxExemptQty { get; set; }
        }
        public class ManualTaxExempt
        {
            public string Grade_Name { get; set; }
            public decimal hoseTaxExVol { get; set; }
            public decimal hoseTaxExVal { get; set; }
        }

        public class TaxHistory
        {
            public int Tax_ID { get; set; }
            public string Tax_Name { get; set; }
            public int Period_ID { get; set; }
            public decimal Tax_Tot_Value { get; set; }
            public decimal Tax_Sale_Value { get; set; }
            public decimal? Tax_NonTaxable_Discount { get; set; }
        }

        public class DeptDiscounts
        {
            public string depName { get; set; }
            public decimal qty { get; set; }
            public decimal val { get; set; }
        }

        public class ManualFuelDiscount
        {
            public string Grade_Name { get; set; }
            public decimal hoseVolDisc { get; set; }
            public decimal hoseValDisc { get; set; }
            public int hoseQtyDisc { get; set; }
        }

        public class FuelDiscount
        {
            public string Grade_Name { get; set; }
            public decimal hoseVolDisc { get; set; }
            public decimal hoseValDisc { get; set; }
            public int hoseQtyDisc { get; set; }
        }

        public class DiscountTotal
        {
            public string discount_name { get; set; }
            public int discQty { get; set; }
            public decimal discVal { get; set; }
        }

        public class DepRefunds
        {
            public string depName { get; set; }
            public decimal qty { get; set; }
            public decimal val { get; set; }
        }

        public class TransHourData
        {
            public string TransactionHour { get; set; }
            public int transCount { get; set; }
            public decimal saleTotal { get; set; }
            public decimal? Quantity { get; set; }
        }

        public class HoseHistData
        {
            public int hoseID { get; set; }
            public int hoseNum { get; set; }
            public string gradeName { get; set; }
            public int pumpID { get; set; }
            public string pumpName { get; set; }
            public int logicalNum { get; set; }
            public int periodID { get; set; }
            public decimal openMeterVal { get; set; }
            public decimal openMeterVol { get; set; }
            public decimal closeMeterVal { get; set; }
            public decimal closeMeterVol { get; set; }
            public decimal postpayVal { get; set; }
            public decimal postpayVol { get; set; }
            public decimal driveoffVal { get; set; }
            public decimal driveoffVol { get; set; }
            public decimal testDelVol { get; set; }
        }

        public class TransactionNumbers
        {
            public string endingTransNum { get; set; }
            public string startingTransNum { get; set; }
            public string endingTransReset { get; set; }
            public string startingTransReset { get; set; }
            public int posID { get; set; }
            public int Qty { get; set; }
        }

        public class VehicleTotals
        {
            public string name { get; set; }
            public int count { get; set; }
        }

        public class TankHistory
        {
            public string tankName { get; set; }
            public decimal openVolume { get; set; }
            public decimal closeVolume { get; set; }
            public decimal addedVolume { get; set; }
            public decimal soldVolume { get; set; }
        }

        public class DeliveryRefund
        {
            public string gradeName { get; set; }
            public decimal qty { get; set; }
            public decimal val { get; set; }
        }

        public class ManualRefund
        {
            public string Grade_Name { get; set; }
            public decimal hoseVolRef { get; set; }
            public decimal hoseValRef { get; set; }
        }
    }
    public class PeriodList
    {
        public int Period_ID { get; set; }
        public string Period_Create_TS { get; set; }
        public string Period_Close_DT { get; set; }
        public int Shift_number { get; set; }
    }

    public class CashDraw
    {
        public class CDrawFinHistory
        {
            public int MOP_ID { get; set; }
            public string MOP_Name { get; set; }
            public decimal CDraw_Tot_Amount { get; set; }
            public decimal CDraw_Float { get; set; }
            public int CDraw_Num_Safedrop { get; set; }
            public decimal CDraw_Tot_Safedrop { get; set; }
            public int CDraw_Num_Payin { get; set; }
            public decimal CDraw_Tot_Payin { get; set; }
            public int CDraw_Num_CashOut { get; set; }
            public decimal CDraw_Tot_CashOut { get; set; }
            public decimal CDraw_Tot_Refund { get; set; }
        }
        public class CDrawGradeHistory
        {
            public int Grade_ID { get; set; }
            public string Grade_Name { get; set; }
            public int CDrawGrade_Trs { get; set; }
            public decimal CDrawGrade_Vol { get; set; }
            public decimal CDrawGrade_Val { get; set; }
            public decimal CDrawGrade_Vol_Disc { get; set; }
            public decimal CDrawGrade_Val_Disc { get; set; }
            public decimal CDrawGrade_Vol_Ref { get; set; }
            public decimal CDrawGrade_Val_Ref { get; set; }
        }
        public class CDrawDeptHistory
        {
            public int Department_ID { get; set; }
            public string Dept_Name { get; set; }
            public decimal CDrawDept_Qty_Sld { get; set; }
            public decimal CDrawDept_Val_Sld { get; set; }
            public decimal CDrawDept_Qty_Ref { get; set; }
            public decimal CDrawDept_Val_Ref { get; set; }
            public decimal CDrawDept_Qty_Disc { get; set; }
            public decimal CDrawDept_Val_Disc { get; set; }

        }
        public class CDrawDetails
        {
            public int CDraw_Period_ID { get; set; }
            public string Cashier_Name { get; set; }
            public string CDraw_Open_Date { get; set; }
            public string CDraw_Close_Date { get; set; }
        }
        public class CDrawManualGradeHistory
        {
            public int Grade_ID { get; set; }
            public string Grade_Name { get; set; }
            public int CDrawGrade_Trs { get; set; }
            public decimal CDrawGrade_Vol { get; set; }
            public decimal CDrawGrade_Val { get; set; }
            public decimal CDrawGrade_Vol_Disc { get; set; }
            public decimal CDrawGrade_Val_Disc { get; set; }
            public decimal CDrawGrade_Vol_Ref { get; set; }
            public decimal CDrawGrade_Val_Ref { get; set; }
        }
    }
    public class CashDrawList
    {
        public int CDraw_Period_ID { get; set; }
        public string Cashier_Name { get; set; }
        public string CDraw_Close_Date { get; set; }
    }
    public class CummulativeSales
    {
        public class DiscPresetHistSum
        {
            public string preset_name { get; set; }
            public int discount_id { get; set; }
            public int preset_id { get; set; }
            public int pos_id { get; set; }
            public int discount_qty { get; set; }
            public decimal discount_val { get; set; }
        }
        public class GradeHistSum
        {
            public int grade_id { get; set; }
            public string Grade_Name { get; set; }
            public int pos_id { get; set; }
            public int delivery_type { get; set; }
            public int grade_trs { get; set; }
            public decimal grade_vol { get; set; }
            public decimal grade_val { get; set; }
        }

        public class DeptHistSum
        {
            public int department_id { get; set; }
            public string Dept_Name { get; set; }
            public int pos_id { get; set; }
            public decimal dept_qty_item_sold { get; set; }
            public decimal dept_val_item_sold { get; set; }
        }

        public class MopHistSum
        {
            public int Mop_ID { get; set; }
            public string Mop_Name { get; set; }
            public int pos_id { get; set; }
            public decimal Mop_Net_Sale_Value { get; set; }
        }

        public class TaxHistSum
        {
            public string taxName { get; set; }
            public decimal taxSaleVal { get; set; }
            public int pos_id { get; set; }
        }

        public class DiscTotal
        {
            public string taxName { get; set; }
            public decimal discSaleVal { get; set; }
            public decimal discTaxVal { get; set; }
            public int pos_id { get; set; }
        }

        public class SaleTotal
        {
            public string taxName { get; set; }
            public decimal saleVal { get; set; }
            public decimal saleTaxVal { get; set; }
            public int pos_id { get; set; }
            public string nonTaxDiscValue { get; set; }
        }

        public class ScpwdDiscTotal
        {
            public string taxName { get; set; }
            public decimal scpwdDiscVal { get; set; }
            public decimal scowdTaxVal { get; set; }
            public int pos_id { get; set; }
        }

        public class Details
        {
            public string endDate { get; set; }
            public string lastReportDate { get; set; }
            public string startDate { get; set; }
            public string begSalesInvoice { get; set; }
            public string endSalesInvoice { get; set; }
            public string begSalesResetter { get; set; }
            public string endSalesResetter { get; set; }
            public int ZCounter { get; set; }
            public decimal Previous_Accumulating_Total_Sales { get; set; }
            public decimal Current_Accumulating_Total_Sales { get; set; }
        }

        public class NetSales
        {
            public string taxName { get; set; }
            public decimal saleValue { get; set; }
            public decimal vatAmount { get; set; }
            public decimal NonVatDiscount { get; set; }
        }


        public class RefundSummary
        {
            public int pos_id { get; set; }
            public decimal vatable { get; set; }
            public decimal vat_exempt { get; set; }
            public decimal zero_rated { get; set; }
        }



    }
    public class AccumulatingReport
    {
        public int PosID { get; set; }
        public string LastReportDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTransNumber { get; set; }
        public string EndTransNumber { get; set; }
        public decimal PreviousNetSales { get; set; }
        public decimal CurrentNetSales { get; set; }
        public decimal DifferenceNetSales { get; set; }
        public decimal DiscountSeniorPWD { get; set; }
        public decimal DiscountOther { get; set; }
        public decimal RefundTotal { get; set; }
        public decimal Shortage { get; set; }
        public decimal Overage { get; set; }
        public decimal NetVatableSales { get; set; }
        public decimal NetVatAmount { get; set; }
        public decimal NetExemptSales { get; set; }
        public decimal NetZeroRated { get; set; }
        public int ZCounter { get; set; }
    }
    public class VehicleType
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class VoidAndRefund
    {
        public List<RefundReport> refundData { get; set; }
        public List<VoidReport> voidData { get; set; }
    }

    public class RefundReport
    {
        public int transID { get; set; }
        public int cashierID { get; set; }
        public string cashierName { get; set; }
        public int POSID { get; set; }
        public DateTime transDate { get; set; }
        public decimal saleTotal { get; set; }
        public List<RefundItem> saleItems { get; set; }
    }

    public class RefundItem
    {
        public int Item_number { get; set; }
        public string Item_Description { get; set; }
        public decimal Value { get; set; }
        public int Item_Type { get; set; }
    }

    public class VoidReport
    {
        public DateTime Void_Event_Date { get; set; }
        public int POS_ID { get; set; }
        public string Item_Description { get; set; }
        public decimal Item_Value { get; set; }
        public int Cashier_ID { get; set; }
        public string Cashier_Name { get; set; }
    }
}
