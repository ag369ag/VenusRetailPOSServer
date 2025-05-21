using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VenusRetailPOSServer
{
    public partial class BOF : Form
    {
        public string CashierName { private get; set; }
        public DateTime GT_OpenDate { get; set; }
        EnbDB enb;
        private int posNumber = 0;
        public BOF(int posID)
        {
            InitializeComponent();
            enb = new EnbDB(posID);
            posNumber = posID;
        }

        private void BOF_Load(object sender, EventArgs e)
        {
            var now = DateTime.Now;

            dt_from.MinDate = GT_OpenDate;
            dt_from.MaxDate = now;
            dt_from.Value = now;

            dt_to.MinDate = GT_OpenDate;
            dt_to.MaxDate = now;
            dt_to.Value = now;

            ts_label.Text = "Waiting for input...";
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            ActiveForm.Close();
        }
        private string CreateRange(List<int> pid)
        {
            if (pid.Count == 0)
                return null;


            string wh = " AND PERIOD_ID IN (";
            foreach (int x in pid)
                wh += $"{x},";

            wh = wh.TrimEnd(',');
            wh += ")";
            return wh;

        }
        private List<string> GenerateBIRSalesReport(List<EnbDB.CSVHeader> headers, string csvHeader, int function)
        {
            string from = dt_from.Value.ToString("yyyy-MM-dd");
            string to = dt_to.Value.ToString("yyyy-MM-dd");
            Console.WriteLine($"from: {from} and to : {to}");
            var validPeriodIDs = enb.GetPeriodIDs(to, from);
            string whereClause = CreateRange(validPeriodIDs);
            Console.WriteLine($"where clause: {whereClause}");
            List<string> lines = new List<string>();
            if (validPeriodIDs.Count == 0)
            {
                MessageBox.Show("Unabale to Generate CSV, Please execute close day");

                return null;
            }

            switch (function)
            {
                case 1:
                    {
                        lines.Add("BIR Sales Summary Report");
                    }
                    break;
                case 2:
                    {
                        lines.Add("Senior Citizen Report");
                    }
                    break;
                case 3:
                    {
                        lines.Add("Person with Disability Report");
                    }
                    break;
                case 4:
                    {
                        lines.Add("National Athletes and Coaches Report");
                    }
                    break;
                case 5:
                    {
                        lines.Add("Solo Parent Report");
                    }
                    break;
                case 6:
                    {
                        lines.Add("Medal of Valor Report");
                    }
                    break;
                default:
                    { }
                    break;
            }

            foreach (EnbDB.CSVHeader x in headers)
            {
                var splitH = Regex.Split(x.Data.Trim(), @"\\n");
                foreach (string y in splitH)
                    lines.Add(y);
                lines.Add("");
                lines.Add($"{x.Description}");
                lines.Add($"SN: {x.SerialNo}");
                lines.Add($"MIN: {x.POS_Min}");
                lines.Add($"POS #{x.POS_ID}");
                lines.Add(DateTime.Now.ToString());
                lines.Add($"{CashierName}");
                lines.Add("");
                lines.Add(csvHeader);
                //lines.Add("PosID,LastReportDate,StartDate,EndDate,StartTransNumber,EndTransNumber,PreviouAccumulatingTotalNetSales,CurrentAccumulatingTotalNetSales,DifferenceAccumulatingTotalNetSales,SnrPWDDiscounts,OtherDiscounts,TotalRefunds,Shortages,Overages,NetVatableSales,NetVatAmount,NetExemptSales,NetZeroRatedSales,ZCounter");
                try
                {
                    List<string> result = new List<string>();
                    switch (function)
                    {
                        case 1:
                            result = enb.BIRSalesSummary(x.POS_ID, whereClause);
                            break;
                        case 2:
                            result = enb.GetSCReports(x.POS_ID, whereClause);
                            break;
                        case 3:
                            result = enb.GetPWDReports(x.POS_ID, whereClause);
                            break;
                        case 4:
                            result = enb.GetNAACReports(x.POS_ID, whereClause);
                            break;
                        case 5:
                            result = enb.GetSPReports(x.POS_ID, whereClause);
                            break;
                        case 6:
                            result = enb.GetMoVReports(x.POS_ID, whereClause);
                            break;

                    }
                    lines.AddRange(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to Generate Report" + Environment.NewLine
                        + "Reason: " + ex.Message, "Error");
                    return null;
                }

                lines.Add("");
                lines.Add("");
            }
            return lines;
        }

        private void btn_generate_Click(object sender, EventArgs e)
        {
            var head = enb.GetHeaders();
            string csvh = "";

            if (chk_birsummary.Checked)
            {
                //csvh = "Date,Beginning SI/OR No.,Ending SI/OR No.,Grand Accum. Sales Ending Balance, Grand Accum. Beg. Balance, Sales Issued w/ Manual SI/OR,Gross Sales for the Day, VATable Sales, VAT Amount,VAT-Exempt Sales,Zero-Rated Sales,Deductions Discount SC,Deductions Discount PWD,Deductionss Discount NAAC,Deductions Solo Parent, Deductions MoV, Deductions Discount Others,Deductions Return,Deductions Voids,Deductions Total Deductions,Adjustment Discount SC,Adjustment Discount SC,Adjustment Discount PWD,Adjustment Discount Others, Adjustment VAT on Returns,Adjustment Others,Adjustment Total VAT Adjustment,VAT Payable,Net Sales,Sales Overrun/Overflow,Total Income,Reset Counter,Z-Counter,Remarks";
                csvh = "Date,Beginning SI/OR No.,Ending SI/OR No.,Grand Accum. Sales Ending Balance, Grand Accum. Beg. Balance, Sales Issued w/ Manual SI/OR,Gross Sales for the Day, VATable Sales, VAT Amount,VAT-Exempt Sales,Zero-Rated Sales,Deductions Discount SC,Deductions Discount PWD,Deductionss Discount NAAC,Deductions Solo Parent, Deductions MoV, Deductions Discount Others,Deductions Return,Deductions Voids,Deductions Total Deductions,Adjustment Discount SC,Adjustment Discount PWD,Adjustment Discount Others, Adjustment VAT on Returns,Adjustment Others,Adjustment Total VAT Adjustment,VAT Payable,Net Sales,Sales Overrun/Overflow,Total Income,Reset Counter,Z-Counter,Remarks";
                var data = GenerateBIRSalesReport(head, csvh, 1);
                string fname = $"POS#{posNumber} - BIR_SALES_SUMMARY_REPORT - {DateTime.Now.Date.ToShortDateString().Replace('/', '_')}.csv";
                ProcessExport(fname, data);
            }
            if (chk_senior.Checked)
            {
                csvh = "Date,Name Of Senior Citizen (SC),OSCA ID No./SC ID No.,SC TIN,SI/OR Number,Sales (inclusive of VAT),VAT Amount,VAT Exempt Sales,Discount 5%,Discount 20%,Net Sales";
                var data = GenerateBIRSalesReport(head, csvh, 2);
                string fname = $"POS#{posNumber} - SENIOR_CITIZEN_SALES_REPORT - {DateTime.Now.Date.ToShortDateString().Replace('/', '_')}.csv";
                ProcessExport(fname, data);
            }
            if (chk_pwd.Checked)
            {
                csvh = "Date,Name Of Person with Disability (PWD),PWD ID No.,PWD TIN,SI/OR Number,Sales (inclusive of VAT),VAT Amount,VAT Exempt Sales,Discount 5%,Discount 20%,Net Sales";
                var data = GenerateBIRSalesReport(head, csvh, 3);
                string fname = $"POS#{posNumber} - PERSON_WITH_DISABILTY_SALES_REPORT - {DateTime.Now.Date.ToShortDateString().Replace('/', '_')}.csv";
                ProcessExport(fname, data);
            }
            if (chk_naac.Checked)
            {
                csvh = "Date,Name of National Athlete/Coach,PNSTM ID No., SI/OR Number,Gross Sales/Receipts,Sales Discount,Net Sales";
                var data = GenerateBIRSalesReport(head, csvh, 4);
                string fname = $"POS#{posNumber} - NATIONAL_ATHLETE_AND_COACH_SALES_REPORT - {DateTime.Now.Date.ToShortDateString().Replace('/', '_')}.csv";
                ProcessExport(fname, data);
            }
            if (chk_sp.Checked)
            {
                csvh = "Date,Name of Solo Parent, SPIC No.,Name of child,Birth Date of child, Age of child, SI/OR Number,Gross Sales,Discount,Net Sales";
                var data = GenerateBIRSalesReport(head, csvh, 5);
                string fname = $"POS#{posNumber} - SOLO_PARENT_SALES_REPORT - {DateTime.Now.Date.ToShortDateString().Replace('/', '_')}.csv";
                ProcessExport(fname, data);
            }

            if (chk_mov.Checked)
            {
                csvh = "Date,Name Of Medal Holder,MoV ID No./MoV ID No.,MoV TIN,SI/OR Number,Sales (inclusive of VAT),VAT Amount,VAT Exempt Sales,Discount 5%,Discount 20%,Net Sales";
                var data = GenerateBIRSalesReport(head, csvh, 6);
                string fname = $"POS#{posNumber} - MEDAL_OF_VALOR_SALES_REPORT - {DateTime.Now.Date.ToShortDateString().Replace('/', '_')}.csv";
                ProcessExport(fname, data);
            }
        }

        private void ProcessExport(string filename, List<string> data)
        {
            if (FileIO.WriteToFile(data, filename) == true)
            {
                ts_label.Text = $"\u2713 Export Done for {filename}";
                //setMessage("Export done!", MessageType.Info);
            }
            else
            {
                ts_label.Text = $"\u274c Export failed for {filename}";
                //setMessage("Export failed!", MessageType.Error);
            }
        }

        private void Select_All_Toggled(object sender, EventArgs e)
        {
            CheckBox selectAll = sender as CheckBox;
            if (selectAll.Checked)
            {
                chk_mov.Checked = true;
                chk_sp.Checked = true;
                chk_naac.Checked = true;
                chk_pwd.Checked = true;
                chk_senior.Checked = true;
                chk_birsummary.Checked = true;
            }
            else
            {
                if (SetSelectAllOn())
                {
                    chk_mov.Checked = false;
                    chk_sp.Checked = false;
                    chk_naac.Checked = false;
                    chk_pwd.Checked = false;
                    chk_senior.Checked = false;
                    chk_birsummary.Checked = false;
                }
            }
            /*else
            {
                chk_mov.Checked = false;
                chk_sp.Checked = false;
                chk_naac.Checked = false;
                chk_pwd.Checked = false;
                chk_senior.Checked = false;
                chk_birsummary.Checked = false;
            }*/
        }

        private void SetSelectAllOff(CheckBox cb)
        {
            if (!cb.Checked)
            {
                selectAllCheckBox.Checked = false;
            }
            else
            {
                SetSelectAllOn();
            }
        }

        private bool SetSelectAllOn()
        {
            if(
                chk_mov.Checked == true &&
            chk_sp.Checked == true &&
            chk_naac.Checked == true &&
            chk_pwd.Checked == true &&
            chk_senior.Checked == true &&
            chk_birsummary.Checked == true)
            {
                selectAllCheckBox.Checked = true;
                return true;
            }
            else
            {
                selectAllCheckBox.Checked = false;
                return false;
            }
        }

        private void chk_birsummary_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chckbx = sender as CheckBox;
            SetSelectAllOff(chckbx);
        }

        private void chk_senior_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chckbx = sender as CheckBox;
            SetSelectAllOff(chckbx);
        }

        private void chk_pwd_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chckbx = sender as CheckBox;
            SetSelectAllOff(chckbx);
        }

        private void chk_naac_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chckbx = sender as CheckBox;
            SetSelectAllOff(chckbx);
        }

        private void chk_sp_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chckbx = sender as CheckBox;
            SetSelectAllOff(chckbx);
        }

        private void chk_mov_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chckbx = sender as CheckBox;
            SetSelectAllOff(chckbx);
        }
    }
}
