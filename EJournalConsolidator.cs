using IniParser.Model;
using IniParser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VenusRetailPOSServer
{
    public partial class EJournalConsolidator : Form
    {
        public JournalDB db;
        List<string> strCache;
        public FileIO io;
        public List<string> linesb = new List<string>();

        private ReportsForm reportsForm;

        int progress = 0;
        private string SerialNo { get; set; }
        private string POSID { get; set; }
        private string DT_From { get; set; }
        private string DT_To { get; set; }
        public string[] ejcData;

        private int lineWidth = 0;

        public EJournalConsolidator(ReportsForm rf)
        {
            InitializeComponent();
            reportsForm = rf;
            dp_from.Value = DateTime.Today;
            db = new JournalDB(rf.loggedInPOSID);
            var parser = new FileIniDataParser();
            IniData appConfig = parser.ReadFile("config.ini");
            lineWidth = Int32.Parse(appConfig["PRINTER"]["lineChars"]);
            txt_posID.Text = rf.loggedInPOSID.ToString();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.saveWorker.CancelAsync();
            DisableButton(2);
        }

        private void frm_main_Load(object sender, EventArgs e)
        {

            strCache = new List<string>();
            dp_from.MaxDate = DateTime.Today;
            dp_to.MaxDate = DateTime.Today;
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            //Display("Seaching for transactions...");
            SearchDates(1);
        }

        private bool SearchDates(int function)
        {

            POSID = null;

            if (string.IsNullOrWhiteSpace(txt_posID.Text) == false)
            {
                POSID = txt_posID.Text;
            }
            else
            {
                MessageBox.Show("POS ID is not valid");
                return false;
            }

            TimeSpan startTime = TimeSpan.Parse("0:00");
            TimeSpan endTime = TimeSpan.Parse("23:59:59");
            DateTime dtf = dp_from.Value.Date.Add(startTime);
            DateTime dtt = dp_to.Value.Date.Add(endTime);

            string date_from = dtf.ToString();
            string date_to = dtt.ToString();
            DT_From = dtf.ToString("MMddyyyy");
            DT_To = dtt.ToString("MMddyyyy");
            switch (function)
            {
                case 1:
                    var result = db.CountTransaction(POSID, date_from, date_to);

                    if (result > 0)
                    {
                        EnableButton(1);
                    }

                    lbl_searchResult.Text = result + " item/s found";

                    return true;
                case 2:
                    strCache = db.ReadJournalData(POSID, date_from, date_to);
                    return true;
            }

            //Display("Seaching for transactions... Done!");
            return true;
        }

        private void DisableButton(int f)
        {
            switch (f)
            {
                case 1:
                    btn_save.Enabled = false;
                    btn_save.BackColor = Color.Gray;

                    break;
                case 2:
                    btn_cancel.Enabled = false;

                    break;

            }
        }

        private void EnableButton(int f)
        {
            switch (f)
            {
                case 1:
                    btn_save.Enabled = true;
                    btn_save.BackColor = Color.LimeGreen;

                    break;
                case 2:
                    btn_cancel.Enabled = true;

                    break;

            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            //Display("Exporting transaction collection");
            char[] invalids = Path.GetInvalidFileNameChars();
            string SNFromDB = db.GetSerialNumber(POSID);
            foreach (char c in invalids)
            {
                SNFromDB = SNFromDB.Replace(c, '-');
            }
            SerialNo = SNFromDB;
            progressBar1.Value = 0;
            resultLabel.Text = string.Empty;

            if (SearchDates(2))
            {
                progress = 0;
                saveWorker.RunWorkerAsync(strCache);
                DisableButton(1);
                EnableButton(2);
            }


            DialogResult shouldPrint = MessageBox.Show("Do you want to print electronic journal?", "Journal", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            Console.WriteLine(shouldPrint.ToString());
            if (shouldPrint == DialogResult.Yes)
            {
                DialogResult = DialogResult.OK;
                ActiveForm.Close();
            }
            else
            {
                DialogResult = DialogResult.Cancel;
                ActiveForm.Close();
            }
        }

        private void saveWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = WriteToTextFile((List<string>)e.Argument, worker, e);
        }

        public string WriteToTextFile(List<string> lines, BackgroundWorker worker, DoWorkEventArgs e)
        {
            string path1 = AppDomain.CurrentDomain.BaseDirectory + "\\Exports";
            if (!Directory.Exists(path1))
            {
                Directory.CreateDirectory(path1);
            }

            var parser = new FileIniDataParser();
            IniData appConfig = parser.ReadFile("config.ini");
            lineWidth = Int32.Parse(appConfig["PRINTER"]["lineChars"]);

            int crt = lines.Count;
            string fileName = "POS" + POSID + "_" + SerialNo + "_" + DT_From + "-" + DT_To;
            //pos#_sn#_date

            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Exports\\" + fileName + ".txt";

            if (!File.Exists(path))
                using (StreamWriter sw = File.CreateText(path))
                { };

            using (StreamWriter sw = File.AppendText(path))
            {
                int i = 0;
                DateTime nowDate = DateTime.Now;                            // System date
                DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();   // Date Format

                dateFormat.MonthDayPattern = "MM"; // Correctly set to "MM" for month
                string strDate = nowDate.ToString("MM-dd-yyyy hh:mm tt", dateFormat);

                Console.WriteLine("Start print");
                int a = 0;
                foreach (string x in lines)
                {

                    /* bool hasReprintString = x.ToLower().Contains("reprint");
                     bool isAccumulating = x.ToLower().Contains("accumulating");
                     bool isReport = x.ToLower().Contains("report");
                     bool afterSNMIN = false;*/


                    //linesb.Add(x);
                    int tyLine = 0;



                    foreach (string receiptLine in x.Split('\n'))
                    {
                        /*string reprintText = "*** Reprint ***";
                        string reprintDate = $"Reprint Date:{strDate}";
                        int totalLength = lineWidth;
                        int padding = (totalLength - reprintText.Length) / 2;
                        int paddingb = (totalLength - reprintDate.Length) / 2;


                        // Create the centered text
                        string centeredText = new string(' ', padding) + reprintText + new string(' ', padding);
                        string centeredTextb = new string(' ', paddingb) + reprintDate + new string(' ', paddingb);

                        if (afterSNMIN)
                        {
                            if (centeredText.Length < totalLength)
                            {
                                centeredText += " ";
                                centeredTextb += " ";

                            }

                            Console.WriteLine(centeredText + "this is not printed");
                            Console.WriteLine(centeredTextb + "this is not printed");
                            linesb.Insert(a, CenteredText(" "));
                            linesb.Insert(a + 1, centeredText);
                            linesb.Insert(a + 2, centeredTextb);

                            sw.WriteLine(CenteredText(""));
                            sw.WriteLine(centeredText);
                            sw.WriteLine(centeredTextb);

                            a++;
                            a++;
                            a++;
                            afterSNMIN = false;
                        }

                        if (isAccumulating && receiptLine.ToLower().Contains("sn:") && receiptLine.ToLower().Contains("min:") && !hasReprintString)
                        {
                            afterSNMIN = true;
                        }

                        string cutInBIRReceipt = new string('-', lineWidth);
                        if( ( receiptLine.Replace('=', '-') == cutInBIRReceipt ||receiptLine == cutInBIRReceipt) && x.ToLower().Contains("customer's copy"))
                        {
                            linesb.Add(" ");
                            linesb.Add(CenteredText("THIS DOCUMENT IS NOT VALID FOR CLAIM OF"));
                            linesb.Add(CenteredText("INPUT TAX"));
                            a = a + 3;

                            sw.WriteLine(CenteredText(" "));
                            sw.WriteLine(CenteredText("THIS DOCUMENT IS NOT VALID FOR CLAIM OF"));
                            sw.WriteLine(CenteredText("INPUT TAX"));
                        }

                        if ( ((receiptLine.Contains("POS #") && !isReport) || receiptLine.ToLower().Contains("report")) && !hasReprintString && !isAccumulating)
                        {
                            
                            

                            if (centeredText.Length < totalLength) 
                            {
                                centeredText += " ";
                                centeredTextb += " ";

                            }

                            Console.WriteLine(centeredText + "this is not printed");
                            Console.WriteLine(centeredTextb + "this is not printed");
                            //Console.WriteLine("true this");
                            linesb.Insert(a, centeredText);
                            linesb.Insert(a+1, centeredTextb);

                            //linesb.Insert(a + 1, centeredText);
                            //linesb.Insert(a + 2, centeredTextb);
                            //linesb.Insert(a, CenteredText(" "));

                            sw.WriteLine(centeredText);
                            sw.WriteLine(centeredTextb);
                            sw.WriteLine(CenteredText(""));
                            // Adjust the receiptLine if necessary
                            //receiptLine = receiptLine.Insert(a + 1, centeredText);
                            //receiptLine = receiptLine.Insert(a + 1, $"Reprint Date: {strDate}");
                            a++;
                            a++;
                        }*/

                        string receiptBreaker = new string('-', lineWidth);
                        if (receiptLine.Replace("\n", "") == receiptBreaker && x.ToLower().Contains("copy"))
                        {
                            linesb.Add(CenteredText(""));
                            linesb.Add(CenteredText("THIS DOCUMENT IS NOT VALID FOR CLAIM OF"));
                            linesb.Add(CenteredText("INPUT TAX"));
                            a = a + 3;

                            sw.WriteLine(CenteredText(""));
                            sw.WriteLine(CenteredText("THIS DOCUMENT IS NOT VALID FOR CLAIM OF"));
                            sw.WriteLine(CenteredText("INPUT TAX"));
                        }

                        linesb.Add(receiptLine.Replace("\n", "").Replace('=', '-'));
                        Console.WriteLine($"{receiptLine.Replace("\n", "")}");

                        a++;



                        sw.WriteLine(receiptLine.Replace('=', '-'));


                    }

                    reportsForm.Footers(out string[] f1, out string[] f2, out string[] f3, out string[] f4, out string[] f5);

                    if (!x.ToLower().Contains("accred"))
                    {
                        foreach (string s in f2)
                        {
                            linesb.Add(CenteredText(s));
                            sw.WriteLine(CenteredText(s));
                        }
                        linesb.Add(" ");
                        sw.WriteLine(" ");
                    }

                    //if(x.ToLower().Contains("thank you") && !x.ToLower().Contains("not valid for claim"))
                    if (!x.ToLower().Contains("not valid for claim"))
                    {
                        Console.WriteLine(CenteredText("THIS DOCUMENT IS NOT VALID FOR CLAIM OF"));
                        Console.WriteLine(CenteredText("INPUT TAX"));
                        linesb.Add(CenteredText("THIS DOCUMENT IS NOT VALID FOR CLAIM OF"));
                        linesb.Add(CenteredText("INPUT TAX"));
                        a = a + 2;

                        sw.WriteLine(CenteredText("THIS DOCUMENT IS NOT VALID FOR CLAIM OF"));
                        sw.WriteLine(CenteredText("INPUT TAX"));
                    }

                    Console.WriteLine("");
                    Console.WriteLine(new string('-', lineWidth));
                    Console.WriteLine("");
                    linesb.Add("");
                    linesb.Add(new string('-', lineWidth));
                    linesb.Add("");
                    a = a + 3;

                    sw.WriteLine(" ");
                    sw.WriteLine(new string('-', 42));
                    sw.WriteLine(" ");
                    Console.WriteLine("Added line in text file");

                    /*if (receiptLine.Contains("Thank you"))
                    {*/
                    /*   linesb.Add("");
                       linesb.Add(new string('-', lineWidth));
                       linesb.Add("");
                   a = a + 2;*/
                    //}


                    //if (x.Contains("SN: DSC 1201-3501-1 MIN: 0000001")) 
                    //{
                    //    Console.WriteLine("*** Reprint ***");
                    //    Console.WriteLine($"Reprint Date:{strDate}");
                    //    //Console.WriteLine("true this");
                    //   linesb.Insert(i+1,"*** Reprint ***");
                    //    linesb.Insert(i+2,$"Reprint Date:{strDate}");
                    //}
                    //Console.WriteLine(no++);
                    //Console.WriteLine(x);
                    i++;
                    int pctg = (int)((float)i / (float)crt * 100);
                    worker.ReportProgress(pctg);
                }

                Console.WriteLine(new string('=', 55));
                Console.WriteLine("test receipt ");
                foreach (string lin in linesb)
                {
                    Console.WriteLine(lin);
                }
                //foreach(string y in linesb)
                //{

                //Console.WriteLine(y);
                //}
                //ejcData = linesb;
                //foreach(string z in linesb)
                //{
                //    Console.WriteLine(z);
                //}
            }
            return "Completed";
        }

        private string CenteredText(string text)
        {
            int sideSpace = (lineWidth - text.Length) / 2;
            string centeredString = new string(' ', sideSpace) + text + new string(' ', sideSpace);
            return centeredString;
        }

        private void saveWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            progress++;
           // DisplayRunning(progress);
        }

        private void saveWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // Next, handle the case where the user canceled 
                // the operation.
                // Note that due to a race condition in 
                // the DoWork event handler, the Cancelled
                // flag may not have been set, even though
                // CancelAsync was called.
                resultLabel.Text = "Cancelled";
               // Display("Action cancelled");
            }
            else
            {
                // Finally, handle the case where the operation 
                // succeeded.

                resultLabel.Text = e.Result.ToString();
              //  Display(e.Result.ToString());
            }
            strCache.Clear();
            DisableButton(1);
            DisableButton(2);
        }
    }
}
