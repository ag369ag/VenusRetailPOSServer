using IniParser;
using IniParser.Model;
using Microsoft.PointOfService;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VenusRetailPOSServer
{
    public partial class ReportsForm : Form
    {
        Cashier activeCashier;
        public int loggedInPOSID;
        bool CloseDayStarted = false;
        API api;
        EnbDB enb;
        int lineChars = 0;
        RECEIPT rct;
        FileIO fileIO;
        public IniData appConfig;
        PosPrinter m_Printer = null;
        Splash splash;
        CONFIG cfg;
        PeriodData periodData;
        public List<Tax> tx;
        public List<CashDrawList> cdList;
        public CashDrawListForm cdldlg;
        public List<PeriodList> periodLists;
        public ShiftList shiftdlg;
        //public bool drawerUsable;
        //CashDrawer m_Drawer = null;
        const int MAX_LINE_WIDTHS = 2;
        public decimal VATRate = 0;
        public ReportsForm(int posID, Cashier cashier)
        {
            InitializeComponent();
            loggedInPOSID = posID;
            activeCashier = cashier;
            api = new API(posID);
            enb = new EnbDB(loggedInPOSID);
            rct = new RECEIPT();
            fileIO = new FileIO();

            splash = new Splash();
            splash.Show();

            splash.AddLoadingInfo("Starting VenusPOS...");
            splash.AddLoadingInfo("Inititalizing objects...");

            splash.AddLoadingInfo("Reading INI file...");
            ReadConfig();

            lineChars = Int32.Parse(appConfig["PRINTER"]["lineChars"]);
            
            try
            {
                splash.AddLoadingInfo("Resetting sale windows...");
                

                splash.AddLoadingInfo("Loading POS data...");
                cfg = api.GetConfig();

                splash.AddLoadingInfo("Loading Period data...");
                var pData = api.GetPeriod(activeCashier.id);
                if (pData == "false")
                {
                    MessageBox.Show("Unable to fetch period data");
                    return;
                }
                else
                {
                    periodData = JsonConvert.DeserializeObject<PeriodData>(pData);
                }


                splash.AddLoadingInfo("Loading Receipts Layout...");
                rct = api.GetReceipt();


                splash.AddLoadingInfo("Loading taxes...");
                tx = api.GetTaxList();
                //Add value to tax
                VATRate = (tx.GetTaxRateByName("VAT") / 100) + 1M;

/*                splash.AddLoadingInfo("Loading products....");
                products = api.GetProducts();

                //splash.AddLoadingInfo("Loading ca...");
                //vehicles = api.GetVehicleType();

                //string response = api.GetDiscounts();
                splash.AddLoadingInfo("Creating product buttons...");
                CreateProductsButton(1, 1);*/

                splash.AddLoadingInfo("Initializing printer...");
                // error in initializing pos explorer in printer init and cash draw init
                // another error for save journal and save journal exe is missing for transaction payment
                enb = new EnbDB(loggedInPOSID);
                var xx = PrinterInit();

                splash.AddLoadingInfo("Claiming printer...");
                if (xx)
                    ClaimPrinter();

                splash.EnableCloseButton(true);
                this.splash.Close();
                //splash.AddLoadingInfo("Initializing cash drawer...");
                //drawerUsable = CashDrawInit();

                //DrawInit();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Application Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        
        private bool ClaimPrinter()
        {
            try
            {
                //Get the exclusive control right for the opened device.
                //Then the device is disable from other application.
                m_Printer.Claim(1000);

                //Enable the device.
                m_Printer.DeviceEnabled = true;

                //Output by the high quality mode
                //m_Printer.RecLetterQuality = true;

                // Even if using any printers, 0.01mm unit makes it possible to print neatly.
                m_Printer.MapMode = MapMode.Metric;

                return true;
            }
            catch (PosControlException e)
            {
                MessageBox.Show("Claiming Printer Failed");
                MessageBox.Show("Printer error, Please check your printer and try again", "Printer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        private bool PrinterInit()
        {
            string strLogicalName = "POSPrinter";
            
            try
            {
                //Create PosExplorer

                PosExplorer posExplorer = new PosExplorer();

                DeviceInfo deviceInfo = null;

                try
                {

                    deviceInfo = posExplorer.GetDevice(DeviceType.PosPrinter, strLogicalName);
                    m_Printer = (PosPrinter)posExplorer.CreateInstance(deviceInfo);

                }
                catch (Exception f)
                {
                    MessageBox.Show(f.Message, "No Printer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    m_Printer = null;
                    return false;
                }
                if (appConfig["PRINTER"]["ignoreerror"] == "false")
                {
                    AddStatusUpdateEvent(m_Printer);
                    AddErrorEvent(m_Printer);
                }
                //Open the device
                m_Printer.Open();
                return true;

            }
            catch (PosControlException f)
            {
                m_Printer = null;
                //MessageBox.Show("Unable to initialize printer." + Environment.NewLine + f.Message, "Printer Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                splash.AddLoadingInfo("Printer Initialization Error!!!");
                return false;
            }
            catch (Exception e)
            {
                m_Printer = null;
                //MessageBox.Show("Unable to initialize printer." + Environment.NewLine + f.Message, "Printer Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                splash.AddLoadingInfo("Printer Initialization Error!!!");
                return false;
            }
        }

        protected void AddStatusUpdateEvent(object eventSource)
        {

            EventInfo statusUpdateEvent = eventSource.GetType().GetEvent("StatusUpdateEvent");
            if (statusUpdateEvent != null)
            {
                statusUpdateEvent.AddEventHandler(eventSource,
                    new StatusUpdateEventHandler(OnStatusUpdateEvent));
            }

        }

        protected void OnStatusUpdateEvent(object source, StatusUpdateEventArgs e)
        {
            if (InvokeRequired)
            {
                //Ensure calls to Windows Form Controls are from this application's thread
                Invoke(new StatusUpdateEventHandler(OnStatusUpdateEvent), new object[2] { source, e });
                return;
            }

            //MessageBox.Show(e.Status.ToString());

            switch (e.Status)
            {
                //Drawer is closed.
                case CashDrawer.StatusClosed:
                    Cursor = Cursors.Default;
                    MessageBox.Show("Drawer closed");
                    //btn_openDrawer.Enabled = true;
                    break;
                //Drawer is opened.
                case CashDrawer.StatusOpen:
                    Cursor = Cursors.WaitCursor;
                    MessageBox.Show("Drawer opened");
                    //btn_openDrawer.Enabled = false;
                    break;
                case PosPrinter.StatusCoverOpen:
                    MessageBox.Show("Printer cover is opened.");
                    //MessageBox.Show("Cover open");
                    //m_btnStateByCover = false;
                    break;
                //No receipt paper.
                case PosPrinter.StatusReceiptEmpty:
                    MessageBox.Show("Printer paper is empty");
                    //MessageBox.Show("Paper empty... Closing pos");
                    //if (POSLoaded) ForcePOSLogOff();
                    //m_btnStateByPaper = false;
                    break;
                //'Printer cover is close.
                case PosPrinter.StatusCoverOK:
                    //setMessage("Printer cover is ok.", MessageType.Error);
                    //MessageBox.Show("Cover Okay");
                    //m_btnStateByCover = true;
                    break;
                //'Receipt paper is ok.
                case PosPrinter.StatusReceiptPaperOK:
                case PosPrinter.StatusReceiptNearEmpty:
                    //MessageBox.Show("Paper okay");
                    //m_btnStateByPaper = true;
                    break;
            }
        }

        protected void AddErrorEvent(object eventSource)
        {
            //<<<step10>>>--Start
            EventInfo errorEvent = eventSource.GetType().GetEvent("ErrorEvent");
            if (errorEvent != null)
            {
                errorEvent.AddEventHandler(eventSource,
                    new DeviceErrorEventHandler(OnErrorEvent));
            }
            //<<<step10>>>--End
        }

        protected void OnErrorEvent(object source, DeviceErrorEventArgs e)
        {
            if (InvokeRequired)
            {
                //Ensure calls to Windows Form Controls are from this application's thread
                Invoke(new DeviceErrorEventHandler(OnErrorEvent), new object[2] { source, e });
                return;
            }
            MessageBox.Show("Error event called");
        }


        private void ReadConfig()
        {
            try
            {
                var parser = new FileIniDataParser();
                appConfig = parser.ReadFile("config.ini");
            }
            catch
            {
                MessageBox.Show("Unable to read Configuration File");
                ReportsForm.ActiveForm.Close();
            }
        }

        public void restartPos()
        {
            Application.Restart();
            Application.Exit();
            //UpdatedWindow.ActiveForm.Close();
            Environment.Exit(0);
        }

        private bool ReleasePrinter()
        {
            if (!m_Printer.Claimed)
            {
                return true;
            }
            Console.WriteLine("Start Releasing");
            try
            {
                m_Printer.DeviceEnabled = false;
                m_Printer.Release();
                //orNumber = "";
                Task.Delay(100);
                return true;

            }
            catch (PosControlException f)
            {
                MessageBox.Show(f.ToString());
                return false;
            }
        }

        protected void RemoveStatusUpdateEvent(object eventSource)
        {

            EventInfo statusUpdateEvent = eventSource.GetType().GetEvent("StatusUpdateEvent");
            if (statusUpdateEvent != null)
            {
                statusUpdateEvent.RemoveEventHandler(eventSource,
                    new StatusUpdateEventHandler(OnStatusUpdateEvent));
            }

        }

        private void btn_close_cdraw_Click(object sender, EventArgs e)
        {
            if (activeCashier.clsCDraw != 1)
            {
                MessageBox.Show("User not allowed to close cashdraw");
                return;
             //   setMessage("User not allowed to close cashdraw", MessageType.Error);

            }
            try
            {
                if (!CloseDayStarted)
                {
                    var result = MessageBox.Show("Do you really want to close cash draw?", "Close Cash Draw", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (result != DialogResult.Yes)
                    {
                        return;
                    }
                }

                MessageBox.Show("Generating Cash Draw Report.");
                //setMessage("Generating Cash Draw Report.", MessageType.Info);

                var respo = api.CloseCashDraw(activeCashier.id);
                if (respo == "false")
                {
                    MessageBox.Show("Close cashdraw failed");
                    return;
                }
                ProcessCashDraw(respo);
                enb.LogPOSEvent($"Cashier Close Cashdraw:{activeCashier.name}", 14, loggedInPOSID, activeCashier.name);

                
                if (appConfig["APP"]["OverrideLogoffSOP"] == "false")
                {
                    if (m_Printer.Claimed == true)
                    {
                        ReleasePrinter();
                    }

                    while (m_Printer.Claimed)
                    {
                        System.Threading.Thread.Sleep(100);
                    }

                   
                }
                else
                {
                  //  MessageBox.Show("POS is closing after 1 seconds.");
                  //  setMessage("POS is closing after 1 seconds.", MessageType.Info);
                    //System.Threading.Thread.Sleep(1000);
                    //    Session.Logoff();
                    //ActiveForm.Close();
                    //Application.Restart();
                    //Application.Exit();
                    //Environment.Exit(0);
                   // restartPos();


                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.Message, "Error Closing Cashdraw");
            }
        }

        public void ProcessCashDraw(string data, bool isReprint = false)
        {
            var details = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

            string finHistory = JsonConvert.SerializeObject(details["cdrawFinHistory"]);
            string gradeHistory = JsonConvert.SerializeObject(details["cdrawGradeHist"]);
            string deptHistory = JsonConvert.SerializeObject(details["cdrawDeptHist"]);
            string cdrawdetails = JsonConvert.SerializeObject(details["cdrawDetails"]);
            string manualHistory = JsonConvert.SerializeObject(details["manualCdrawGradeHist"]);

            decimal amount = decimal.Parse(details["Amount"].ToString());

            Console.WriteLine(finHistory);
            Console.WriteLine(gradeHistory);
            Console.WriteLine(deptHistory);
            Console.WriteLine(cdrawdetails);
            Console.WriteLine(manualHistory);

            List<CashDraw.CDrawFinHistory> finHis = new List<CashDraw.CDrawFinHistory>();
            finHis = JsonConvert.DeserializeObject<List<CashDraw.CDrawFinHistory>>(finHistory);

            List<CashDraw.CDrawGradeHistory> grdHis = new List<CashDraw.CDrawGradeHistory>();
            grdHis = JsonConvert.DeserializeObject<List<CashDraw.CDrawGradeHistory>>(gradeHistory);

            List<CashDraw.CDrawDeptHistory> deptHis = new List<CashDraw.CDrawDeptHistory>();
            deptHis = JsonConvert.DeserializeObject<List<CashDraw.CDrawDeptHistory>>(deptHistory);

            CashDraw.CDrawDetails cdetails = new CashDraw.CDrawDetails();
            cdetails = JsonConvert.DeserializeObject<CashDraw.CDrawDetails>(cdrawdetails);

            List<CashDraw.CDrawManualGradeHistory> mgHis = new List<CashDraw.CDrawManualGradeHistory>();
            mgHis = JsonConvert.DeserializeObject<List<CashDraw.CDrawManualGradeHistory>>(manualHistory);

            PrepareCashDraw(finHis, grdHis, deptHis, cdetails, mgHis, isReprint, amount);
        }

        private void Headers(out string[] h1, out string[] h2, out string[] h3, out string[] h4, out string[] h5)
        {
            h1 = Regex.Split(rct.headerL1.Trim(), @"\\n");
            h2 = Regex.Split(rct.headerL2.Trim(), @"\\n");
            h3 = Regex.Split(rct.headerL3.Trim(), @"\\n");
            h4 = Regex.Split(rct.headerL4.Trim(), @"\\n");
            h5 = Regex.Split(rct.headerL5.Trim(), @"\\n");
        }

        public void Footers(out string[] f1, out string[] f2, out string[] f3, out string[] f4, out string[] f5)
        {
            f1 = Regex.Split(rct.footerL1.Trim(), @"\\n");
            f2 = Regex.Split(rct.footerL2.Trim(), @"\\n");
            f3 = Regex.Split(rct.footerL3.Trim(), @"\\n");
            f4 = Regex.Split(rct.footerL4.Trim(), @"\\n");
            f5 = Regex.Split(rct.footerL5.Trim(), @"\\n");
        }

        public String CenteredText(String str)
        {
            int x = lineChars;
            int rem = x - str.Length;
            int sp = rem / 2;
            String tab = "";
            try
            {
                for (int i = 0; i <= sp; i++)
                {
                    tab += " ";
                }
            }
            catch { }

            return tab + str;
        }

        public String ThreeColumnLine(String first, String second, String third)
        {
            int spaces = 0;
            String tab = "";
            String line = "";

            spaces = 26 - (first.Length + second.Length);

            for (int i = 0; i < spaces; i++)
            {
                tab += " ";
            }
            line = first + tab + second;

            spaces = 42 - (line.Length + third.Length);
            tab = "";
            for (int i = 0; i < spaces; i++)
            {
                tab += " ";
            }

            return line + tab + third;
        }

        public String MakePrintString(int iLineChars, String first, String second)
        {
            int iSpaces = 0;
            String tab = "";
            try
            {
                //dynamically compute for the number of space between the two text
                //spaces is the difference between the number of character supported by the printer
                //and the sum of the length of first and second text
                iSpaces = iLineChars - (first.Length + second.Length);
                for (int j = 0; j < iSpaces; j++)
                {
                    tab += " ";
                }
            }
            catch (Exception)
            {
            }
            return first + tab + second;
        }

        private void SaveToPrintLog(string data)
        {
            var client = new RestClient(api.url + "/VJournalCtrl/savePrintedFromPOS");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("posID", loggedInPOSID);
            request.AddParameter("printedData", data.Replace('=', '-'));

            IRestResponse response = client.Execute(request);

        }

        private void savePrinterStatusLog(string message = "")
        {
            string logPath = "logs\\PrinterStatusLog.txt";

            if (!File.Exists(logPath))
            {
                File.Create(logPath);
            }

            DateTime currentDate = DateTime.Now;

            using (StreamWriter sw = System.IO.File.AppendText(logPath))
            {
                string status = (message != "") ? message : m_Printer.Claimed.ToString();
                string connectionChecker = (message != "") ? "Other" : "Printer Connected";
                sw.WriteLine($"{currentDate}, {connectionChecker}: {status}");
            }

        }

        private long GetRecLineChars(ref int[] RecLineChars)
        {
            long lRecLineChars = 0;
            long lCount;
            int i;

            // Calculate the element count.
            lCount = m_Printer.RecLineCharsList.GetLength(0);

            if (lCount == 0)
            {
                lRecLineChars = 0;
            }
            else
            {
                if (lCount > MAX_LINE_WIDTHS)
                {
                    lCount = MAX_LINE_WIDTHS;
                }

                for (i = 0; i < lCount; i++)
                {
                    RecLineChars[i] = m_Printer.RecLineCharsList[i];
                }

                lRecLineChars = lCount;
            }

            return lRecLineChars;
        }

        private string GetErrorCode(PosControlException ex)
        {
            string strErrorCodeEx = "";
            switch (ex.ErrorCodeExtended)
            {
                case PosPrinter.ExtendedErrorCoverOpen:
                case PosPrinter.ExtendedErrorJournalEmpty:
                case PosPrinter.ExtendedErrorReceiptEmpty:
                case PosPrinter.ExtendedErrorSlipEmpty:
                    strErrorCodeEx = ex.Message;
                    break;
                default:
                    string strEC = ex.ErrorCode.ToString();
                    string strECE = ex.ErrorCodeExtended.ToString();
                    strErrorCodeEx = "ErrorCode =" + strEC + "\nErrorCodeExtended =" + strECE + "\n"
                        + ex.Message;
                    break;
            }
            return strErrorCodeEx;
        }

        public async void PrepareCashDraw(List<CashDraw.CDrawFinHistory> finHis, List<CashDraw.CDrawGradeHistory> gradeHis, List<CashDraw.CDrawDeptHistory> deptHis, CashDraw.CDrawDetails dts, List<CashDraw.CDrawManualGradeHistory> mgHis, bool isReprint, decimal amount)
        {
            List<string> rl = new List<string>();

            DateTime nowDate = DateTime.Now;                            //System date
            DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();   //Date Format

            dateFormat.MonthDayPattern = "mm";
            string strDate = nowDate.ToString("MM-dd-yyyy  hh:mm tt", dateFormat);

            string strPrintData = "";

            Headers(out string[] h1, out string[] h2, out string[] h3, out string[] h4, out string[] h5);

            foreach (string ln in h1)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h2)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h3)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h4)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h5)
                rl.Add(CenteredText(ln.Trim()));

            rl.Add("");

            if (isReprint)
            {
                rl.Add(CenteredText("*** Reprint ***"));
                rl.Add(CenteredText($"Reprint Date:{strDate}"));
            }

            //End HEADER----------------------------------------------------------
            rl.Add(CenteredText("CASH DRAW REPORT"));
            //rl.Add(CenteredText("X-Reading"));
            rl.Add("");

            rl.Add(CenteredText("Open: " + dts.CDraw_Open_Date));
            rl.Add(CenteredText("Close: " + dts.CDraw_Close_Date));
            rl.Add(CenteredText("Closed by: " + dts.Cashier_Name));
            rl.Add("");

            rl.Add(CenteredText(strDate + "  " + activeCashier.name));
            rl.Add("");

            //Start CashDRAW------
            strPrintData = ThreeColumnLine("", "Qty", "Value");
            rl.Add(strPrintData);
            rl.Add("");

            decimal finTotal;
            decimal granTotal = 0;

            for (int i = 0; i < finHis.Count; i++)
            {
                Console.WriteLine(finHis[i].CDraw_Tot_Amount == 0);
                Console.WriteLine(finHis[i].CDraw_Tot_CashOut == 0);
                Console.WriteLine(finHis[i].CDraw_Tot_Payin == 0);
                Console.WriteLine(finHis[i].CDraw_Tot_Safedrop == 0);

                if (finHis[i].CDraw_Tot_Amount == 0 && finHis[i].CDraw_Tot_CashOut == 0 && finHis[i].CDraw_Tot_Payin == 0 && finHis[i].CDraw_Tot_Safedrop == 0)
                {
                    continue;
                }
                rl.Add(finHis[i].MOP_Name.Trim());

                strPrintData = ThreeColumnLine("   Sales", "", (finHis[i].CDraw_Tot_Amount + finHis[i].CDraw_Tot_Safedrop + finHis[i].CDraw_Tot_Refund).ToString("N", new CultureInfo("en-US")));
                rl.Add(strPrintData);

                if (finHis[i].MOP_Name.ToUpper().Trim() == "CASH")
                {
                    strPrintData = ThreeColumnLine(" - Safedrops", finHis[i].CDraw_Num_Safedrop.ToString(), finHis[i].CDraw_Tot_Safedrop.ToString("N", new CultureInfo("en-US")));
                    rl.Add(strPrintData);

                    finTotal = finHis[i].CDraw_Tot_Amount + finHis[i].CDraw_Tot_Refund; // - finHis[i].CDraw_Tot_Safedrop;

                    strPrintData = ThreeColumnLine("    TOTAL", "", finTotal.ToString("N", new CultureInfo("en-US")));
                    rl.Add(strPrintData);

                    granTotal += finTotal;
                }
                else if (finHis[i].MOP_Name.ToUpper().Trim() == "INHOUSE")
                {
                    strPrintData = ThreeColumnLine(" + Acc Dis/Chg-Cr", finHis[i].CDraw_Num_CashOut.ToString(), (finHis[i].CDraw_Tot_CashOut).ToString("N", new CultureInfo("en-US")));
                    rl.Add(strPrintData);

                    finTotal = finHis[i].CDraw_Tot_Amount - finHis[i].CDraw_Tot_CashOut + finHis[i].CDraw_Tot_Refund;

                    strPrintData = ThreeColumnLine("    TOTAL", "", finTotal.ToString("N", new CultureInfo("en-US")));
                    rl.Add(strPrintData);

                    granTotal += finTotal;
                }
                else
                {
                    strPrintData = ThreeColumnLine("    TOTAL", "", (finHis[i].CDraw_Tot_Amount + finHis[i].CDraw_Tot_Refund).ToString("N", new CultureInfo("en-US")));
                    rl.Add(strPrintData);

                    granTotal += (finHis[i].CDraw_Tot_Amount + finHis[i].CDraw_Tot_Refund);
                }

                rl.Add("");
            }

            rl.Add("");

            strPrintData = MakePrintString(lineChars, "    GRAND TOTAL", granTotal.ToString("N", new CultureInfo("en-US")));
            rl.Add(strPrintData);
            rl.Add("");

            //END CashDraw-----

            //START DEPARTMENT SALES
            rl.Add(CenteredText("- GROSS DEPARTMENT SALES -"));
            rl.Add("");

            strPrintData = ThreeColumnLine("Department", "Quantity", "Value");
            rl.Add(strPrintData);
            rl.Add("");

            //>>NON FUEL SALES

            for (int i = 0; i < deptHis.Count; i++)
            {
                if (deptHis[i].Dept_Name.ToUpper().Trim() == "PRODUCTS")
                {
                    strPrintData = ThreeColumnLine(deptHis[i].Dept_Name, deptHis[i].CDrawDept_Qty_Sld.ToString(), (deptHis[i].CDrawDept_Val_Sld + amount).ToString("N", new CultureInfo("en-US")));
                    rl.Add(strPrintData);
                    //if (deptHis[i].CDrawDept_Qty_Ref > 0)
                    //{
                    //    strPrintData = ThreeColumnLine(" Refunds", deptHis[i].CDrawDept_Qty_Ref.ToString(), deptHis[i].CDrawDept_Val_Ref.ToString("N", new CultureInfo("en-US")));
                    //    rl.Add(strPrintData);
                    //}
                    rl.Add("");
                    strPrintData = ThreeColumnLine("TOTAL", (deptHis[i].CDrawDept_Qty_Sld).ToString(), (deptHis[i].CDrawDept_Val_Sld + amount).ToString("N", new CultureInfo("en-US")));
                    rl.Add(strPrintData);
                    rl.Add("");
                }
            }

            //>>----------------
            //START DEPARTMENT REFUNDS
            rl.Add(CenteredText("- DEPARTMENT DISCOUNTS -"));
            rl.Add("");

            strPrintData = ThreeColumnLine("Department", "Quantity", "Value");
            rl.Add(strPrintData);
            rl.Add("");

            //>>NON FUEL SALES

            for (int i = 0; i < deptHis.Count; i++)
            {
                if (deptHis[i].Dept_Name.ToUpper().Trim() == "PRODUCTS")
                {

                    strPrintData = ThreeColumnLine(deptHis[i].Dept_Name, deptHis[i].CDrawDept_Qty_Disc.ToString(), deptHis[i].CDrawDept_Val_Disc.ToString("N", new CultureInfo("en-US")));
                    rl.Add(strPrintData);
                    rl.Add("");
                    strPrintData = ThreeColumnLine("TOTAL", deptHis[i].CDrawDept_Qty_Disc.ToString(), deptHis[i].CDrawDept_Val_Disc.ToString("N", new CultureInfo("en-US")));
                    rl.Add(strPrintData);
                    rl.Add("");
                }
            }

            //>>----------------

            //START DEPARTMENT REFUNDS
            rl.Add(CenteredText("- DEPARTMENT REFUNDS -"));
            rl.Add("");

            strPrintData = ThreeColumnLine("Department", "Quantity", "Value");
            rl.Add(strPrintData);
            rl.Add("");

            //>>NON FUEL SALES

            for (int i = 0; i < deptHis.Count; i++)
            {
                if (deptHis[i].Dept_Name.ToUpper().Trim() == "PRODUCTS")
                {

                    strPrintData = ThreeColumnLine(deptHis[i].Dept_Name, deptHis[i].CDrawDept_Qty_Ref.ToString(), deptHis[i].CDrawDept_Val_Ref.ToString("N", new CultureInfo("en-US")));
                    rl.Add(strPrintData);
                    rl.Add("");
                    strPrintData = ThreeColumnLine("TOTAL", deptHis[i].CDrawDept_Qty_Ref.ToString(), deptHis[i].CDrawDept_Val_Ref.ToString("N", new CultureInfo("en-US")));
                    rl.Add(strPrintData);
                    rl.Add("");
                }
            }

            //>>----------------
            //#region commented codes
            ////FUELS SALES
            //rl.Add(CenteredText("- FUEL SALES BY GRADE -"));

            //strPrintData = FourColumnLine("Grade", "Qty", "Volume", "Value");
            //rl.Add(strPrintData);
            //rl.Add("");

            //decimal totQty = 0;
            //decimal totVol = 0;
            //decimal totVal = 0;

            //for (int i = 0; i < gradeHis.Count; i++)
            //{
            //    if (gradeHis[i].CDrawGrade_Trs == 0 && gradeHis[i].CDrawGrade_Vol == 0 && gradeHis[i].CDrawGrade_Val == 0) continue;

            //    strPrintData = FourColumnLine(gradeHis[i].Grade_Name, gradeHis[i].CDrawGrade_Trs.ToString("F2"), gradeHis[i].CDrawGrade_Vol.ToString("F2"), gradeHis[i].CDrawGrade_Val.ToString("N", new CultureInfo("en-US")));
            //    rl.Add(strPrintData);

            //    totQty += gradeHis[i].CDrawGrade_Trs;
            //    totVol += gradeHis[i].CDrawGrade_Vol;
            //    totVal += gradeHis[i].CDrawGrade_Val;
            //}
            //strPrintData = FourColumnLine("TOTAL", totQty.ToString(), totVol.ToString(), totVal.ToString("N", new CultureInfo("en-US")));
            //rl.Add(strPrintData);
            //rl.Add("");

            ////END FUEL SALES------------------------------------------------------------------------------------
            //rl.Add(CenteredText("- FUEL DISCOUNTS -"));
            //rl.Add("");

            //strPrintData = ThreeColumnLine("Grade", "Volume", "Value");
            //rl.Add(strPrintData);
            //rl.Add("");

            //decimal FuelDiscVol = 0;
            //decimal FuelDiscVal = 0;
            //for (int i = 0; i < gradeHis.Count; i++)
            //{
            //    if (gradeHis[i].CDrawGrade_Vol_Disc == 0 && gradeHis[i].CDrawGrade_Val_Disc == 0) continue;

            //    strPrintData = ThreeColumnLine(gradeHis[i].Grade_Name, gradeHis[i].CDrawGrade_Vol_Disc.ToString("F2"), gradeHis[i].CDrawGrade_Val_Disc.ToString("c"));
            //    rl.Add(strPrintData);

            //    FuelDiscVol += gradeHis[i].CDrawGrade_Vol_Disc;
            //    FuelDiscVal += gradeHis[i].CDrawGrade_Val_Disc;
            //}
            //strPrintData = ThreeColumnLine("TOTAL", FuelDiscVol.ToString("F2"), FuelDiscVal.ToString("c"));
            //rl.Add(strPrintData);
            //rl.Add("");


            //rl.Add(CenteredText("- FUEL REFUNDS -"));
            //rl.Add("");

            //strPrintData = ThreeColumnLine("Grade", "Volume", "Value");
            //rl.Add(strPrintData);
            //rl.Add("");

            //decimal FuelRefVol = 0;
            //decimal FuelRefVal = 0;
            //for (int i = 0; i < gradeHis.Count; i++)
            //{
            //    if (gradeHis[i].CDrawGrade_Vol_Ref == 0 && gradeHis[i].CDrawGrade_Val_Ref == 0) continue;

            //    strPrintData = ThreeColumnLine(gradeHis[i].Grade_Name, gradeHis[i].CDrawGrade_Vol_Ref.ToString("F2"), gradeHis[i].CDrawGrade_Val_Ref.ToString("c"));
            //    rl.Add(strPrintData);

            //    FuelRefVol += gradeHis[i].CDrawGrade_Vol_Ref;
            //    FuelRefVal += gradeHis[i].CDrawGrade_Val_Ref;
            //}
            //strPrintData = ThreeColumnLine("TOTAL", FuelRefVol.ToString("F2"), FuelRefVal.ToString("c"));
            //rl.Add(strPrintData);
            //rl.Add("");
            ////Manual Fuel Sales --------------------------------------------------------------------------------
            //rl.Add(CenteredText("- MANUAL SALES BY GRADE -"));

            //strPrintData = FourColumnLine("Grade", "Qty", "Volume", "Value");
            //rl.Add(strPrintData);
            //rl.Add("");

            //int mnQty = 0;
            //decimal mnVal = 0;
            //decimal mnVol = 0;

            //for (int i = 0; i < mgHis.Count; i++)
            //{
            //    if (mgHis[i].CDrawGrade_Trs == 0 && mgHis[i].CDrawGrade_Val == 0 && mgHis[i].CDrawGrade_Vol == 0) continue;

            //    strPrintData = FourColumnLine(mgHis[i].Grade_Name, mgHis[i].CDrawGrade_Trs.ToString(), mgHis[i].CDrawGrade_Vol.ToString("F2"), mgHis[i].CDrawGrade_Val.ToString("N", new CultureInfo("en-US")));
            //    rl.Add(strPrintData);

            //    mnQty += mgHis[i].CDrawGrade_Trs;
            //    mnVal += mgHis[i].CDrawGrade_Val;
            //    mnVol += mgHis[i].CDrawGrade_Vol;
            //}

            //strPrintData = FourColumnLine("TOTAL", mnQty.ToString(), mnVol.ToString(), mnVal.ToString("N", new CultureInfo("en-US")));
            //rl.Add(strPrintData);

            //rl.Add("");
            //rl.Add(CenteredText("- MANUAL FUEL DISCOUNTS -"));
            //rl.Add("");

            //strPrintData = ThreeColumnLine("Grade", "Volume", "Value");
            //rl.Add(strPrintData);
            //rl.Add("");

            //decimal ManFuelDiscVol = 0;
            //decimal ManFuelDiscVal = 0;
            //for (int i = 0; i < mgHis.Count; i++)
            //{
            //    if (mgHis[i].CDrawGrade_Vol_Disc == 0 && mgHis[i].CDrawGrade_Val_Disc == 0) continue;

            //    strPrintData = ThreeColumnLine(mgHis[i].Grade_Name, mgHis[i].CDrawGrade_Vol_Disc.ToString("F2"), mgHis[i].CDrawGrade_Val_Disc.ToString("c"));
            //    rl.Add(strPrintData);

            //    ManFuelDiscVol += mgHis[i].CDrawGrade_Vol_Disc;
            //    ManFuelDiscVal += mgHis[i].CDrawGrade_Val_Disc;
            //}
            //strPrintData = ThreeColumnLine("TOTAL", ManFuelDiscVol.ToString("F2"), ManFuelDiscVal.ToString("c"));
            //rl.Add(strPrintData);
            //rl.Add("");


            //rl.Add(CenteredText("- MANUAL FUEL REFUNDS -"));
            //rl.Add("");

            //strPrintData = ThreeColumnLine("Grade", "Volume", "Value");
            //rl.Add(strPrintData);
            //rl.Add("");

            //decimal ManFuelRefVol = 0;
            //decimal ManFuelRefVal = 0;
            //for (int i = 0; i < mgHis.Count; i++)
            //{
            //    if (mgHis[i].CDrawGrade_Vol_Ref == 0 && mgHis[i].CDrawGrade_Val_Ref == 0) continue;

            //    strPrintData = ThreeColumnLine(mgHis[i].Grade_Name, mgHis[i].CDrawGrade_Vol_Ref.ToString("F2"), mgHis[i].CDrawGrade_Val_Ref.ToString("c"));
            //    rl.Add(strPrintData);

            //    ManFuelRefVol += mgHis[i].CDrawGrade_Vol_Ref;
            //    ManFuelRefVal += mgHis[i].CDrawGrade_Val_Ref;
            //}
            //strPrintData = ThreeColumnLine("TOTAL", ManFuelRefVol.ToString("F2"), ManFuelRefVal.ToString("c"));
            //rl.Add(strPrintData);
            //rl.Add("");
            ////#endregion
            //END DEPARTMENT SALES
            rl.Add("");

            Footers(out string[] f1, out string[] f2, out string[] f3, out string[] f4, out string[] f5);

            foreach (string ln in f2)
                rl.Add(CenteredText(ln.Trim()));

            rl.Add("");

            foreach (string ln in f3)
                rl.Add(CenteredText(ln.Trim()));
            //if (activeSale.zeroRated == true)
            //{
            rl.Add("");
            foreach (string ln in f4)
            {
                /*if (!isReprint)
                {
                    continue;
                }*/
                string[] xb = ln.Split(new char[] { '\n' });
                foreach (string lnb in xb)
                {
                    rl.Add(CenteredText(lnb.Trim()));
                }

            }

            rl.Add("");




            string dataToPrintLog = "";
            foreach (string line in rl)
            {
                dataToPrintLog += $"{line}\n";
            }
            SaveToPrintLog(dataToPrintLog);

            string open = dts.CDraw_Open_Date;
            string[] x = open.Split(' ');
            string[] xx = x[1].Split('.');
            xx[0] = Regex.Replace(xx[0], @":", "");
            xx[0] = xx[0].Remove(4, 2);
            open = x[0] + " " + xx[0];

            string close = dts.CDraw_Close_Date;

            if (close == null)
            {
                close = "";
            }
            else
            {
                string[] y = close.Split(' ');
                string[] yy = y[1].Split('.');
                yy[0] = Regex.Replace(yy[0], @":", "");
                yy[0] = yy[0].Remove(4, 2);
                close = y[0] + " " + yy[0];
            }


            string path = "reports\\Cash Draw Report" + " " + open + " to " + close + ".txt";
            fileIO.CreateTextFile(path, rl);

            await PrintText(rl);
            if (appConfig["APP"]["AutoOpenTextFiles"] == "true")
                Process.Start(path);
        }

        public async Task PrintText(List<string> lines)
        {

            /*string strCmdText = "print";
            foreach(string receiptLine in lines)
            {
                strCmdText += $"{receiptLine}\n";
            }

            strCmdText += "> \\\\127.0.0.1\\B400";
            
            //Console.WriteLine(st)
            System.Diagnostics.Process.Start("CMD.exe", strCmdText);*/

            if (appConfig["APP"]["Print"] != "true")
            {
                //MessageBox.Show("No Printing", "Dev Mode", MessageBoxButtons.OK);
                return;
            }

            var res = m_Printer.Claimed;
            if (!res)
            {
                if (!ClaimPrinter())
                    //MessageBox.Show("Error claiming printer");
                    return;
            }

            bool receiptIsRefundReprint = lines.Contains("Refund") && lines.Contains("Reprint");


            savePrinterStatusLog();

            m_Printer.AsyncMode = true;

            if (m_Printer.CapRecPresent)
            {
                savePrinterStatusLog("CapRecPresent");

                try
                {

                    //lineChars = 42;

                    //m_Printer.RecLineHeight = 9;
                    //m_Printer.RecLineSpacing = 9;
                    //Batch processing mode
                    /*m_Printer.TransactionPrint(PrinterStation.Receipt
                        , PrinterTransactionControl.Transaction);*/

                    //try to print smaller font size
                    if (appConfig["PRINTER"]["printsize"] == "small")
                    {
                        int[] RecLineChars = new int[MAX_LINE_WIDTHS] { 0, 0 };
                        long lRecLineCharsCount;


                        lRecLineCharsCount = GetRecLineChars(ref RecLineChars);
                        if (lRecLineCharsCount >= 2)
                        {
                            m_Printer.RecLineChars = RecLineChars[1];
                            //56
                            m_Printer.RecLineSpacing = 300;
                        }
                        else
                        {
                            m_Printer.RecLineChars = RecLineChars[0];
                        }
                    }
                    else
                    {
                        if (appConfig["PRINTER"]["printertype"] == "dotmatrix")
                        {
                            int[] RecLineChars = new int[MAX_LINE_WIDTHS] { 0, 0 };
                            long lRecLineCharsCount;

                            Console.WriteLine("RECLINECHARS 1 {0}", RecLineChars[1]);
                            Console.WriteLine("RECLINECHARS 0 {0}", RecLineChars[0]);

                            lRecLineCharsCount = GetRecLineChars(ref RecLineChars);
                            if (lRecLineCharsCount >= 2)
                            {
                                m_Printer.RecLineChars = RecLineChars[1];
                                //56
                                m_Printer.RecLineSpacing = 350;
                            }
                            else
                            {
                                m_Printer.RecLineChars = RecLineChars[0];
                            }
                        }
                        else
                        {
                            m_Printer.RecLineSpacing = 350;
                        }
                    }

                    Console.WriteLine("START LINE PRINTING");
                    foreach (string line in lines)
                    {
                        Console.WriteLine(line + " THIS ...");
                        //var z = line.TrimEnd(new char[] { '\\', 'n' });
                        var z = line.TrimEnd('\n');

                        string cutLine = new string('=', lineChars);


                        if (z == cutLine)
                        {
                            m_Printer.PrintImmediate(PrinterStation.Receipt, "\u001b|fP");
                            // m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|fP");
                            continue;
                        }

                        if (z == cutLine.Replace('=', '-') && receiptIsRefundReprint)
                        {
                            m_Printer.PrintImmediate(PrinterStation.Receipt, "\u001b|fP");
                            break;
                            // m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|fP");

                        }
                        //            m_Printer.PrintNormal(PrinterStation.Receipt, z + "\n");
                        m_Printer.PrintImmediate(PrinterStation.Receipt, z + "\n");
                        //m_Printer.PrintImmediate();
                    }
                    //m_Printer.PrintNormal(PrinterStation.Receipt, "Hello world");

                    //Make 5mm speces
                    //m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|200uF");

                    //Paper Cut
                    m_Printer.PrintImmediate(PrinterStation.Receipt, "\u001b|fP");
                    //          m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|fP");


                    //--------------------------------------------------------------
                    //print all the buffer data. and exit the batch processing mode.
                    /*    m_Printer.TransactionPrint(PrinterStation.Receipt
                            , PrinterTransactionControl.Normal);*/
                    //<<<step6>>>--End
                    Console.WriteLine("END PRINTING");
                    m_Printer.ClearOutput();
                }
                catch (PosControlException ex)
                {

                    if (ex.ErrorCode == ErrorCode.Illegal && ex.ErrorCodeExtended == 1004)
                    {
                        MessageBox.Show("Unable to print receipt.\n" + GetErrorCode(ex)
                            , "Printer Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // Clear the buffered data since the buffer retains print data when an error occurs during printing.
                        m_Printer.ClearOutput();
                        return;
                    }

                    try
                    {
                        // Clear the buffered data since the buffer retains print data when an error occurs during printing.
                        m_Printer.ClearOutput();
                    }
                    catch (PosControlException pes)
                    {
                        MessageBox.Show(pes.Message.ToString());
                    }

                    MessageBox.Show(ex.Message.ToString());
                    m_Printer.AsyncMode = false;
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("EXEPTION ONLY " + ex.Message.ToString());
                }
            }

            m_Printer.AsyncMode = false;
        }

        private void btn_close_shift_Click(object sender, EventArgs e)
        {
            if (activeCashier.clsShift != 1)
            {
                MessageBox.Show("User not allowed to close shift.");
                return;
                //setMessage("User not allowed to close shift.", MessageType.Error);
                //return;
            }

            if (!CloseDayStarted)
            {
                var result = MessageBox.Show("Do you really want to close shift?", "Close Shift", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result != DialogResult.Yes)
                    return;
            }

            MessageBox.Show("Generating Shift Report");

            var respo = api.closePeriod(1);
            if (respo == "false")
            {
                MessageBox.Show("Close shift failed");
                return;
            }
            Console.WriteLine(respo);
            ProcessReports(respo, "SHIFT REPORT", 0);

            MessageBox.Show("Close shift successful", "Shift Closed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ProcessReports(string data, string title, int type, bool reprint = false)
        {
            Console.WriteLine("***********************************************");
            Console.WriteLine(data);

            var details = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
            string depSales = JsonConvert.SerializeObject(details["depSales"]);
            string fuelSales = JsonConvert.SerializeObject(details["fuelSales"]);
            string accDiscSales = JsonConvert.SerializeObject(details["accDiscSales"]);
            string finHistory = JsonConvert.SerializeObject(details["finHistory"]);
            string cashiersTotal = JsonConvert.SerializeObject(details["cashiersTotal"]);
            string periodDetails = JsonConvert.SerializeObject(details["periodDetails"]);
            //string invoiceNumbers = JsonConvert.SerializeObject(details["invoiceNumbers"]);
            string manualSales = JsonConvert.SerializeObject(details["manualFuelSales"]);
            string depTaxEx = JsonConvert.SerializeObject(details["deptTaxExempt"]);
            string delTaxEx = JsonConvert.SerializeObject(details["deliveryTaxExempt"]);
            string taxHis = JsonConvert.SerializeObject(details["taxHistory"]);
            string depDisct = JsonConvert.SerializeObject(details["deptDiscounts"]);
            string mfsDisct = JsonConvert.SerializeObject(details["manualFuelSalesDisc"]);
            string fsDisct = JsonConvert.SerializeObject(details["fuelSalesDisc"]);
            string discountTotal = JsonConvert.SerializeObject(details["discountTotal"]);
            string depRefunds = JsonConvert.SerializeObject(details["depRefund"]);
            string transHourData = JsonConvert.SerializeObject(details["transHourData"]);
            string hoseHistData = JsonConvert.SerializeObject(details["hoseHistData"]);
            string transNumbers = JsonConvert.SerializeObject(details["transNums"]);
            string vehicleTotals = JsonConvert.SerializeObject(details["vehicleTotals"]);
            string tankHistory = JsonConvert.SerializeObject(details["tankHistData"]);
            string delRefunds = JsonConvert.SerializeObject(details["deliveryRefunds"]);
            string manualRefunds = JsonConvert.SerializeObject(details["manualRefunds"]);
            string manualTaxEx = JsonConvert.SerializeObject(details["manualTaxExempt"]);

            string eodRep = JsonConvert.SerializeObject(details["EoD_History"]);
            string eodTax = JsonConvert.SerializeObject(details["EoD_TaxHist"]);
            string eodDisc = JsonConvert.SerializeObject(details["EoD_DiscountHist"]);

            string serviceChargeValues = JsonConvert.SerializeObject(details["serviceChargeAmount"]);

            List<Reports.EodDiscHist> eodDiscHist = new List<Reports.EodDiscHist>();
            eodDiscHist = JsonConvert.DeserializeObject<List<Reports.EodDiscHist>>(eodDisc);

            List<Reports.EodTaxHist> eodTaxHist = new List<Reports.EodTaxHist>();
            eodTaxHist = JsonConvert.DeserializeObject<List<Reports.EodTaxHist>>(eodTax);

            List<Reports.EodReport> eodRpt = new List<Reports.EodReport>();
            eodRpt = JsonConvert.DeserializeObject<List<Reports.EodReport>>(eodRep);

            List<Reports.DeptSales> dSales = new List<Reports.DeptSales>();
            dSales = JsonConvert.DeserializeObject<List<Reports.DeptSales>>(depSales);

            List<Reports.FuelSales> fSales = new List<Reports.FuelSales>();
            fSales = JsonConvert.DeserializeObject<List<Reports.FuelSales>>(fuelSales);

            List<Reports.CashiersTotal> cTotal = new List<Reports.CashiersTotal>();
            cTotal = JsonConvert.DeserializeObject<List<Reports.CashiersTotal>>(cashiersTotal);

            Reports.PeriodDetails pDetails = new Reports.PeriodDetails();
            pDetails = JsonConvert.DeserializeObject<Reports.PeriodDetails>(periodDetails);

            //Reports.InvoiceNumbers iNum = new Reports.InvoiceNumbers();
            //iNum = JsonConvert.DeserializeObject<Reports.InvoiceNumbers>(invoiceNumbers);

            Reports.AccDiscSales acDisc = new Reports.AccDiscSales();
            acDisc = JsonConvert.DeserializeObject<Reports.AccDiscSales>(accDiscSales);

            List<Reports.FinHistory> fHis = new List<Reports.FinHistory>();
            fHis = JsonConvert.DeserializeObject<List<Reports.FinHistory>>(finHistory);

            List<Reports.ManualSales> mSales = new List<Reports.ManualSales>();
            mSales = JsonConvert.DeserializeObject<List<Reports.ManualSales>>(manualSales);

            List<Reports.DeptTaxExempt> dpTxEx = new List<Reports.DeptTaxExempt>();
            dpTxEx = JsonConvert.DeserializeObject<List<Reports.DeptTaxExempt>>(depTaxEx);

            List<Reports.DeliveryTaxExempt> dlTxEx = new List<Reports.DeliveryTaxExempt>();
            dlTxEx = JsonConvert.DeserializeObject<List<Reports.DeliveryTaxExempt>>(delTaxEx);

            List<Reports.TaxHistory> txHis = new List<Reports.TaxHistory>();
            txHis = JsonConvert.DeserializeObject<List<Reports.TaxHistory>>(taxHis);

            List<Reports.DeptDiscounts> depDisc = new List<Reports.DeptDiscounts>();
            depDisc = JsonConvert.DeserializeObject<List<Reports.DeptDiscounts>>(depDisct);

            List<Reports.ManualFuelDiscount> mfDisc = new List<Reports.ManualFuelDiscount>();
            mfDisc = JsonConvert.DeserializeObject<List<Reports.ManualFuelDiscount>>(mfsDisct);

            List<Reports.FuelDiscount> fsDisc = new List<Reports.FuelDiscount>();
            fsDisc = JsonConvert.DeserializeObject<List<Reports.FuelDiscount>>(fsDisct);

            List<Reports.DiscountTotal> discTotal = new List<Reports.DiscountTotal>();
            discTotal = JsonConvert.DeserializeObject<List<Reports.DiscountTotal>>(discountTotal);

            List<Reports.DepRefunds> dpRef = new List<Reports.DepRefunds>();
            dpRef = JsonConvert.DeserializeObject<List<Reports.DepRefunds>>(depRefunds);

            List<Reports.TransHourData> thData = new List<Reports.TransHourData>();
            thData = JsonConvert.DeserializeObject<List<Reports.TransHourData>>(transHourData);

            List<Reports.HoseHistData> hhData = new List<Reports.HoseHistData>();
            hhData = JsonConvert.DeserializeObject<List<Reports.HoseHistData>>(hoseHistData);

            List<Reports.TransactionNumbers> trNums = new List<Reports.TransactionNumbers>();
            try
            {
                trNums = JsonConvert.DeserializeObject<List<Reports.TransactionNumbers>>(transNumbers);
            }
            catch (Exception)
            {
                trNums = JsonConvert.DeserializeObject<List<Reports.TransactionNumbers>>("[]");
            }

            List<Reports.VehicleTotals> vhTotals = new List<Reports.VehicleTotals>();
            vhTotals = JsonConvert.DeserializeObject<List<Reports.VehicleTotals>>(vehicleTotals);

            List<Reports.TankHistory> tankHist = new List<Reports.TankHistory>();
            tankHist = JsonConvert.DeserializeObject<List<Reports.TankHistory>>(tankHistory);

            List<Reports.DeliveryRefund> delRef = new List<Reports.DeliveryRefund>();
            delRef = JsonConvert.DeserializeObject<List<Reports.DeliveryRefund>>(delRefunds);

            List<Reports.ManualRefund> manRef = new List<Reports.ManualRefund>();
            manRef = JsonConvert.DeserializeObject<List<Reports.ManualRefund>>(manualRefunds);

            List<Reports.ManualTaxExempt> manTaxEx = new List<Reports.ManualTaxExempt>();
            manTaxEx = JsonConvert.DeserializeObject<List<Reports.ManualTaxExempt>>(manualTaxEx);

            List<List<decimal>> serviceCharges = new List<List<decimal>>();
            serviceCharges = JsonConvert.DeserializeObject<List<List<decimal>>>(serviceChargeValues);

            bool incReadings = false;
            //if (type == 1)
            //{
            //    incReadings = (MessageBox.Show("Include pump readings in the printout?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) ? true : false;
            //}

            PrepareReports(title, type, dSales, fSales, acDisc, fHis, cTotal, pDetails, mSales, dpTxEx, dlTxEx, txHis, depDisc, mfDisc, fsDisc, discTotal, dpRef, thData, hhData, trNums, incReadings, vhTotals, tankHist, delRef, manRef, manTaxEx, reprint, eodRpt, eodTaxHist, eodDiscHist, serviceCharges);
        }

        public String FourColumnLine(String first, String second, String third, String fourth)
        {
            int spaces = 0;
            String tab = "";
            String line = "";

            try
            {
                spaces = 18 - (first.Length + second.Length);
            }
            catch
            {
                spaces = 18;
            }


            for (int i = 0; i < spaces; i++)
            {
                tab += " ";
            }
            line = first + tab + second;
            tab = "";

            try
            {
                spaces = 28 - (line.Length + third.Length);
            }
            catch
            {
                spaces = 28;
            }

            for (int i = 0; i < spaces; i++)
            {
                tab += " ";
            }
            line = line + tab + third;
            tab = "";

            try
            {
                spaces = 42 - (line.Length + fourth.Length);

            }
            catch
            {
                spaces = 42;
            }

            for (int i = 0; i < spaces; i++)
            {
                tab += " ";
            }

            return line + tab + fourth;
        }
        private string SIformatter(string resetter, string number, bool isRefund = false)
        {
            if (resetter == "0" && number == "0")
            {
                return "No Activity";
            }
            string middle = "-";
            int resetLength = 3 - resetter.Length;
            int numberLength = 9 - number.Length;
            string formatted = new string('0', resetLength) + resetter + middle + new string('0', numberLength) + number;

            return formatted;
        }

        public async void PrepareReports(string title, int type, List<Reports.DeptSales> dSales, List<Reports.FuelSales> fSales, Reports.AccDiscSales adSales, List<Reports.FinHistory> fHis,
         List<Reports.CashiersTotal> cTotal, Reports.PeriodDetails pd, List<Reports.ManualSales> mSales, List<Reports.DeptTaxExempt> depTxEx,
         List<Reports.DeliveryTaxExempt> delTxEx, List<Reports.TaxHistory> txHis, List<Reports.DeptDiscounts> depDisc, List<Reports.ManualFuelDiscount> mfDisc,
         List<Reports.FuelDiscount> fuelDisc, List<Reports.DiscountTotal> discTotal, List<Reports.DepRefunds> dpRefunds, List<Reports.TransHourData> thData,
         List<Reports.HoseHistData> hhData, List<Reports.TransactionNumbers> transNum, bool incReadings, List<Reports.VehicleTotals> vehicleTotals, List<Reports.TankHistory> tanks,
         List<Reports.DeliveryRefund> dlRefunds, List<Reports.ManualRefund> manualFuelDiscounts, List<Reports.ManualTaxExempt> manualTaxExempts, bool reprint, List<Reports.EodReport> eodRpt, List<Reports.EodTaxHist> eodTax, List<Reports.EodDiscHist> eodDisc, List<List<decimal>> serviceChargeAmount)
        {

            Console.WriteLine($" SERVICE CHARGE PRESENT AMOUNT IS {serviceChargeAmount[0]}");
            Console.WriteLine($" SERVICE CHARGE LAST AMOUNT IS {serviceChargeAmount[1]}");
            List<string> rl = new List<string>();

            List<EoD_Report> eoD_Reports = new List<EoD_Report>();

            DateTime nowDate = DateTime.Now;                            //System date
            DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();   //Date Format
            dateFormat.MonthDayPattern = "MM";
            string strDate = nowDate.ToString("MM/dd/yyyy hh:mm tt", dateFormat);

            string strDate2 = nowDate.ToString("MM-dd-yyyy", dateFormat);

            string strPrintData = "";

            string printZCounter = "";
            
            
            Headers(out string[] h1, out string[] h2, out string[] h3, out string[] h4, out string[] h5);

            foreach (string ln in h1)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h2)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h3)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h4)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h5)
                rl.Add(CenteredText(ln.Trim()));
            if (reprint)
            {
                rl.Add(CenteredText("*** REPRINT ***"));
                rl.Add(CenteredText("Reprint Date:" + strDate));

            }

            if (title == "SHIFT REPORT")
            {
                rl.Add("");
                rl.Add(CenteredText($"POS #{loggedInPOSID}"));
            }

            rl.Add("");

            if (title == "DAY REPORT" && type == 1)
            {

                rl.Add(CenteredText("END OF DAY REPORT"));

                rl.Add(CenteredText("Z-Reading"));
            }
            else
            {
                rl.Add(CenteredText(title));
                if (title != "MONTH REPORT")
                    rl.Add(CenteredText("X-Reading"));
            }
            rl.Add("");

            //END Header----------------------------------

            DateTime ddt = DateTime.Parse(pd.Period_Create_TS);
            string cdt = "";

            String cl = null;

            if (pd.Period_Close_DT != null)
            {
               // ddt = DateTime.Parse(pd.Period_Close_DT);
                cdt = DateTime.Parse(pd.Period_Close_DT).ToString("MM/dd/yyyy hh:mm tt", dateFormat);
               // cl = ddt.ToString("MM/dd/yyyy hh:mm tt", dateFormat);
            }


            //rl.Add(CenteredText(strDate + "  " + chr.name));
            rl.Add(CenteredText(cl + "  " + activeCashier.name));
            rl.Add("");

            rl.Add(CenteredText("Period ID: " + pd.Period_ID));
            var dash = new string('-', lineChars);
            rl.Add(dash);


            rl.Add(MakePrintString(lineChars, "Open date:", ddt.ToString("MM/dd/yyyy hh:mm tt", dateFormat)));


            rl.Add(MakePrintString(lineChars, "Close date:", cdt));
            rl.Add(dash);
            if (title == "DAY REPORT" && type == 1)
            {

                foreach (var z in eodRpt)
                {
                    bool isNoAct = z.Reset_Start + z.Invoice_Begin == z.Reset_End + z.Invoice_End && int.Parse(z.Refund_Qty) < 1 ? true : false;

                    rl.Add("");
                    rl.Add(dash);
                    rl.Add(CenteredText("POS#" + z.POS_ID));
                    rl.Add(dash);
                    rl.Add(MakePrintString(lineChars, "Beg. Invoice#", SIformatter(z.Reset_Start, z.Invoice_Begin)));
                    rl.Add(MakePrintString(lineChars, "End. Invoice#", SIformatter(z.Reset_End, z.Invoice_End)));
                    rl.Add(MakePrintString(lineChars, "Beg. Refund#", SIformatter(z.Reset_TRS_Start, z.TRS_Start, true)));
                    rl.Add(MakePrintString(lineChars, "End. Refund#", SIformatter(z.Reset_TRS_End, z.TRS_End, true)));
                    rl.Add("");
                    rl.Add(MakePrintString(lineChars, "Reset Counter No.", z.Reset_Counter));
                    //rl.Add(MakePrintString(lineChars, "Z-Counter No.", z.Z_Counter));
                    rl.Add("");
                    rl.Add(MakePrintString(lineChars, "Present Accumulated Sales:", "P" + Math.Round(decimal.Parse(z.Sales_Present) + serviceChargeAmount[int.Parse(z.POS_ID) - 1][0], 2, MidpointRounding.AwayFromZero)));
                    rl.Add(MakePrintString(lineChars, "Previous Accumulated Sales:", "P" + Math.Round(decimal.Parse(z.Sales_Previous) + serviceChargeAmount[int.Parse(z.POS_ID) - 1][1], 2, MidpointRounding.AwayFromZero)));
                    rl.Add(MakePrintString(lineChars, "Sales for the Day:", "P" + Math.Round(decimal.Parse(z.Daily_Sales) + serviceChargeAmount[int.Parse(z.POS_ID) - 1][0], 2, MidpointRounding.AwayFromZero)));

                    Console.WriteLine($"{eodDisc.Where(a => a.POS_ID == int.Parse(z.POS_ID) && a.Discount_Preset_ID == 7).Sum(a => a.Discount_Value)} ======================================");

                    decimal vatableVal = eodTax.Where(a => a.POS_ID == int.Parse(z.POS_ID) && a.Tax_Name.Trim() == "VAT").Sum(a => a.Sale_Value);
                    eoD_Reports.Add(new EoD_Report
                    {
                        POSID = int.Parse(z.POS_ID),
                        Period_ID = int.Parse(z.Period_ID),
                        DT_Generated = nowDate.ToLongDateString(),
                        SI_Start = SIformatter(z.Reset_Start, z.Invoice_Begin),
                        SI_End = SIformatter(z.Reset_End, z.Invoice_End),
                        AccSalesEnd = decimal.Parse(z.Sales_Present),
                        AccSalesBegin = decimal.Parse(z.Sales_Previous),
                        ManualSales = 0,
                        GrossSales = eodTax.Where(a => a.POS_ID == int.Parse(z.POS_ID)).Sum(a => a.Sale_Value),
                        VATABLE_Sales = vatableVal / 1.12M,
                        VAT_Amount = vatableVal - (vatableVal / 1.12M),
                        VAT_Exempt = eodTax.Where(a => a.POS_ID == int.Parse(z.POS_ID) && a.Tax_Name.Trim() == "NVAT").Sum(a => a.Sale_Value) + serviceChargeAmount[int.Parse(z.POS_ID) - 1][0],
                        ZeroRated = eodTax.Where(a => a.POS_ID == int.Parse(z.POS_ID) && a.Tax_Name.Trim() == "Zero Rated").Sum(a => a.Sale_Value),
                        Deduct_SC = eodDisc.Where(a => a.POS_ID == int.Parse(z.POS_ID) && (a.Discount_Preset_ID == 1 || a.Discount_Preset_ID == 2)).Sum(a => a.Discount_Value),
                        Deduct_PWD = eodDisc.Where(a => a.POS_ID == int.Parse(z.POS_ID) && (a.Discount_Preset_ID == 3 || a.Discount_Preset_ID == 4)).Sum(a => a.Discount_Value),
                        Deduct_NAAC = eodDisc.Where(a => a.POS_ID == int.Parse(z.POS_ID) && a.Discount_Preset_ID == 6).Sum(a => a.Discount_Value),
                        Deduct_SP = eodDisc.Where(a => a.POS_ID == int.Parse(z.POS_ID) && a.Discount_Preset_ID == 5).Sum(a => a.Discount_Value),
                        Deduct_MoV = eodDisc.Where(a => a.POS_ID == int.Parse(z.POS_ID) && a.Discount_Preset_ID == 7).Sum(a => a.Discount_Value),
                        Deduct_Others = eodDisc.Where(a => a.POS_ID == int.Parse(z.POS_ID) && a.Discount_Preset_ID == 9 && a.Discount_Preset_ID == 13).Sum(a => a.Discount_Value),
                        Deduct_Returns = 0,
                        Deduct_Total = eodDisc.Where(a => a.POS_ID == int.Parse(z.POS_ID)).Sum(a => a.Discount_Value),
                        NetSales = decimal.Parse(z.Daily_Sales),
                        ResetCounter = int.Parse(z.Reset_Counter),
                        ZCounter = int.Parse(z.Z_Counter)

                    });

                    printZCounter = z.Z_Counter;
                    foreach (var ib in eodDisc)
                    {
                        Console.WriteLine(ib.Discount_Value);
                    }
                    Console.WriteLine("log1A");

                }
                rl.Add(dash);
                rl.Add("");

            }
            /*EoD_Report eodrep = new EoD_Report
            {
                POSID = int.Parse(er.POS_ID),
                Period_ID = int.Parse(er.POS_ID),
                DT_Generated = strDate,
                SI_Start = SIformatter(er.Reset_Start, er.Invoice_Begin),
                SI_End = SIformatter(er.Reset_End, er.Invoice_End),
                AccSalesEnd = Math.Round(decimal.Parse(er.Sales_Present), 2, MidpointRounding.AwayFromZero),
                AccSalesBegin = Math.Round(decimal.Parse(er.Sales_Previous), 2, MidpointRounding.AwayFromZero),
                ManualSales = mSales.Sum(a => a.postPayVal),
                GrossSales = transTotalVal,
                VATABLE_Sales = txSale,
                VAT_Amount = vatTax,
                VAT_Exempt = nvat.Tax_Sale_Value,
                ZeroRated = vatx.Tax_Sale_Value,
                Deduct_SC = discTotal.Where(a => a.discount_name.ToLower().Contains("senior")).Sum(a => a.discVal),
                Deduct_PWD = discTotal.Where(a => a.discount_name.ToLower().Contains("pwd")).Sum(a => a.discVal),
                Deduct_NAAC = discTotal.Where(a => a.discount_name.ToLower().Contains("naac")).Sum(a => a.discVal),
                Deduct_SP = discTotal.Where(a => a.discount_name.ToLower().Contains("solo")).Sum(a => a.discVal),
                Deduct_Others = discTotal.Where(a => !a.discount_name.ToLower().Contains("senior") && !a.discount_name.ToLower().Contains("pwd") && !a.discount_name.ToLower().Contains("naac") && !a.discount_name.ToLower().Contains("solo")).Sum(a => a.discVal),
                Deduct_Returns = cTotal.Sum(a => a.Cshr_Val_Refund),
                Deduct_Total = cTotal.Sum(a => a.Cshr_Val_Refund) + discTotal.Sum(a => a.discVal),
                NetSales = transTotalVal - (cTotal.Sum(a => a.Cshr_Val_Refund) + discTotal.Sum(a => a.discVal)),
                ResetCounter = int.Parse(er.Reset_Counter),
                ZCounter = int.Parse(er.Z_Counter)
            };*/


            //START DEPARTMENT SALES

            decimal deptTotalQty = 0;
            decimal deptTotalValue = 0;

            rl.Add(CenteredText("- Gross Department Sales -"));
            rl.Add("");

            strPrintData = ThreeColumnLine("Department", "Quantity", "Value");
            rl.Add(strPrintData);

            for (int i = 0; i < dSales.Count; i++)
            {
                if (dSales[i].depName != "FUELS")
                {
                    deptTotalQty += dSales[i].qty;
                    deptTotalValue += dSales[i].val;
                    decimal val = dSales[i].val;
                    if (dSales[i].depName == "PRODUCTS")
                    {
                        deptTotalValue += (serviceChargeAmount[0][0] + serviceChargeAmount[1][0]);
                        val += (serviceChargeAmount[0][0] + serviceChargeAmount[1][0]);
                    }
                    strPrintData = ThreeColumnLine(dSales[i].depName, dSales[i].qty.ToString("F0"), "P" + val.ToString("N", new CultureInfo("en-US")));
                    rl.Add(strPrintData);
                }
            }


            rl.Add("");
            strPrintData = ThreeColumnLine("TOTAL", deptTotalQty.ToString("F0"), "P" + deptTotalValue.ToString("N", new CultureInfo("en-US")));
            rl.Add(strPrintData);
            rl.Add("");

            //Department Refunds------
            rl.Add(CenteredText("- Department Refunds -"));
            rl.Add("");

            strPrintData = ThreeColumnLine("Department", "Quantity", "Value");
            rl.Add(strPrintData);

            decimal drQty = 0;
            decimal drVal = 0;
            foreach (Reports.DepRefunds dr in dpRefunds)
            {
                if (dr.depName != "FUELS" && dr.val != 0)
                {
                    drQty += dr.qty;
                    drVal += dr.val;

                    strPrintData = ThreeColumnLine(dr.depName, Math.Abs(dr.qty).ToString("F0"), "(P" + dr.val.ToString("N", new CultureInfo("en-US")) + ")");
                    rl.Add(strPrintData);
                }
            }

            rl.Add("");
            strPrintData = ThreeColumnLine("TOTAL", Math.Abs(drQty).ToString("F0"), "P" + drVal.ToString("N", new CultureInfo("en-US")));
            rl.Add(strPrintData);
            rl.Add("");


            //Department Discounts-----

            rl.Add(CenteredText("- Department Discount -"));
            rl.Add("");

            strPrintData = ThreeColumnLine("Department", "Quantity", "Value");
            rl.Add(strPrintData);

            decimal ddQty = 0;
            decimal ddVal = 0;
            foreach (Reports.DeptDiscounts dd in depDisc)
            {
                if (dd.depName != "FUELS" && dd.val != 0)
                {
                    ddQty += dd.qty;
                    ddVal += dd.val;

                    strPrintData = ThreeColumnLine(dd.depName, dd.qty.ToString("F0"), "(P" + dd.val.ToString("N", new CultureInfo("en-US")) + ")");
                    rl.Add(strPrintData);
                }
            }

            rl.Add("");
            strPrintData = ThreeColumnLine("TOTAL", ddQty.ToString("F0"), "P" + ddVal.ToString("N", new CultureInfo("en-US")));
            rl.Add(strPrintData);
            rl.Add("");

            //END DD----------------

            //DT Ex------------------
            rl.Add(CenteredText("- Department Tax Exemption -"));
            rl.Add("");

            strPrintData = ThreeColumnLine("Department", "Quantity", "Value");
            rl.Add(strPrintData);

            decimal dtxQty = 0;
            decimal dtxVal = 0;

            foreach (Reports.DeptTaxExempt dt in depTxEx)
            {
                if (dt.depName != "FUELS" && dt.val != 0)
                {
                    dtxQty += dt.qty;
                    dtxVal += dt.val;

                    strPrintData = ThreeColumnLine(dt.depName, Math.Abs(dt.qty).ToString("F0"), "P" + dt.val.ToString("N", new CultureInfo("en-US")));
                    rl.Add(strPrintData);
                }
            }

            rl.Add("");
            strPrintData = ThreeColumnLine("TOTAL", Math.Abs(dtxQty).ToString("F0"), "P" + dtxVal.ToString("N", new CultureInfo("en-US")));
            rl.Add(strPrintData);
            rl.Add("");

            //rl.Add(CenteredText("- Fuel Sales by Grade - "));
            rl.Add("");
            #region commented code
            //#region ManualDeliveries
            ////Manual Sales
            //int mnQty = 0;
            //decimal mnVal = 0;
            //decimal mnVol = 0;

            //rl.Add(CenteredText("- Gross Manual Deliveries -"));
            //rl.Add("");

            //strPrintData = FourColumnLine("Grade", "Qty", "Volume", "Value");
            //rl.Add(strPrintData);

            //for (int i = 0; i < mSales.Count; i++)
            //{
            //    if (mSales[i].postPayQty == 0) continue;

            //    decimal a = mSales[i].postPayVal + mSales[i].taxExVal;

            //    strPrintData = FourColumnLine(mSales[i].Grade_Name, mSales[i].postPayQty.ToString(), mSales[i].postPayVol.ToString("F3"), "P" + a.ToString("N", new CultureInfo("en-US")));
            //    rl.Add(strPrintData);

            //    mnQty += mSales[i].postPayQty;
            //    mnVal += a;
            //    mnVol += mSales[i].postPayVol;
            //}

            //rl.Add("");
            //strPrintData = FourColumnLine("TOTAL", mnQty.ToString(), mnVol.ToString("F3"), "P" + mnVal.ToString("N", new CultureInfo("en-US")));
            //rl.Add(strPrintData);
            //rl.Add("");
            //#endregion

            //#region ManualDeliveries Discount
            //// Manual Sales Discounts
            //rl.Add(CenteredText("- Manual Delivery Discounts -"));
            //rl.Add("");

            //strPrintData = FourColumnLine("Grade", "Qty", "Volume", "Value");
            //rl.Add(strPrintData);

            //int mnDiscQty = 0;
            //decimal mnDiscVal = 0;
            //decimal mnDiscVol = 0;

            //foreach (Reports.ManualFuelDiscount mfd in mfDisc)
            //{
            //    if (mfd.hoseQtyDisc == 0) continue;

            //    mnDiscQty += mfd.hoseQtyDisc;
            //    mnDiscVol += mfd.hoseVolDisc;
            //    mnDiscVal += mfd.hoseValDisc;

            //    strPrintData = FourColumnLine(mfd.Grade_Name, mfd.hoseQtyDisc.ToString(), mfd.hoseVolDisc.ToString("F3"), "P" + mfd.hoseValDisc.ToString("N", new CultureInfo("en-US")));
            //    rl.Add(strPrintData);
            //}

            //rl.Add("");
            //strPrintData = FourColumnLine("TOTAL", mnDiscQty.ToString(), mnDiscVol.ToString("F3"), "P" + mnDiscVal.ToString("N", new CultureInfo("en-US")));
            //rl.Add(strPrintData);
            //rl.Add("");
            //// End manual sales discounts
            //#endregion

            //#region ManualDelivery TaxEx
            //rl.Add(CenteredText("- Manual Delivery Tax Exemption -"));
            //rl.Add("");

            //strPrintData = ThreeColumnLine("Grade", "Qty", "Value");
            //rl.Add(strPrintData);

            //decimal mfTxQty = 0;
            //decimal mfTxVal = 0;

            //foreach (Reports.ManualTaxExempt mfTx in manualTaxExempts)
            //{
            //    if (mfTx.hoseTaxExVal == 0) continue;

            //    strPrintData = ThreeColumnLine(mfTx.Grade_Name, mfTx.hoseTaxExVol.ToString(), "P" + mfTx.hoseTaxExVal.ToString("N", new CultureInfo("en-US")));
            //    rl.Add(strPrintData);

            //    mfTxQty += mfTx.hoseTaxExVol;
            //    mfTxVal += mfTx.hoseTaxExVal;
            //}
            //rl.Add("");
            //strPrintData = ThreeColumnLine("TOTAL", mfTxQty.ToString("F0"), "P" + mfTxVal.ToString("N", new CultureInfo("en-US")));
            //rl.Add(strPrintData);
            //rl.Add("");
            //#endregion


            //#region ManualDeliveries Refund
            //rl.Add(CenteredText("- Manual Delivery Refunds -"));
            //rl.Add("");

            //strPrintData = ThreeColumnLine("Grade", "Volume", "Value");
            //rl.Add(strPrintData);

            //decimal mdRefVol = 0;
            //decimal mdRefVal = 0;

            //foreach (Reports.ManualRefund mdRef in manualFuelDiscounts)
            //{
            //    if (mdRef.hoseVolRef == 0 && mdRef.hoseValRef == 0) continue;
            //    mdRefVol += mdRef.hoseVolRef;
            //    mdRefVal += mdRef.hoseValRef;

            //    strPrintData = ThreeColumnLine(mdRef.Grade_Name, mdRef.hoseVolRef.ToString("N", new CultureInfo("en-US")), "P" + mdRef.hoseValRef.ToString("N", new CultureInfo("en-US")));
            //    rl.Add(strPrintData);
            //}

            //rl.Add("");
            //strPrintData = ThreeColumnLine("TOTAL", mdRefVol.ToString("N", new CultureInfo("en-US")), "P" + mdRefVal.ToString("N", new CultureInfo("en-US")));
            //rl.Add(strPrintData);
            //rl.Add("");
            //#endregion


            ////END DT Ex--------------

            //#region Fuel Sales
            ////POST PAY
            //int fuelTotalQty = 0;
            //decimal fuelTotalVol = 0;
            //decimal fuelTotalVal = 0;

            //rl.Add(CenteredText("- Postpay Deliveries -"));
            //rl.Add("");

            //strPrintData = FourColumnLine("Grade", "Qty", "Volume", "Value");
            //rl.Add(strPrintData);

            //for (int i = 0; i < fSales.Count; i++)
            //{
            //    if (fSales[i].postPayVal == 0 && fSales[i].postPayQty == 0) continue;

            //    strPrintData = FourColumnLine(fSales[i].Grade_Name, fSales[i].postPayQty.ToString(), fSales[i].postPayVol.ToString("F3"), "P" + fSales[i].postPayVal.ToString("N", new CultureInfo("en-US")));
            //    rl.Add(strPrintData);

            //    fuelTotalQty += fSales[i].postPayQty;
            //    fuelTotalVol += fSales[i].postPayVol;
            //    fuelTotalVal += fSales[i].postPayVal;
            //}

            //rl.Add("");

            //strPrintData = FourColumnLine("TOTAL", fuelTotalQty.ToString(), fuelTotalVol.ToString("F3"), "P" + fuelTotalVal.ToString("N", new CultureInfo("en-US")));
            //rl.Add(strPrintData);
            //rl.Add("");
            //#endregion

            //#region DeliveryDiscounts
            ////Delivery Discounts------------
            //rl.Add(CenteredText("- Delivery Discounts -"));
            //rl.Add("");

            //strPrintData = FourColumnLine("Grade", "Qty", "Volume", "Value");
            //rl.Add(strPrintData);

            //int fdQty = 0;
            //decimal fdVol = 0;
            //decimal fdVal = 0;

            //foreach (Reports.FuelDiscount fd in fuelDisc)
            //{
            //    if (fd.hoseValDisc == 0 && fd.hoseQtyDisc == 0) continue;

            //    fdVol += fd.hoseVolDisc;
            //    fdVal += fd.hoseValDisc;
            //    fdQty += fd.hoseQtyDisc;

            //    strPrintData = FourColumnLine(fd.Grade_Name, fd.hoseQtyDisc.ToString(), fd.hoseVolDisc.ToString("F3"), "P" + fd.hoseValDisc.ToString("N", new CultureInfo("en-US")));
            //    rl.Add(strPrintData);
            //}

            //rl.Add("");
            //strPrintData = FourColumnLine("TOTAL", fdQty.ToString(), fdVol.ToString("F3"), "P" + fdVal.ToString("N", new CultureInfo("en-US")));
            //rl.Add(strPrintData);
            //rl.Add("");
            //#endregion

            //#region Deliveries TaxEx

            //rl.Add(CenteredText("- Delivery Tax Exemption -"));
            //rl.Add("");

            //strPrintData = ThreeColumnLine("Grade", "Qty", "Value");
            //rl.Add(strPrintData);

            //decimal dlxQty = 0;
            //decimal dlxVal = 0;

            //foreach (Reports.DeliveryTaxExempt dlTxEx in delTxEx)
            //{
            //    if (dlTxEx.gradeTaxExemptQty == 0) continue;

            //    strPrintData = ThreeColumnLine(dlTxEx.Grade_Name, dlTxEx.gradeTaxExemptQty.ToString(), "P" + dlTxEx.gradeTaxExemptValue.ToString("N", new CultureInfo("en-US")));
            //    rl.Add(strPrintData);

            //    dlxQty += dlTxEx.gradeTaxExemptQty;
            //    dlxVal += dlTxEx.gradeTaxExemptValue;
            //}
            //rl.Add("");
            //strPrintData = ThreeColumnLine("TOTAL", dlxQty.ToString("F0"), "P" + dlxVal.ToString("N", new CultureInfo("en-US")));
            //rl.Add(strPrintData);
            //rl.Add("");
            //#endregion

            //#region FUELREFUNDS
            //rl.Add(CenteredText("- Fuel Refunds -"));
            //rl.Add("");

            //strPrintData = ThreeColumnLine("Grade", "Volume", "Value");
            //rl.Add(strPrintData);

            //decimal frQty = 0;
            //decimal frVal = 0;
            ////foreach (Reports.DepRefunds dr in dpRefunds)
            ////{
            ////    if (dr.depName == "FUELS" && dr.val != 0)
            ////    {
            ////        frQty += dr.qty;
            ////        frVal += dr.val;

            ////        strPrintData = ThreeColumnLine(dr.depName, dr.qty.ToString("F0"), "P" + dr.val.ToString("N", new CultureInfo("en-US")));
            ////        rl.Add(strPrintData);
            ////    }
            ////}
            //foreach (Reports.DeliveryRefund dlr in dlRefunds)
            //{
            //    if (dlr.qty == 0 && dlr.val == 0) continue;
            //    frQty += dlr.qty;
            //    frVal += dlr.val;

            //    strPrintData = ThreeColumnLine(dlr.gradeName, Math.Abs(dlr.qty).ToString("N", new CultureInfo("en-US")), "P" + Math.Abs(dlr.val).ToString("N", new CultureInfo("en-US")));
            //    rl.Add(strPrintData);
            //}

            //rl.Add("");
            //strPrintData = ThreeColumnLine("TOTAL", Math.Abs(frQty).ToString("N", new CultureInfo("en-US")), "P" + Math.Abs(frVal).ToString("N", new CultureInfo("en-US")));
            //rl.Add(strPrintData);
            //rl.Add("");
            //#endregion

            //#region VenusParts
            ////End Delivery Discounts-------------------

            ////TEST DELIVERY
            //int tdQty = 0;
            //decimal tdVol = 0;

            //if (appConfig["CLIENTCONFIG"]["testDel"] == "default")
            //{
            //    rl.Add(CenteredText("- Test Deliveries -"));
            //}
            //else
            //{
            //    rl.Add(CenteredText("- " + appConfig["CLIENTCONFIG"]["testDel"]) + " -");
            //}

            //rl.Add("");

            //strPrintData = ThreeColumnLine("Grade", "Qty", "Volume");
            //rl.Add(strPrintData);

            //for (int i = 0; i < fSales.Count; i++)
            //{
            //    if (fSales[i].testDelQty == 0) continue;
            //    strPrintData = ThreeColumnLine(fSales[i].Grade_Name, fSales[i].testDelQty.ToString(), fSales[i].testDelVol.ToString("F3"));
            //    rl.Add(strPrintData);

            //    tdQty += fSales[i].testDelQty;
            //    tdVol += fSales[i].testDelVol;
            //}

            //rl.Add("");

            //strPrintData = ThreeColumnLine("TOTAL", tdQty.ToString(), tdVol.ToString("F3"));
            //rl.Add(strPrintData);
            //rl.Add("");

            ////Driveoff Deliveries
            //int doQty = 0;
            //decimal doVal = 0;
            //decimal doVol = 0;

            //if (appConfig["CLIENTCONFIG"]["driveoffDel"] == "default")
            //{
            //    rl.Add(CenteredText("- DriveOff Deliveries -"));
            //}
            //else
            //{
            //    rl.Add(CenteredText("- " + appConfig["CLIENTCONFIG"]["driveoffDel"]) + " -");
            //}

            //rl.Add("");

            //strPrintData = FourColumnLine("Grade", "Qty", "Volume", "Value");
            //rl.Add(strPrintData);

            //for (int i = 0; i < fSales.Count; i++)
            //{
            //    if (fSales[i].offQty == 0) continue;

            //    strPrintData = FourColumnLine(fSales[i].Grade_Name, fSales[i].driveOffQty.ToString(), fSales[i].driveOffVol.ToString("F2"), fSales[i].driveOffCost.ToString("N", new CultureInfo("en-US")));
            //    rl.Add(strPrintData);

            //    doQty += fSales[i].driveOffQty;
            //    doVol += fSales[i].driveOffVol;
            //    doVal += fSales[i].driveOffCost;
            //}

            //rl.Add("");

            //strPrintData = FourColumnLine("TOTAL", doQty.ToString(), doVol.ToString("F2"), doVal.ToString("F2"));
            //rl.Add(strPrintData);
            //rl.Add("");

            ////OFFLINE
            //int offQty = 0;
            //decimal offVal = 0;
            //decimal offVol = 0;

            //rl.Add(CenteredText("- Offline Deliveries -"));
            //rl.Add("");

            //strPrintData = FourColumnLine("Grade", "Qty", "Volume", "Value");
            //rl.Add(strPrintData);

            //for (int i = 0; i < fSales.Count; i++)
            //{
            //    if (fSales[i].offQty == 0) continue;

            //    strPrintData = FourColumnLine(fSales[i].Grade_Name, fSales[i].offQty.ToString("F2"), fSales[i].offVol.ToString("F2"), fSales[i].offVal.ToString("N", new CultureInfo("en-US")));
            //    rl.Add(strPrintData);

            //    offQty += fSales[i].offQty;
            //    offVol += fSales[i].offVol;
            //    offVal += fSales[i].offVal;
            //}

            //rl.Add("");

            //strPrintData = FourColumnLine("TOTAL", offQty.ToString(), offVol.ToString("F2"), offVal.ToString("F2"));
            //rl.Add(strPrintData);
            //rl.Add("");
            //#endregion


            //#region TOTAL DELIVERIES
            //rl.Add("");
            //rl.Add(CenteredText("- Total Deliveries -"));
            //rl.Add("");

            //strPrintData = FourColumnLine("Grade", "Qty", "Volume", "Value");
            //rl.Add(strPrintData);
            //rl.Add("");

            //int fuelQtyTotal = 0;
            //decimal fuelVolTotal = 0;
            //decimal fuelValTotal = 0;

            //foreach (Reports.FuelSales fs in fSales)
            //{
            //    string name = fs.Grade_Name;
            //    int qty = fs.postPayQty;
            //    decimal vol = fs.postPayVol;
            //    decimal val = fs.postPayVal;

            //    foreach (Reports.ManualSales ms in mSales)
            //    {
            //        if (ms.Grade_Name == name)
            //        {
            //            qty += ms.postPayQty - ms.refundQty;
            //            vol += ms.postPayVol - ms.refundVol;
            //            val += (ms.postPayVal - ms.refundVal) + (ms.taxExVal - ms.taxExValRef);
            //        }
            //    }

            //    fuelQtyTotal += qty;
            //    fuelValTotal += val;
            //    fuelVolTotal += vol;

            //    strPrintData = FourColumnLine(fs.Grade_Name, qty.ToString(), vol.ToString("F3"), "P" + val.ToString("N", new CultureInfo("en-US")));
            //    rl.Add(strPrintData);

            //}

            //rl.Add("");
            //strPrintData = FourColumnLine("TOTAL", fuelQtyTotal.ToString(), fuelVolTotal.ToString("F3"), "P" + fuelValTotal.ToString("N", new CultureInfo("en-US")));
            //rl.Add(strPrintData);
            //rl.Add("");

            //#endregion
            #endregion


            #region TAX TOTALS
            rl.Add(CenteredText("- Tax Totals -"));
            rl.Add("");

            var vat = txHis.First(a => a.Tax_Name == "VAT");
            //var txSale = vat.Tax_Sale_Value - vat.Tax_Tot_Value;
            decimal nonTax = vat.Tax_NonTaxable_Discount.GetValueOrDefault(0);
            decimal netVatSale = (vat.Tax_Sale_Value - nonTax);
            decimal txSale = Math.Round(netVatSale / 1.12M, 2, MidpointRounding.AwayFromZero);
            var vatTax = Math.Round(netVatSale - txSale, 2, MidpointRounding.AwayFromZero);

            strPrintData = MakePrintString(lineChars, "VATable Sales", "P" + txSale.ToString("N", new CultureInfo("en-US")));
            rl.Add(strPrintData);

            //strPrintData = MakePrintString(lineChars, "VAT Amount", "P" + vat.Tax_Tot_Value.ToString("N", new CultureInfo("en-US")));
            strPrintData = MakePrintString(lineChars, "VAT Amount", "P" + vatTax.ToString("N", new CultureInfo("en-US")));

            rl.Add(strPrintData);

            var nvat = txHis.First(a => a.Tax_Name == "NVAT");
            strPrintData = MakePrintString(lineChars, "VAT-Exempt Sales", "P" + Math.Round(nvat.Tax_Sale_Value + (serviceChargeAmount[0][0] + serviceChargeAmount[1][0]), 2, MidpointRounding.AwayFromZero).ToString("N", new CultureInfo("en-US")));
            rl.Add(strPrintData);

            var vatx = txHis.First(a => a.Tax_Name == "Zero Rated");
            strPrintData = MakePrintString(lineChars, "VAT Zero-Rated Sales ", "P" + Math.Round(vatx.Tax_Sale_Value, 2, MidpointRounding.AwayFromZero).ToString("N", new CultureInfo("en-US")));
            rl.Add(strPrintData);

            rl.Add("");
            decimal oTotal = netVatSale + nvat.Tax_Sale_Value + vatx.Tax_Sale_Value;
            strPrintData = MakePrintString(lineChars, "TOTAL", "P" + Math.Round(oTotal + (serviceChargeAmount[0][0] + serviceChargeAmount[1][0]), 2, MidpointRounding.AwayFromZero).ToString("N", new CultureInfo("en-US")));
            rl.Add(strPrintData);
            #endregion


            //FINALISATIONs TOTAL

            rl.Add("");
            rl.Add(CenteredText("- Finalisations Total -"));
            rl.Add("");

            strPrintData = ThreeColumnLine("MOP", "Qty", "Value");
            rl.Add(strPrintData);

            decimal transTotalQty = 0;
            decimal transTotalVal = 0;

            for (int i = 0; i < fHis.Count; i++)
            {
                if (fHis[i].netQty > 0)
                {
                    transTotalQty += fHis[i].netQty;
                    transTotalVal += fHis[i].netValue + fHis[i].valSafedrop;

                    decimal val = fHis[i].netValue + fHis[i].valSafedrop;
                    if (i == 0)
                    {
                        /* val += (serviceChargeAmount[0][0] + serviceChargeAmount[1][0]);
                         transTotalVal += (serviceChargeAmount[0][0] + serviceChargeAmount[1][0]);*/
                    }
                    strPrintData = ThreeColumnLine(fHis[i].name, fHis[i].netQty.ToString(), "P" + (val).ToString("N", new CultureInfo("en-US")));
                    rl.Add(strPrintData);

                    if (fHis[i].name.ToLower().Trim() == "cash")
                    {
                        strPrintData = ThreeColumnLine("Safedrops", fHis[i].numSafedrop.ToString(), "(P" + fHis[i].valSafedrop.ToString("N", new CultureInfo("en-US")) + ")");
                        rl.Add(strPrintData);
                    }
                    if (fHis[i].name == "CASH DISCOUNT")
                    {
                        decimal totalDiscQty = 0;
                        decimal totalDiscVal = 0;

                        if (i != fHis.Count - 1)
                        {
                            totalDiscQty += fHis[i].netQty;
                            totalDiscVal += fHis[i].netValue;
                        }
                        else
                        {
                            strPrintData = ThreeColumnLine("   Disc/Chrg", totalDiscQty.ToString(), "P" + totalDiscVal.ToString("N", new CultureInfo("en-US")));
                            rl.Add(strPrintData);
                        }
                    }
                }
            }

            rl.Add("");
            strPrintData = ThreeColumnLine("TOTAL", transTotalQty.ToString("F0"), "P" + transTotalVal.ToString("N", new CultureInfo("en-US")));
            rl.Add(strPrintData);
            rl.Add("");

            //Discount Totals
            rl.Add(CenteredText("- Discount Totals -"));
            rl.Add("");

            strPrintData = ThreeColumnLine("Discounts", "Qty", "Value");
            rl.Add(strPrintData);

            decimal dTotal = 0;
            int dQty = 0;

            foreach (Reports.DiscountTotal dt in discTotal)
            {
                if (dt.discQty == 0)
                    continue;
                strPrintData = ThreeColumnLine(dt.discount_name, dt.discQty.ToString("F0"), "(P" + Math.Abs(dt.discVal).ToString("N", new CultureInfo("en-US")) + ")");
                rl.Add(strPrintData);
                dQty += dt.discQty;
                dTotal += dt.discVal;
            }
            if (Math.Abs(serviceChargeAmount[0][0] + serviceChargeAmount[1][0]) > 0)
            {
                strPrintData = ThreeColumnLine("Service Charge", 1.ToString("F0"), "P" + Math.Abs(serviceChargeAmount[0][0] + serviceChargeAmount[1][0]).ToString("N", new CultureInfo("en-US")));
                rl.Add(strPrintData);
            }
            dTotal += (serviceChargeAmount[0][0] + serviceChargeAmount[1][0]);
            rl.Add("");
            strPrintData = ThreeColumnLine("TOTAL", dQty.ToString("F0"), "(P" + Math.Abs(dTotal).ToString("N", new CultureInfo("en-US")) + ")");
            rl.Add(strPrintData);
            rl.Add("");

            //CASHIER TOTAL
            rl.Add(CenteredText("- Cashiers Total -"));
            rl.Add("");

            strPrintData = ThreeColumnLine("Cashiers", "Qty", "Value");
            rl.Add(strPrintData);
            rl.Add("");
            Console.WriteLine("cashier total");
            cTotal.ForEach(a => Console.WriteLine(a.Cashier_ID));
            for (int i = 0; i < cTotal.Count; i++)
            {
                if (cTotal[i].Cshr_Num_Trs != 0 || cTotal[i].Cshr_Num_Void != 0 || cTotal[i].Cshr_Num_Refund != 0 || cTotal[i].Cshr_Num_NoSales != 0)
                {
                    //TRANSACTIONS
                    decimal valgross = cTotal[i].Cshr_Val_Trs;
                    //decimal valgross = cTotal[i].Cshr_Val_Trs - Math.Abs(cTotal[i].Cshr_Val_Refund);
                    decimal valqty = cTotal[i].Cshr_Num_Trs;
                    //decimal valqty = cTotal[i].Cshr_Num_Trs - Math.Abs(cTotal[i].Cshr_Num_Refund);
                    //int valref = cTotal[i].Cshr_Val_Refund;
                    /*if(i == 0)
                    {
                        valgross += (serviceChargeAmount[0][0] + serviceChargeAmount[1][0]);
                    }*/

                    strPrintData = ThreeColumnLine(cTotal[i].Cashier_Name.Trim(), valqty.ToString(), "P" + valgross.ToString("N", new CultureInfo("en-US")));
                    rl.Add(strPrintData);
                    // transTotalQty += (cTotal[i].Cshr_Num_Trs + cTotal[i].Cshr_Num_Refund);
                    //VOIDS
                    if (cTotal[i].Cshr_Num_Void != 0)
                    {
                        //strPrintData = ThreeColumnLine("   Voids", cTotal[i].Cshr_Num_Void.ToString(), "P" + cTotal[i].Cshr_Val_Void.ToString("N", new CultureInfo("en-US")));
                        //  strPrintData = ThreeColumnLine("   Voids", cTotal[i].Cshr_Num_Void.ToString(), "");
                        //  rl.Add(strPrintData);
                    }
                    //NO SALES
                    if (cTotal[i].Cshr_Num_NoSales != 0)
                    {
                        strPrintData = ThreeColumnLine("   Open Drawer", cTotal[i].Cshr_Num_NoSales.ToString(), "P" + "0.00");
                        rl.Add(strPrintData);
                    }
                    //REFUNDS
                    if (cTotal[i].Cshr_Num_Refund != 0)
                    {
                        strPrintData = ThreeColumnLine("   Refunds", cTotal[i].Cshr_Num_Refund.ToString(), "(P" + Math.Abs(cTotal[i].Cshr_Val_Refund).ToString("N", new CultureInfo("en-US")) + ")");
                        rl.Add(strPrintData);
                    }
                    decimal cshrTot = cTotal[i].Cshr_Val_Trs - Math.Abs(cTotal[i].Cshr_Val_Refund);
                    /*if (i == 0)
                    {
                        cshrTot += (serviceChargeAmount[0][0] + serviceChargeAmount[1][0]);
                    }*/
                    strPrintData = ThreeColumnLine("TOTAL", (cTotal[i].Cshr_Num_Trs + Math.Abs(cTotal[i].Cshr_Num_Refund)).ToString(), (cshrTot).ToString("N", new CultureInfo("en-US")));
                    //strPrintData = ThreeColumnLine("TOTAL", " ", cTotal[i].Cshr_Num_Trs.ToString());
                    rl.Add(strPrintData);
                }
            }

            //#region VehicleTotals

            //rl.Add("");
            //rl.Add(CenteredText("- Vehicle Totals -"));
            //rl.Add("");
            //rl.Add(MakePrintString(lineChars, "Vehicle Type", "Count"));

            //int vTotal = 0;
            //foreach (Reports.VehicleTotals vt in vehicleTotals)
            //{
            //    strPrintData = MakePrintString(lineChars, vt.name, vt.count.ToString());
            //    rl.Add(strPrintData);
            //    vTotal += vt.count;
            //}

            //rl.Add("");
            //rl.Add(MakePrintString(lineChars, "TOTAL", vTotal.ToString()));
            //#endregion

            #region HourlySales

            //add condition
            if (title.ToUpper() == ("SHIFT REPORT").ToUpper() && appConfig["REPORTS"]["HourlySection"].ToLower() == "y")
            {
                rl.Add("");
                rl.Add(CenteredText("- HOURLY TOTAL SALES -"));
                rl.Add("");

                strPrintData = FourColumnLine("Hour", "Qty", "Volume", "Value");
                rl.Add(strPrintData);
                rl.Add("");

                try
                {
                    for (int j = 0; j <= 23; j++)
                    {
                        strPrintData = "";

                        //Create Date with specific time
                        TimeSpan ts = TimeSpan.FromHours(j);
                        DateTime startTime = DateTime.Parse("01/01/2000").Add(ts);

                        string timestring = startTime.ToString("hhtt") + " - " + (startTime.AddHours(1)).ToString("hhtt");
                        int count = thData.Where(data => DateTime.Parse(data.TransactionHour).TimeOfDay == startTime.TimeOfDay).Select(data => data.transCount).FirstOrDefault();
                        decimal value = thData.Where(data => DateTime.Parse(data.TransactionHour).TimeOfDay == startTime.TimeOfDay).Select(data => data.saleTotal).FirstOrDefault();
                        decimal volume = (thData.Where(data => DateTime.Parse(data.TransactionHour).TimeOfDay == startTime.TimeOfDay).Select(data => data.Quantity).FirstOrDefault()).GetValueOrDefault();

                        strPrintData = FourColumnLine(timestring, count.ToString(), volume.ToString("F3"), "P" + value.ToString("N", new CultureInfo("en-us")));
                        rl.Add(strPrintData);
                    }
                }
                catch { }

            }

            #endregion

            rl.Add("");

            if(title != "DAY REPORT")
            {
                rl.Add(CenteredText("- Invoice Numbers -"));
                rl.Add("");
            }

            if (appConfig["POS"]["UseResetter"] == "Y")
            {
                string rctType = appConfig["POS"]["RecieptType"];
                foreach (Reports.TransactionNumbers tr in transNum)
                {
                    rl.Add("POS #" + tr.posID.ToString());

                    string parsedStart = tr.startingTransReset.PadLeft(3, '0') + "-" + rctType + "#" + tr.startingTransNum.PadLeft(9, '0');
                    string parsedEnd = tr.endingTransReset.PadLeft(3, '0') + "-" + rctType + "#" + tr.endingTransNum.PadLeft(9, '0');


                    //if (parsedStart == parsedEnd)
                    if (tr.Qty == 0)
                    {
                        rl.Add("NO ACTIVITY");
                    }
                    else
                    {
                        strPrintData = MakePrintString(lineChars, "Beginning Sales Invoice", parsedStart);
                        rl.Add(strPrintData);

                        strPrintData = MakePrintString(lineChars, "Ending Sales Invoice", parsedEnd);
                        rl.Add(strPrintData);
                    }
                }
                if (title == "DAY REPORT" && type == 1)
                    rl.Add(MakePrintString(lineChars, "Z-Counter", printZCounter));
            }
            else
            {
                foreach (Reports.TransactionNumbers tr in transNum)
                {
                    rl.Add("POS #" + tr.posID.ToString());
                    //if (tr.startingTransNum == tr.endingTransNum)
                    if (tr.Qty == 0)
                    {
                        rl.Add("NO ACTIVITY");
                    }
                    else
                    {
                        strPrintData = MakePrintString(lineChars, "Beginning Sales Invoice", tr.startingTransNum);
                        rl.Add(strPrintData);
                        strPrintData = MakePrintString(lineChars, "Ending Sales Invoice", tr.endingTransNum);
                        rl.Add(strPrintData);
                    }
                }
            }

            rl.Add("");
            rl.Add("");
            strPrintData = ThreeColumnLine("TOTAL", transTotalQty.ToString("F0"), "P" + transTotalVal.ToString("N", new CultureInfo("en-US")));
            rl.Add(strPrintData);

            //#region HoseDeliveries


            //if (incReadings == true || appConfig["REPORTS"]["HoseSection"] == "Y")
            //{
            //    rl.Add("");
            //    rl.Add(CenteredText("- HOSE DELIVERIES -"));
            //    rl.Add("");

            //    strPrintData = ThreeColumnLine("", "Volume", "Value");
            //    rl.Add(strPrintData);
            //    rl.Add("");

            //    foreach (Reports.HoseHistData hh in hhData)
            //    {
            //        strPrintData = "p" + hh.logicalNum + "h" + hh.hoseNum + " " + hh.gradeName;
            //        rl.Add(strPrintData);

            //        strPrintData = ThreeColumnLine("  Closing", hh.closeMeterVol.ToString("F2"), "P" + hh.closeMeterVal.ToString("N", new CultureInfo("en-us")));
            //        rl.Add(strPrintData);
            //        strPrintData = ThreeColumnLine("  Opening", hh.openMeterVol.ToString("F2"), "P" + hh.openMeterVal.ToString("N", new CultureInfo("en-us")));
            //        rl.Add(strPrintData);
            //        strPrintData = ThreeColumnLine("  Deliveries", hh.postpayVol.ToString("F2"), "P" + hh.postpayVal.ToString("N", new CultureInfo("en-us")));
            //        rl.Add(strPrintData);

            //        if (hh.driveoffVol > 0)
            //        {
            //            strPrintData = ThreeColumnLine("    Driveoffs", hh.driveoffVol.ToString("F2"), "P" + hh.driveoffVal.ToString("N", new CultureInfo("en-us")));
            //            rl.Add(strPrintData);
            //        }

            //        if (hh.testDelVol > 0)
            //        {
            //            strPrintData = ThreeColumnLine("    Test Delvs", hh.testDelVol.ToString("F2"), "");
            //            rl.Add(strPrintData);
            //        }
            //    }

            //    rl.Add("");
            //    rl.Add(CenteredText("- TANK DELIVERIES -"));
            //    rl.Add("");
            //    foreach (Reports.TankHistory th in tanks)
            //    {

            //        rl.Add("");
            //        strPrintData = MakePrintString(lineChars, th.tankName, "Volume");
            //        rl.Add(strPrintData);
            //        rl.Add(MakePrintString(lineChars, "Opening", th.openVolume.ToString("F4")));
            //        rl.Add(MakePrintString(lineChars, "Added", th.addedVolume.ToString("F4")));
            //        rl.Add(MakePrintString(lineChars, "Sold", th.soldVolume.ToString("F4")));
            //        rl.Add(MakePrintString(lineChars, "Closing", th.closeVolume.ToString("F4")));
            //    }
            //}

            //#endregion

            rl.Add("");
            Footers(out string[] f1, out string[] f2, out string[] f3, out string[] f4, out string[] f5);
            foreach (string ln in f2)
                rl.Add(CenteredText(ln.Trim()));
            rl.Add("");

            foreach (string ln in f4)
            {
                string[] xb = ln.Split(new char[] { '\n' });
                foreach (string lnb in xb)
                {
                    rl.Add(CenteredText(lnb.Trim()));
                }

            }

            /*if (reprint)
            {
                foreach (string ln in f4)
                {
                    string[] xb = ln.Split(new char[] { '\n' });
                    foreach (string lnb in xb)
                    {
                        rl.Add(CenteredText(lnb.Trim()));
                    }

                }
            }*/

            rl.Add("");


            string dataToPrintLog = "";
            foreach (string line in rl)
            {
                dataToPrintLog += $"{line}\n";
            }
            SaveToPrintLog(dataToPrintLog);

            string open = pd.Period_Create_TS;
            string[] x = open.Split(' ');
            string[] xx = x[1].Split('.');
            xx[0] = Regex.Replace(xx[0], @":", "");
            xx[0] = xx[0].Remove(4, 2);
            open = x[0] + " " + xx[0];

            string close = pd.Period_Close_DT;

            if (close == null)
            {
                close = "";
            }
            else
            {
                string[] y = close.Split(' ');
                string[] yy = y[1].Split('.');
                yy[0] = Regex.Replace(yy[0], @":", "");
                yy[0] = yy[0].Remove(4, 2);
                close = y[0] + " " + yy[0];
            }


            string path = "reports\\" + title + " " + open + " to " + close + ".txt";
            fileIO.CreateTextFile(path, rl);
            //PrintText(rl);
            Console.WriteLine("this path");
            Console.WriteLine(path);
            if (activeCashier.bof == 1)
            {
                await PrintText(rl);
            }
            else
            {
                MessageBox.Show("Printing of report is disabled for this user.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (appConfig["APP"]["AutoOpenTextFiles"] == "true")
                Process.Start(path);

            //string args = "0 "  +pd.Period_ID;
            //StartProcess(appConfig["WINTAS"]["Path"], args);
            if (title == "DAY REPORT")
            {
                EnbDB enb = new EnbDB(loggedInPOSID);
                foreach (EoD_Report er in eoD_Reports)
                {
                    enb.SaveEoDReport(er);
                    //  enb.SaveEoDReport(er);
                }

                //enb.SaveEoDReport(er);



            }
        }


        public bool StartProcess(string path, string args)
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.FileName = path;
                processInfo.ErrorDialog = true;
                processInfo.UseShellExecute = false;
                processInfo.RedirectStandardOutput = true;
                processInfo.RedirectStandardError = true;
                processInfo.Arguments = args;
                processInfo.WorkingDirectory = Path.GetDirectoryName(path);
                Process proc = Process.Start(processInfo);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + path, "Unable To Start Program", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        private void btn_close_day_Click(object sender, EventArgs e)
        {
            string closeResult = api.CheckCloseShiftTodayStatus(2);

            if (closeResult != "continue")
            {
                MessageBox.Show("Day is already closed!");
                return;
            }


            if (activeCashier.clsDay != 1)
            {
                MessageBox.Show("User not allowed to close day.");
                return;
            }

            var result = MessageBox.Show("Do you really want to close day?", "Close Day", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result != DialogResult.Yes)
                return;

            MessageBox.Show("Generating Day Report.");

            var respo = api.closePeriod(2);
            if (respo == "false")
            {
                MessageBox.Show("Close day failed");
                return;
            }

            ProcessReports(respo, "DAY REPORT", 1);
            enb.LogPOSEvent($"Cashier Close Period:{activeCashier.name}", 13, loggedInPOSID, activeCashier.name);

            MessageBox.Show("Close day successful", "Day Closed", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CloseDayStarted = true;

            //btn_close_shift.PerformClick();

            //btn_close_cdraw.PerformClick();

        }

        private void btn_close_month_Click(object sender, EventArgs e)
        {
            if (activeCashier.clsMonth != 1)
            {
                MessageBox.Show("User not allowed to close month.");
                return;
            }

            var result = MessageBox.Show("Do you really want to close month?", "Close Month", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result != DialogResult.Yes)
                return;

            MessageBox.Show("Generating Month Report.");

            var respo = api.closePeriod(3);
            if (respo == "false")
            {
                MessageBox.Show("Close day failed");
                return;
            }

            ProcessReports(respo, "MONTH REPORT", 0);
            MessageBox.Show("Close month successful", "Month Closed", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (File.Exists("logs\\ApiLogs.txt"))
            {
                File.Delete("logs\\ApiLogs.txt");
            }

            string rctArchiveFolder = @"oldrct\rct - " + DateTime.Now.ToString("MMMM yyyy");
            string reportsArchiveFolder = @"oldreports\reports - " + DateTime.Now.ToString("MMMM yyyy");

            FileIO.RenameDirectory("rct", rctArchiveFolder);
            FileIO.CreateDirectory("rct");

            FileIO.RenameDirectory("reports", reportsArchiveFolder);
            FileIO.CreateDirectory("reports");
        }

        private void btn_refund_report_Click(object sender, EventArgs e)
        {
            if (activeCashier.bof != 1)
            {
                MessageBox.Show("User is not allowed to print Refund Report.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            RefundReportForm voidRefundDlg = new RefundReportForm();
            if (voidRefundDlg.ShowDialog() == DialogResult.OK)
            {
                bool isFullReport = voidRefundDlg.FullReport;
                var startDate = voidRefundDlg.StartDate;
                var endDate = voidRefundDlg.EndDate;

                //Console.WriteLine("Form1 [{0}]::[{1}]", startDate, endDate);

                var result = api.GetVoidRefundData(startDate.ToString("yyyy-MM-dd HH:mm:ss"), endDate.ToString("yyyy-MM-dd HH:mm:ss"));
                if (result.ToUpper() == "FALSE")
                {
                    MessageBox.Show("No Data Available");
                }
                else
                {
                    VoidAndRefund report = JsonConvert.DeserializeObject<VoidAndRefund>(result);
                    Var_Dump(report);
                    PrepareVoidAndRefundReport(report, startDate, endDate, isFullReport);
                }
            }
        }

        private void Var_Dump(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            Console.WriteLine("Var_Dump :: " + json);
            Console.WriteLine("");
        }

        private async void PrepareVoidAndRefundReport(VoidAndRefund data, DateTime start, DateTime end, bool fullReport)
        {
            List<string> rl = new List<string>();

            DateTime nowDate = DateTime.Now;                            //System date
            DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();   //Date Format
            dateFormat.MonthDayPattern = "MM";
            string strDate = nowDate.ToString("MM/dd/yyyy hh:mm tt", dateFormat);

            string strDate2 = nowDate.ToString("MM-dd-yyyy", dateFormat);

            string strPrintData = "";

            Headers(out string[] h1, out string[] h2, out string[] h3, out string[] h4, out string[] h5);

            foreach (string ln in h1)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h2)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h3)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h4)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h5)
                rl.Add(CenteredText(ln.Trim()));

            rl.Add("");

            //if (fullReport)
            //{
            rl.Add(CenteredText("Refund Report"));
            //}
            //else
            //{
            //    rl.Add(CenteredText("Void & Refund Summary Report"));
            //}

            rl.Add("");

            string dash = new string('-', lineChars);
            rl.Add(dash);
            strPrintData = MakePrintString(lineChars, "From:", start.ToString("yyyy/MM/dd"));
            rl.Add(strPrintData);
            strPrintData = MakePrintString(lineChars, "To:", end.ToString("yyyy/MM/dd"));
            rl.Add(strPrintData);
            rl.Add(dash);

            //END Header----------------------------------
            /*#region VOIDS
            rl.Add(CenteredText("- Voided Items by Cashier -"));
            rl.Add("");


            var voids = data.voidData;
            var csID = voids.Select(v => v.Cashier_ID).Distinct();
            foreach (int id in csID)
            {
                var csname = (from item in voids
                              where item.Cashier_ID == id
                              select item.Cashier_Name).First();

                decimal totpercashier = 0;
                int qty = 0;
                rl.Add(CenteredText("Cashier: " + id.ToString() + " " + csname));
                //this is the voids per cashier
                var byCashier = voids.Where(v => v.Cashier_ID == id);
                //this is the dates per cashier
                var dates = byCashier.Select(by => by.Void_Event_Date.Date).Distinct();
                foreach (DateTime dt in dates)
                {
                    var z = byCashier.Where(cs => cs.Void_Event_Date.Date == dt);
                    if (fullReport)
                    {
                        rl.Add(dt.ToString("MM/dd/yyyy"));
                        foreach (VoidReport y in z)
                        {
                            totpercashier += y.Item_Value;
                            string left = y.Void_Event_Date.ToString("HH:mm") + " " + y.Item_Description;
                            string right = y.Item_Value.ToString("c");
                            rl.Add(MakePrintString(lineChars, left, right));
                            qty++;
                        }
                    }
                    else
                    {
                        var sum = z.Select(v => v.Item_Value).Sum();
                        var count = z.Count();
                        strPrintData = MakePrintString(lineChars, dt.ToString("MM/dd/yyyy"), sum.ToString("c"));
                        rl.Add(strPrintData);
                        totpercashier += sum;
                        qty += count;
                    }
                    rl.Add("");
                }
                rl.Add(ThreeColumnLine("TOTAL", qty.ToString(), totpercashier.ToString("c")));
                rl.Add("");
            }

            rl.Add(dash);
            rl.Add(CenteredText("- Voided Items by Date -"));
            rl.Add("");

            var voiddates = voids.Select(v => v.Void_Event_Date.Date).Distinct();
            foreach (DateTime dt in voiddates)
            {
                rl.Add(CenteredText(dt.ToString("MM/dd/yyyy")));
                var byDate = voids.Where(v => v.Void_Event_Date.Date == dt.Date);
                var csid = byDate.Select(c => c.Cashier_ID).Distinct();
                decimal totPerDate = 0;
                int qtyPerDate = 0;
                foreach (int id in csid)
                {
                    var csname = (from item in voids
                                  where item.Cashier_ID == id
                                  select item.Cashier_Name).First();
                    var zz = byDate.Where(bd => bd.Cashier_ID == id);
                    if (fullReport)
                    {
                        foreach (VoidReport y in zz)
                        {
                            rl.Add("Cashier: " + id.ToString() + " " + csname);
                            string left = y.Void_Event_Date.ToString("HH:mm") + " " + y.Item_Description;
                            string right = y.Item_Value.ToString("c");
                            rl.Add(MakePrintString(lineChars, left, right));
                            totPerDate += y.Item_Value;
                            qtyPerDate++;
                        }
                    }
                    else
                    {
                        var sum = zz.Select(v => v.Item_Value).Sum();
                        var count = zz.Count();
                        strPrintData = ThreeColumnLine(id + " " + csname, count.ToString(), sum.ToString("c"));
                        rl.Add(strPrintData);
                        qtyPerDate += count;
                        totPerDate += sum;
                    }
                }

                rl.Add("");
                rl.Add(ThreeColumnLine("TOTAL", qtyPerDate.ToString(), totPerDate.ToString("c")));
                rl.Add("");
            }
            #endregion*/

            #region REFUNDS

            rl.Add(dash);
            rl.Add(CenteredText("- Refunded Sales by Cashier -"));
            rl.Add("");

            var refunds = data.refundData;
            var refCashierIDs = refunds.Select(r => r.cashierID).Distinct();
            foreach (int id in refCashierIDs)
            {
                var csname = (from item in refunds
                              where item.cashierID == id
                              select item.cashierName).First();
                rl.Add(CenteredText("Cashier: " + id.ToString() + " " + csname));
                rl.Add("");
                var refByCashier = refunds.Where(r => r.cashierID == id);
                decimal refTotalPerCashier = 0;
                string lastRefundNumber = "";
                foreach (RefundReport rf in refByCashier)
                {
                    //strPrintData = rf.transDate.ToString("MM/dd/yyyy HH:mm") + " POS: " + rf.POSID + " #" + rf.cashierID;
                    strPrintData = rf.transDate.ToString("MM/dd/yyyy HH:mm") + " POS: " + rf.POSID;
                    rl.Add(strPrintData);

                    string refundORNum = $"Refund #:{rf.transaction_resetter.ToString().PadLeft(3, '0')}-{rf.BIR_Trans_Number.ToString().PadLeft(9, '0')}";
                    if (lastRefundNumber != refundORNum)
                    {
                        //rl.Add(" ");
                        strPrintData = MakePrintString(lineChars, refundORNum, "");
                        rl.Add(strPrintData);
                        string refundReferenceNum = $"Reference #:{rf.transaction_resetter_reference.ToString().PadLeft(3, '0')}-{rf.transaction_number_reference.ToString().PadLeft(9, '0')}";
                        strPrintData = MakePrintString(lineChars, refundReferenceNum, "");
                        rl.Add(strPrintData);
                        lastRefundNumber = refundORNum;
                    }

                    foreach (RefundItem ri in rf.saleItems)
                    {
                        if (ri.Item_Type == 2 || ri.Item_Type == 14 || ri.Item_Type == 53)
                        {

                           
                            

                            strPrintData = MakePrintString(lineChars, ri.Item_Description, ri.Value.ToString("c"));
                            rl.Add(strPrintData);
                            refTotalPerCashier += ri.Value;
                        }
                    }
                    rl.Add("");
                }
                strPrintData = MakePrintString(lineChars, "TOTAL", refTotalPerCashier.ToString("c"));
                rl.Add(strPrintData);
            }

            rl.Add(dash);
            rl.Add(CenteredText("- Refunded Sales by Date -"));
            rl.Add("");

            var refundDates = refunds.Select(r => r.transDate.Date).Distinct();
            decimal refTotalPerDate = 0;
            foreach (DateTime dt in refundDates)
            {
                rl.Add(CenteredText(dt.ToString("MM/dd/yyyy")));
                rl.Add("");
                var refundsByDate = refunds.Where(r => r.transDate.Date == dt);

                string lastRefundNumber = "";

                foreach (RefundReport rf in refundsByDate)
                {
                    strPrintData = rf.transDate.ToString("HH:mm") + " POS: " + rf.POSID + " #" + rf.cashierID + " " + rf.cashierName;
                    rl.Add(strPrintData);

                    string refundORNum = $"Refund #:{rf.transaction_resetter.ToString().PadLeft(3, '0')}-{rf.BIR_Trans_Number.ToString().PadLeft(9, '0')}";
                    
                    if(lastRefundNumber != refundORNum)
                    {       
                        //rl.Add(" ");
                        strPrintData = MakePrintString(lineChars, refundORNum, "");
                        rl.Add(strPrintData);
                        string refundReferenceNum = $"Refund #:{rf.transaction_resetter_reference.ToString().PadLeft(3, '0')}-{rf.transaction_number_reference.ToString().PadLeft(9, '0')}";
                        strPrintData = MakePrintString(lineChars, refundReferenceNum, "");
                        rl.Add(strPrintData);
                        lastRefundNumber = refundORNum;
                    }

                    foreach (RefundItem ri in rf.saleItems)
                    {
                        if (ri.Item_Type == 10 && ri.Value == 0)
                            continue;
                        strPrintData = MakePrintString(lineChars, ri.Item_Description, ri.Value.ToString("c"));
                        rl.Add(strPrintData);
                    }
                    rl.Add("");
                    refTotalPerDate += rf.saleTotal;
                }
                rl.Add("");
                strPrintData = MakePrintString(lineChars, "TOTAL", refTotalPerDate.ToString("c"));
                rl.Add(strPrintData);
            }

            #endregion


            //Footer--------------------------------------
            rl.Add("");
            Footers(out string[] f1, out string[] f2, out string[] f3, out string[] f4, out string[] f5);
            foreach (string ln in f2)
                rl.Add(CenteredText(ln.Trim()));
            rl.Add("");

            foreach (string ln4 in f4)
                rl.Add(CenteredText(ln4.Trim()));
            rl.Add("");

            string open = start.ToString("yyyyMMdd");
            string close = end.ToString("yyyyMMdd");

            string dataToPrintLog = "";
            foreach (string line in rl)
            {
                dataToPrintLog += $"{line}\n";
            }
            SaveToPrintLog(dataToPrintLog);


            string path = "reports\\VoidAndRefund" + " " + open + " to " + close + ".txt";
            fileIO.CreateTextFile(path, rl);
            //PrintText(rl);

            if (activeCashier.bof == 1)
            {
                await PrintText(rl);
            }
            else
            {
                //MessageBox.Show("Printing of report is disabled for this user.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return;
            }

            if (appConfig["APP"]["AutoOpenTextFiles"] == "true")
                Process.Start(path);

        }

        private void btn_cumulative_sale_Click(object sender, EventArgs e)
        {
            if (activeCashier.cumsale != 1)
            {
                MessageBox.Show("User not allowed to generate cummulative sale");
                return;
            }

            MessageBox.Show("Processing Data");

            var respo = api.GetCummulativeSale();
            
            List<string> st = new List<string>
            {
                respo
            };

            ProcessCumSales(respo);
            MessageBox.Show("Accumulating Report Generated", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ProcessCumSales(string data)
        {
            Console.WriteLine("PCS :: " + data);

            var details = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

            string gradeHistSum = JsonConvert.SerializeObject(details["gradeHistSum"]);
            string deptHistSum = JsonConvert.SerializeObject(details["deptHistSum"]);
            string mopHistSum = JsonConvert.SerializeObject(details["mopHistSum"]);
            string taxHistSum = JsonConvert.SerializeObject(details["taxHistSum"]);
            string discTotal = JsonConvert.SerializeObject(details["discTotal"]);
            string saleTotal = JsonConvert.SerializeObject(details["saleTotal"]);
            string scpwdDiscTotal = JsonConvert.SerializeObject(details["scpwdDiscTotal"]);
            string discountSum = JsonConvert.SerializeObject(details["discPresetHistSum"]);
            string dtails = JsonConvert.SerializeObject(details["details"]);
            string netsales = JsonConvert.SerializeObject(details["netSales"]);
            string refundSum = JsonConvert.SerializeObject(details["refundHistSum"]);
            string serviceChargeSum = details["serviceChargeSum"].ToString();

            Console.WriteLine("GH is :: " + gradeHistSum);
            List<CummulativeSales.DiscPresetHistSum> dpSum = new List<CummulativeSales.DiscPresetHistSum>();
            dpSum = JsonConvert.DeserializeObject<List<CummulativeSales.DiscPresetHistSum>>(discountSum);

            List<CummulativeSales.GradeHistSum> ghSum = new List<CummulativeSales.GradeHistSum>();
            ghSum = JsonConvert.DeserializeObject<List<CummulativeSales.GradeHistSum>>(gradeHistSum);

            List<CummulativeSales.DeptHistSum> dhSum = new List<CummulativeSales.DeptHistSum>();
            dhSum = JsonConvert.DeserializeObject<List<CummulativeSales.DeptHistSum>>(deptHistSum);

            List<CummulativeSales.MopHistSum> mhSum = new List<CummulativeSales.MopHistSum>();
            mhSum = JsonConvert.DeserializeObject<List<CummulativeSales.MopHistSum>>(mopHistSum);

            List<CummulativeSales.TaxHistSum> thSum = new List<CummulativeSales.TaxHistSum>();
            thSum = JsonConvert.DeserializeObject<List<CummulativeSales.TaxHistSum>>(taxHistSum);

            List<CummulativeSales.DiscTotal> discTot = new List<CummulativeSales.DiscTotal>();
            discTot = JsonConvert.DeserializeObject<List<CummulativeSales.DiscTotal>>(discTotal);

            List<CummulativeSales.SaleTotal> saleTot = new List<CummulativeSales.SaleTotal>();
            saleTot = JsonConvert.DeserializeObject<List<CummulativeSales.SaleTotal>>(saleTotal);

            List<CummulativeSales.ScpwdDiscTotal> scdiscTot = new List<CummulativeSales.ScpwdDiscTotal>();
            scdiscTot = JsonConvert.DeserializeObject<List<CummulativeSales.ScpwdDiscTotal>>(scpwdDiscTotal);

            CummulativeSales.Details dts = new CummulativeSales.Details();
            dts = JsonConvert.DeserializeObject<CummulativeSales.Details>(dtails);

            List<CummulativeSales.NetSales> netSal = new List<CummulativeSales.NetSales>();
            netSal = JsonConvert.DeserializeObject<List<CummulativeSales.NetSales>>(netsales);

            CummulativeSales.RefundSummary refunds = new CummulativeSales.RefundSummary();
            refunds = JsonConvert.DeserializeObject<CummulativeSales.RefundSummary>(refundSum);

            PrepareCumSales(ghSum, dhSum, mhSum, thSum, discTot, saleTot, scdiscTot, dts, netSal, refunds, dpSum, serviceChargeSum);
        }

        private async void PrepareCumSales(List<CummulativeSales.GradeHistSum> ghSum, List<CummulativeSales.DeptHistSum> dhSum, List<CummulativeSales.MopHistSum> mhSum,
            List<CummulativeSales.TaxHistSum> thSum, List<CummulativeSales.DiscTotal> discTot, List<CummulativeSales.SaleTotal> saleTot,
            List<CummulativeSales.ScpwdDiscTotal> scdiscTot, CummulativeSales.Details details, List<CummulativeSales.NetSales> netSales,
            CummulativeSales.RefundSummary refunds, List<CummulativeSales.DiscPresetHistSum> dpSum, string serviceCharge)
        {
            AccumulatingReport report = new AccumulatingReport();

            List<string> rl = new List<string>();

            DateTime nowDate = DateTime.Now;                            //System date
            DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();   //Date Format
            dateFormat.MonthDayPattern = "MM";
            string strDate = nowDate.ToString("MM/dd/yyyy hh:mm tt", dateFormat);

            string strDate2 = nowDate.ToString("MM-dd-yyyy-HHmm", dateFormat);

            string strPrintData = "";

            decimal serviceChargeAmt = decimal.Parse(serviceCharge);

            //Reciept HEADER------------------------------------------------------
            //rl.Add(CenteredText("ACCUMULATING GRAND TOTAL SALES REPORT"));
            //rl.Add(CenteredText("Z-Reading"));
            //rl.Add(CenteredText("Z-Reading"));
            rl.Add("");

            Headers(out string[] h1, out string[] h2, out string[] h3, out string[] h4, out string[] h5);

            foreach (string ln in h1)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h2)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h3)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h4)
                rl.Add(CenteredText(ln.Trim()));
            foreach (string ln in h5)
                rl.Add(CenteredText(ln.Trim()));

            rl.Add("");

            //Reciept HEADER------------------------------------------------------
            rl.Add(CenteredText("ACCUMULATING GRAND TOTAL SALES REPORT"));
            rl.Add("");

            //End HEADER----------------------------------------------------------
            rl.Add(CenteredText(strDate));
            rl.Add("");
            rl.Add(CenteredText("POS " + cfg.POS_ID));

            //REPORTS DB
            report.PosID = cfg.POS_ID;

            var dash = new string('-', lineChars);
            rl.Add(dash);


            DateTime ddt = DateTime.Parse(details.startDate);
            rl.Add(MakePrintString(lineChars, "Open date:", ddt.ToString("MM/dd/yyyy hh:mm tt", dateFormat)));
            report.StartDate = ddt.ToString("MM/dd/yyyy hh:mm tt", dateFormat);

            ddt = DateTime.Parse(details.endDate);
            rl.Add(MakePrintString(lineChars, "Close date:", ddt.ToString("MM/dd/yyyy hh:mm tt", dateFormat)));
            report.EndDate = ddt.ToString("MM/dd/yyyy hh:mm tt", dateFormat);

            rl.Add(dash);

            ddt = DateTime.Parse(details.lastReportDate);
            rl.Add(MakePrintString(lineChars, "Last report date:", ddt.ToString("MM/dd/yyyy hh:mm tt", dateFormat)));
            report.LastReportDate = ddt.ToString("MM/dd/yyyy hh:mm tt", dateFormat);

            rl.Add("");
            rl.Add(CenteredText("- Net Department Sales -"));
            rl.Add("");
            rl.Add(ThreeColumnLine("Department", "Qty", "Value"));

            //for loop departments sales
            decimal totQty = 0;
            decimal totVal = 0;
            foreach (CummulativeSales.DeptHistSum x in dhSum)
            {
                decimal sold = x.dept_val_item_sold;
                if (x.Dept_Name == "PRODUCTS")
                {
                    sold += serviceChargeAmt;
                }
                rl.Add(ThreeColumnLine(x.Dept_Name, x.dept_qty_item_sold.ToString("F0"), "P" + sold.ToString("N", new CultureInfo("en-US"))));
                totQty += x.dept_qty_item_sold;
                totVal += sold;
            }

            rl.Add("");
            rl.Add(ThreeColumnLine("TOTAL", totQty.ToString("F0"), "P" + totVal.ToString("N", new CultureInfo("en-US"))));


            #region Breakdown of sales

            rl.Add("");
            rl.Add(CenteredText("- Breakdown of Sales -"));
            rl.Add("");

            decimal sVal = 0;
            decimal vatable = 0, vatamt = 0, nvat = 0, zr = 0;
            decimal ld = 0;
            decimal refundsTotal = refunds.vatable + refunds.vat_exempt + refunds.zero_rated;

            decimal ns = 0;
            decimal nontaxdisc = 0;
            foreach (CummulativeSales.DiscPresetHistSum x in dpSum)
            {
                if (x.preset_id != 2 && x.preset_id != 4)
                {

                    ld += Math.Abs(x.discount_val);
                }
            }

            foreach (CummulativeSales.SaleTotal x in saleTot)
            {
                if (x.taxName == "VAT")
                {
                    var baseamount = x.saleVal / VATRate;
                    var taxamt = x.saleVal - baseamount;
                    vatable = Math.Round(baseamount, 2, MidpointRounding.AwayFromZero);
                    vatamt = Math.Round(taxamt, 2, MidpointRounding.AwayFromZero);
                }
                if (x.taxName == "NVAT")
                {
                    nvat = x.saleVal + serviceChargeAmt;
                }
                if (x.taxName == "Zero Rated")
                {
                    zr = x.saleVal;
                }
                nontaxdisc += decimal.Parse(x.nonTaxDiscValue);
                sVal += x.saleVal;
                sVal -= decimal.Parse(x.nonTaxDiscValue);

            }
            sVal += serviceChargeAmt;
            //foreach (CummulativeSales.NetSales x in netSales)
            foreach (CummulativeSales.SaleTotal x in saleTot)
            {
                if (x.taxName == "VAT")
                {
                    vatable = decimal.Parse((x.saleVal - decimal.Parse(x.nonTaxDiscValue)).ToString("F2")) / VATRate;

                    //vatamt = (x.saleVal - decimal.Parse(x.nonTaxDiscValue)) - (vatable);
                    vatamt = decimal.Parse((x.saleVal - decimal.Parse(x.nonTaxDiscValue)).ToString("F2")) - (vatable);

                    vatable = Math.Round(vatable, 2, MidpointRounding.AwayFromZero); //- x.vatAmount;
                    vatamt = Math.Round(vatamt, 2, MidpointRounding.AwayFromZero);
                }

                if (x.taxName == "NVAT")
                {
                    nvat = x.saleVal + serviceChargeAmt;
                    //if(x.NonVatDiscount > 0)
                    //{
                    //    vatable = vatable - x.NonVatDiscount;
                    //    vatamt = (vatable - (vatable / 1.12M));
                    //    vatable = vatable / 1.12M;

                    //}
                    //else
                    //{
                    //    vatable = vatable - vatamt;
                    //}
                }

                if (x.taxName == "Zero Rated")
                {
                    zr = x.saleVal;
                }
            }
            foreach (CummulativeSales.NetSales x in netSales)
            {
                decimal vatable1 = 0;
                decimal vatamt1 = 0;
                decimal nvat1 = 0;
                decimal zr1 = 0;
                if (x.taxName == "VAT")
                {
                    vatable1 = decimal.Parse((x.saleValue - x.NonVatDiscount).ToString("F2")) / VATRate;

                    vatamt1 = decimal.Parse((x.saleValue - x.NonVatDiscount).ToString("F2")) - (vatable1);

                    vatable1 = Math.Round(vatable1, 2, MidpointRounding.AwayFromZero); //- x.vatAmount;
                    vatamt1 = Math.Round(vatamt1, 2, MidpointRounding.AwayFromZero);
                }

                if (x.taxName == "NVAT")
                {
                    nvat1 = x.saleValue + serviceChargeAmt;
                    //if(x.NonVatDiscount > 0)
                    //{
                    //    vatable = vatable - x.NonVatDiscount;
                    //    vatamt = (vatable - (vatable / 1.12M));
                    //    vatable = vatable / 1.12M;

                    //}
                    //else
                    //{
                    //    vatable = vatable - vatamt;
                    //}
                }

                if (x.taxName == "Zero Rated")
                {
                    zr1 = x.saleValue;
                }
                ns += (vatable1 + vatamt1 + nvat1 + zr1);
            }

            rl.Add(MakePrintString(lineChars, "VATable Sales", "P" + vatable.ToString("N", new CultureInfo("en-US"))));
            rl.Add(MakePrintString(lineChars, "VAT Amount", "P" + vatamt.ToString("N", new CultureInfo("en-US"))));
            rl.Add(MakePrintString(lineChars, "VAT-Exempt Sales", "P" + nvat.ToString("N", new CultureInfo("en-US"))));
            rl.Add(MakePrintString(lineChars, "VAT Zero-Rated Sales", "P" + zr.ToString("N", new CultureInfo("en-US"))));

            rl.Add("");
            rl.Add(dash);
            rl.Add(MakePrintString(lineChars, "Gross Amount", "P" + sVal.ToString(("N"), new CultureInfo("en-US"))));
            rl.Add(MakePrintString(lineChars, "Less Discount", "(" + "P" + ld.ToString(("N"), new CultureInfo("en-US")) + ")"));
            rl.Add(MakePrintString(lineChars, "Less Refund", "P" + refundsTotal.ToString(("N"), new CultureInfo("en-US"))));
            rl.Add(MakePrintString(lineChars, "Net Amount", "P" + ns.ToString(("N"), new CultureInfo("en-US"))));
            rl.Add(dash);


            #endregion

            //#region SCPWDDiscountTotals

            //sVal = 0;
            //vatable = 0;
            //vatamt = 0;
            //nvat = 0;
            //zr = 0;

            //rl.Add("");
            //rl.Add(CenteredText("- SC / PWD Discount Totals -"));
            //rl.Add("");
            //foreach (CummulativeSales.ScpwdDiscTotal x in scdiscTot)
            //{
            //    if (x.taxName == "VAT")
            //    {
            //        vatable = Math.Round(x.scpwdDiscVal / VATRate, 2, MidpointRounding.AwayFromZero);
            //        vatamt = Math.Round(x.scpwdDiscVal - vatable, 2, MidpointRounding.AwayFromZero);
            //    }
            //    if (x.taxName == "NVAT")
            //    {
            //        nvat = x.scpwdDiscVal;
            //    }
            //    if (x.taxName == "Zero Rated")
            //    {
            //        zr = x.scpwdDiscVal;
            //    }
            //    sVal += x.scpwdDiscVal;
            //}

            //rl.Add(MakePrintString(lineChars, "VATable Sales", "P" + vatable.ToString("N", new CultureInfo("en-US"))));
            //rl.Add(MakePrintString(lineChars, "VAT Amount", "P" + vatamt.ToString("N", new CultureInfo("en-US"))));
            //rl.Add(MakePrintString(lineChars, "VAT-Exempt Sales", "P" + nvat.ToString("N", new CultureInfo("en-US"))));
            //rl.Add(MakePrintString(lineChars, "VAT Zero-Rated Sales", "P" + zr.ToString("N", new CultureInfo("en-US"))));

            //rl.Add("");
            //rl.Add(MakePrintString(lineChars, "TOTAL", "P" + sVal.ToString(("N"), new CultureInfo("en-US"))));
            //report.DiscountSeniorPWD = sVal;

            //#endregion

            //#region DiscountTotals

            //sVal = 0;
            //vatable = 0;
            //vatamt = 0;
            //nvat = 0;
            //zr = 0;

            //rl.Add("");
            //rl.Add(CenteredText("- Discount Totals -"));
            //rl.Add("");

            //foreach (CummulativeSales.DiscTotal x in discTot)
            //{
            //    if (x.taxName == "VAT")
            //    {
            //        vatable = Math.Round(x.discSaleVal / VATRate, 2, MidpointRounding.AwayFromZero);
            //        vatamt = Math.Round(x.discSaleVal - vatable, 2, MidpointRounding.AwayFromZero);
            //    }
            //    if (x.taxName == "NVAT")
            //    {
            //        nvat = x.discSaleVal;
            //    }
            //    if (x.taxName == "Zero Rated")
            //    {
            //        zr = x.discSaleVal;
            //    }
            //    sVal += x.discSaleVal;
            //}

            //rl.Add(MakePrintString(lineChars, "VATable Sales", "P" + vatable.ToString("N", new CultureInfo("en-US"))));
            //rl.Add(MakePrintString(lineChars, "VAT Amount", "P" + vatamt.ToString("N", new CultureInfo("en-US"))));
            //rl.Add(MakePrintString(lineChars, "VAT-Exempt Sales", "P" + nvat.ToString("N", new CultureInfo("en-US"))));
            //rl.Add(MakePrintString(lineChars, "VAT Zero-Rated Sales", "P" + zr.ToString("N", new CultureInfo("en-US"))));

            //rl.Add("");
            //rl.Add(MakePrintString(lineChars, "TOTAL", "P" + sVal.ToString(("N"), new CultureInfo("en-US"))));
            //report.DiscountOther = sVal - report.DiscountSeniorPWD;


            //#endregion

            //#region RefundTotals



            //rl.Add("");
            //rl.Add(CenteredText("- Refund Totals -"));
            //rl.Add("");

            //decimal refundsBase = refunds.vatable / VATRate;
            //decimal refundsTax = refunds.vatable - refundsBase;
            //decimal refundsTotal = refunds.vatable + refunds.vat_exempt + refunds.zero_rated;

            //rl.Add(MakePrintString(lineChars, "VATable Sales", "(P" + refundsBase.ToString(("N"), new CultureInfo("en-US")) + ")"));
            //rl.Add(MakePrintString(lineChars, "VAT Amount", "(P" + refundsTax.ToString(("N"), new CultureInfo("en-US")) + ")"));
            //rl.Add(MakePrintString(lineChars, "VAT-Exempt Sales", "(P" + refunds.vat_exempt.ToString(("N"), new CultureInfo("en-US")) + ")"));
            //rl.Add(MakePrintString(lineChars, "VAT Zero-Rated Sales", "(P" + refunds.zero_rated.ToString(("N"), new CultureInfo("en-US")) + ")"));
            //rl.Add("");
            //rl.Add(MakePrintString(lineChars, "TOTAL", "(P" + refundsTotal.ToString(("N"), new CultureInfo("en-US")) + ")"));
            //report.RefundTotal = refundsTotal;

            //#endregion

            //#region TotalNetSales

            //sVal = 0;
            //vatable = 0;
            //vatamt = 0;
            //nvat = 0;
            //zr = 0;

            //rl.Add("");
            //rl.Add(CenteredText("- Total Net Sales -"));
            //rl.Add("");
            //foreach (CummulativeSales.NetSales x in netSales)
            //{
            //    if (x.taxName == "VAT")
            //    {
            //        vatable = (x.saleValue - x.NonVatDiscount) / VATRate;

            //        vatamt = (x.saleValue - x.NonVatDiscount) - (vatable);

            //        vatable = Math.Round(vatable, 2, MidpointRounding.AwayFromZero); //- x.vatAmount;
            //        vatamt = Math.Round(vatamt, 2, MidpointRounding.AwayFromZero);
            //    }

            //    if (x.taxName == "NVAT")
            //    {
            //        nvat = x.saleValue;
            //        //if(x.NonVatDiscount > 0)
            //        //{
            //        //    vatable = vatable - x.NonVatDiscount;
            //        //    vatamt = (vatable - (vatable / 1.12M));
            //        //    vatable = vatable / 1.12M;

            //        //}
            //        //else
            //        //{
            //        //    vatable = vatable - vatamt;
            //        //}
            //    }

            //    if (x.taxName == "Zero Rated")
            //    {
            //        zr = x.saleValue;
            //    }
            //}
            //sVal = vatable + vatamt + nvat + zr;

            ////vatable = Math.Round(vatable, 2, MidpointRounding.AwayFromZero);
            ////vatamt = Math.Round(vatamt, 2, MidpointRounding.AwayFromZero);

            //rl.Add(MakePrintString(lineChars, "VATable Sales", "P" + vatable.ToString("N", new CultureInfo("en-US"))));
            //report.NetVatableSales = vatable;

            //rl.Add(MakePrintString(lineChars, "VAT Amount", "P" + vatamt.ToString("N", new CultureInfo("en-US"))));
            //report.NetVatAmount = vatamt;

            //rl.Add(MakePrintString(lineChars, "VAT-Exempt Sales", "P" + nvat.ToString("N", new CultureInfo("en-US"))));
            //report.NetExemptSales = nvat;

            //rl.Add(MakePrintString(lineChars, "VAT Zero-Rated Sales", "P" + zr.ToString("N", new CultureInfo("en-US"))));
            //report.NetZeroRated = zr;

            //rl.Add("");
            //rl.Add(MakePrintString(lineChars, "TOTAL", "P" + sVal.ToString(("N"), new CultureInfo("en-US"))));

            ////#endregion

            #region DiscountSummary
            rl.Add("");
            rl.Add(CenteredText("- Discount Summary -"));
            rl.Add("");
            foreach (CummulativeSales.DiscPresetHistSum x in dpSum)
            {
                //rl.Add(MakePrintString(lineChars, x.preset_name, "P" + x.discount_val.ToString("N", new CultureInfo("en-US"))));
                decimal discamt = Math.Abs(x.discount_val);
                rl.Add(MakePrintString(lineChars, x.preset_name, "(" + "P" + discamt.ToString(("N"), new CultureInfo("en-US")) + ")"));


            }
            #endregion
            #region MOPTotal
            sVal = 0;

            rl.Add("");
            rl.Add(CenteredText("- MOP Totals -"));
            rl.Add("");

            //forloop
            foreach (CummulativeSales.MopHistSum x in mhSum)
            {
                rl.Add(MakePrintString(lineChars, x.Mop_Name, "P" + x.Mop_Net_Sale_Value.ToString("N", new CultureInfo("en-US"))));
                sVal += x.Mop_Net_Sale_Value;
            }

            rl.Add(MakePrintString(lineChars, "TOTAL", "P" + sVal.ToString("N", new CultureInfo("en-US"))));

            #endregion

            #region GrandTotalSales

            rl.Add("");
            rl.Add(CenteredText("- GRAND TOTAL SALES -"));
            rl.Add("");

            rl.Add(MakePrintString(lineChars, "Beginning Balance", "P" + details.Previous_Accumulating_Total_Sales.ToString("N", new CultureInfo("en-US"))));
            report.PreviousNetSales = details.Previous_Accumulating_Total_Sales;

            rl.Add(MakePrintString(lineChars, "Ending Balance", "P" + details.Current_Accumulating_Total_Sales.ToString("N", new CultureInfo("en-US"))));
            report.CurrentNetSales = details.Current_Accumulating_Total_Sales;
            report.DifferenceNetSales = report.CurrentNetSales - report.PreviousNetSales;

            #endregion

            rl.Add("");
            rl.Add(CenteredText("- Transactions -"));
            rl.Add("");

            if (appConfig["POS"]["UseResetter"] == "Y")
            {
                string rctType = appConfig["POS"]["RecieptType"];

                string parsedStart = details.begSalesResetter.PadLeft(3, '0') + "-" + details.begSalesInvoice.PadLeft(9, '0');
                string parsedEnd = details.endSalesResetter.PadLeft(3, '0') + "-" + details.endSalesInvoice.PadLeft(9, '0');

                if (parsedStart == parsedEnd)
                {
                    rl.Add("NO ACTIVITY");
                    report.StartTransNumber = "NO ACTIVITY";
                    report.EndTransNumber = "NO ACTIVITY";
                }
                else
                {
                    strPrintData = MakePrintString(lineChars, "Beginning Sales Invoice", parsedStart);
                    report.StartTransNumber = parsedStart;
                    rl.Add(strPrintData);

                    strPrintData = MakePrintString(lineChars, "Ending Sales Invoice", parsedEnd);
                    report.EndTransNumber = parsedEnd;
                    rl.Add(strPrintData);
                }
            }
            else
            {

                if (details.begSalesInvoice == details.endSalesInvoice)
                {
                    rl.Add("NO ACTIVITY");
                    report.StartTransNumber = "NO ACTIVITY";
                    report.EndTransNumber = "NO ACTIVITY";
                }
                else
                {
                    rl.Add(MakePrintString(lineChars, "Beginning Sales Invoice", details.begSalesInvoice));
                    report.StartTransNumber = details.begSalesInvoice;

                    rl.Add(MakePrintString(lineChars, "Ending Sales Invoice", details.endSalesInvoice + serviceChargeAmt));
                    report.EndTransNumber = details.endSalesInvoice;
                }
            }

            rl.Add("");
            rl.Add(MakePrintString(lineChars, "Z-Counter", details.ZCounter.ToString()));
            report.ZCounter = details.ZCounter;

            rl.Add("");
            rl.Add(MakePrintString(lineChars, "ACCUMULATING GRAND TOTAL", "P" + (details.Current_Accumulating_Total_Sales + serviceChargeAmt).ToString("N", new CultureInfo("en-US"))));

            string fileName = "ACCUMULATING GRAND TOTAL SALES REPORT " + strDate2 + ".txt";

            rl.Add("");
            Footers(out string[] f1, out string[] f2, out string[] f3, out string[] f4, out string[] f5);
            foreach (string ln in f2)
                rl.Add(CenteredText(ln.Trim()));
            rl.Add("");

            foreach (string ln in f4)
                rl.Add(CenteredText(ln.Trim()));
            rl.Add("");

            string dataToPrintLog = "";
            foreach (string line in rl)
            {
                dataToPrintLog += $"{line}\n";
            }
            SaveToPrintLog(dataToPrintLog);

            string path = "reports\\" + fileName;
            fileIO.CreateTextFile(path, rl);
            await PrintText(rl);
            if (appConfig["APP"]["AutoOpenTextFiles"] == "true")
                Process.Start(path);

            //   Var_Dump(report);


            JournalDB jb = new JournalDB(loggedInPOSID);
            jb.InsertReport(report);

        }

        private async void btn_ejournal_consolidator_Click(object sender, EventArgs e)
        {
            if (activeCashier.bof != 1)
            {
                MessageBox.Show("User is not allowed to access E-Journal Consolidator.");
                return;
            }
            EJournalConsolidator ejc = new EJournalConsolidator(this);

            if (ejc.ShowDialog() == DialogResult.OK)
            {

                //List<string>ejcData =new List<string>(ejc.linesb);
                //for(int i = 0; i <= ejcData.Length; i++)
                //{
                //    Console.WriteLine("print Start");
                //}
                //PrintText(Lines);
                if (MessageBox.Show("Do you want to print consolidated receipt?", "Notice!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    await PrintText(ejc.linesb);
                }
            }
        }

        private void btn_print_cashdraw_Click(object sender, EventArgs e)
        {
            if (activeCashier.printCDraw != 1)
            {
                MessageBox.Show("User not allowed to print cash draw");
                return;

            }
            var respo = api.CashDrawList();

            if (respo != "false")
            {
                cdList = new List<CashDrawList>();
                cdList = JsonConvert.DeserializeObject<List<CashDrawList>>(respo);
                Console.WriteLine(cdList.Count + "List COunt ===================================================");

                cdldlg = new CashDrawListForm(loggedInPOSID);
                cdldlg.cdlist = cdList;

                string res;
                if (cdldlg.ShowDialog() == DialogResult.OK)
                {
                    res = cdldlg.cashdrawData;
                    ProcessCashDraw(res, true);
                    enb.LogPOSEvent($"Cashier Print Report:{activeCashier.name}, Print Cashdraw", 10, loggedInPOSID, activeCashier.name);

                }
            }
        }

        private void btn_print_shift_Click(object sender, EventArgs e)
        {
            if (activeCashier.printShift != 1)
            {
                MessageBox.Show("User not allowed to print shift");
                return;
            }
            periodLists = api.GetPeriodList(1);
            if (periodLists == null)
            {
                MessageBox.Show("Unable to get shift list", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            shiftdlg = new ShiftList(loggedInPOSID);
            shiftdlg.formTitle = "Print Shift Report";
            shiftdlg.pl = periodLists;
            shiftdlg.posID = cfg.POS_ID;

            if (shiftdlg.ShowDialog() == DialogResult.OK)
            {
                var data = shiftdlg.periodData;
                ProcessReports(data, "SHIFT REPORT", 1, true);
                enb.LogPOSEvent($"Cashier Print Report:{activeCashier.name}, Shift Report", 10, loggedInPOSID, activeCashier.name);

            }
        }

        private void btn_print_day_Click(object sender, EventArgs e)
        {
            if (activeCashier.printDay != 1)
            {
                MessageBox.Show("User not allowed to print day report");
                return;
            }
            periodLists = api.GetPeriodList(2);
            shiftdlg = new ShiftList(loggedInPOSID);
            shiftdlg.formTitle = "Print Day Report";
            shiftdlg.pl = periodLists;
            shiftdlg.posID = cfg.POS_ID;
            if (shiftdlg.ShowDialog() == DialogResult.OK)
            {
                var data = shiftdlg.periodData;
                ProcessReports(data, "DAY REPORT", 1, true);
                enb.LogPOSEvent($"Cashier Print Report:{activeCashier.name}, Day Report", 10, loggedInPOSID, activeCashier.name);

            }
        }

        private void btn_print_month_Click(object sender, EventArgs e)
        {
            if (activeCashier.printMonth != 1)
            {
                MessageBox.Show("User not allowed to print month");
                return;
            }
            periodLists = api.GetPeriodList(3);
            shiftdlg = new ShiftList(loggedInPOSID);
            shiftdlg.formTitle = "Print Month Report";
            shiftdlg.pl = periodLists;
            shiftdlg.posID = cfg.POS_ID;
            if (shiftdlg.ShowDialog() == DialogResult.OK)
            {
                var data = shiftdlg.periodData;
                ProcessReports(data, "MONTH REPORT", 1, true);
                enb.LogPOSEvent($"Cashier Print Report:{activeCashier.name}, Month Report", 10, loggedInPOSID, activeCashier.name);

            }
        }

        private void btn_generate_csv_Click(object sender, EventArgs e)
        {
            if (activeCashier.bof != 1)
            {
                MessageBox.Show("User is not allowed to generate csv.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var boff = new BOF(loggedInPOSID);
            boff.ShowDialog();
        }

        private void btn_generate_accu_csv_Click(object sender, EventArgs e)
        {
            if (activeCashier.bof != 1)
            {
                MessageBox.Show("User is not allowed to generate csv file.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show("This will generate Accumulating Grand Total Sales CSV file.", "Proceed?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                return;
            var data = GenerateData();

            if (FileIO.WriteToFile(data) == true)
            {
                MessageBox.Show("Export done!");
            }
            else
            {
                MessageBox.Show("Export failed!");
            }
        }

        private List<string> GenerateData()
        {
            List<string> lines = new List<string>();
            EnbDB enb = new EnbDB(loggedInPOSID);
            var head = enb.GetHeaders();
            foreach (EnbDB.CSVHeader x in head)
            {
                var splitH = Regex.Split(x.Data.Trim(), @"\\n");
                foreach (string y in splitH)
                    lines.Add(y);
                lines.Add(x.Description);
                lines.Add($"SN: {x.SerialNo}");
                lines.Add($"MIN: {x.POS_Min}");
                lines.Add("");
                lines.Add("Accumulating Grand Total Sales");
                lines.Add("");


                lines.Add("PosID,LastReportDate,StartDate,EndDate,StartTransNumber,EndTransNumber,PreviouAccumulatingTotalNetSales,CurrentAccumulatingTotalNetSales,DifferenceAccumulatingTotalNetSales,SnrPWDDiscounts,OtherDiscounts,TotalRefunds,Shortages,Overages,NetVatableSales,NetVatAmount,NetExemptSales,NetZeroRatedSales,ZCounter");
                try
                {
                    var result = enb.GetAccumulatingData(x.POS_ID);
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

        private void btn_pos_config_Click(object sender, EventArgs e)
        {
            if (activeCashier.bof != 1)
            {
                MessageBox.Show("User is not allowed to access POS config.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
           // string posConfigPath = $"posconfig{loggedInPOSID}";
            string exePath = $"POSCONFIG{loggedInPOSID}\\posconfig.exe";

            StartProcess(exePath, $"POSCONFIG{loggedInPOSID}");

            /*// Create a new process
            Process process = new Process();
            process.StartInfo.FileName = exePath;

            try
            {
                // Start the process
                process.Start();

                process.WaitForExit();

                int exitCode = process.ExitCode;
                Console.WriteLine($"Process exited with code: {exitCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }*/

            enb.LogPOSEvent($"Cashier Open Config:{activeCashier.name}", 9, loggedInPOSID, activeCashier.name);
        }

        private void ReportsForm_load(object sender, EventArgs e)
        {
            string formNumber = this.Text + " - POS" + loggedInPOSID.ToString();
            this.Text = formNumber;
        }
    }
}
