using IniParser.Model;
using IniParser;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;

namespace VenusRetailPOSServer
{
    public class API
    {
        public string url;

       // public Pumpdemo main;
       // public List<MOP> mop;
        public List<Tax> tax;
        //public List<GRADES> grade;
        public RECEIPT receipt;
        public CONFIG config;
        public Cashier cashier;
        public List<Cashier> chr;
        //public List<Products> product;
      //  public Products singlePrd;
      //  public List<dbItemTypes> types;
        //public ApiTransaction trans;
        public string periodID;
        public Response res = new Response();
        public Reports reports = new Reports();
        public List<PeriodList> pl = new List<PeriodList>();
        public List<VehicleType> vehicles = new List<VehicleType>();
       // public List<Products> plu = new List<Products>();
      //  public List<PROFILES> priceProfile = new List<PROFILES>();
      //  public List<Discount> discounts = new List<Discount>();
        public FileIO fileIO = new FileIO();

        public bool showDebug;
        int GLO_POSID;


       
        public API(int posID)
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("config.ini");
            string apiUsed = $"API{posID}";
            url = data[apiUsed]["Url"];
            showDebug = Boolean.Parse(data["APP"]["Debug"]);
            //GLO_POSID = Int32.Parse(data["POS"]["POSID"]);
            GLO_POSID = posID;
            Console.WriteLine(showDebug);
        }

        private void DisplayJSONString(string json, string caller)
        {
            if (showDebug == true)
            {
                MessageBox.Show(json, caller);
            }
        }
        private void DisplayJSONString(string json)
        {
            if (showDebug == true)
            {
                MessageBox.Show(json);
            }
        }


        private void LogJSON(string json, string caller)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = null;
            filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\API_LOG-" + DateTime.Now.Date.ToString("MMMM yyyy") + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(DateTime.Now.ToString());
                    sw.WriteLine("Caller : " + caller);
                    sw.WriteLine("Content : " + json);
                    sw.WriteLine(new string('-', 50));
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(DateTime.Now.ToString());
                    sw.WriteLine("Caller : " + caller);
                    sw.WriteLine("Content : " + json);
                    sw.WriteLine(new string('-', 50));
                }
            }
        }
        private void LogRequest(List<string> lines)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = null;
            filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\REQUEST_LOG-" + DateTime.Now.Date.ToString("MMMM yyyy") + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(DateTime.Now.ToString());
                    foreach (string x in lines)
                        sw.WriteLine(x);
                    sw.WriteLine(new string('-', 50));
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(DateTime.Now.ToString());
                    foreach (string x in lines)
                        sw.WriteLine(x);
                    sw.WriteLine(new string('-', 50));
                }
            }
        }
        public List<Cashier> chrAuth(string id, string pw)
        {
            var client = new RestClient(url + "cashierCtrl/loginWithInnerJoin");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("number", id);
            request.AddParameter("password", pw);


            IRestResponse response = client.Execute(request);
            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
            res = deserializer.Deserialize<Response>(response);
            if (res.statusCode == 1)
            {
                var apiResponse = deserializer.Deserialize<Dictionary<string, string>>(response);
                var data = apiResponse["data"].ToString();
                response.Content = data;

                chr = deserializer.Deserialize<List<Cashier>>(response);
                Console.WriteLine(data);
            }

            return chr;
        }
       
        public string GetPeriod(int cashierID)
        {
            var client = new RestClient(url + "lookUpCtrl/getCshrActShiftPeriod");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("cashierID", cashierID);
            IRestResponse response = client.Execute(request);

            LogJSON(response.Content, "GetPeriod()");
            DisplayJSONString(response.Content, "lookUpCtrl/getCshrActShiftPeriod");
            //API bug, remove if necessary
            response.Content = response.Content.TrimStart('q');

            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
            res = deserializer.Deserialize<Response>(response);
            if (res.statusCode == 1)
            {
                //var apiResponse = deserializer.Deserialize<Dictionary<string, string>>(response);
                //periodID = apiResponse["data"].ToString();
                Console.WriteLine("Period Data {0}", res.data);
                return res.data;
            }
            else
            {
                return "false";
            }
        }
        public string GetSystemStatus()
        {
            //return "true";
            var client = new RestClient(url + "lookUpCtrl/systemStatus")
            {

                Timeout = 1000
            };

            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            response.Content = response.Content.TrimStart('q');

            try
            {
                RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
                res = deserializer.Deserialize<Response>(response);

                if (res.statusCode == 1)
                {
                    return "true";
                }
                return "false";
            }
            catch (Exception e)
            {
                return "false";
            }
        }
        public List<Tax> GetTaxList()
        {
            var client = new RestClient(url + "lookUpCtrl/getTaxList");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            IRestResponse response = client.Execute(request);

            LogJSON(response.Content, "GetTaxList()");
            DisplayJSONString(response.Content, "lookUpCtrl/getTaxList");
            //API bug, remove if necessary
            response.Content = response.Content.TrimStart('q');


            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
            res = deserializer.Deserialize<Response>(response);
            if (res.statusCode == 1)
            {
                response.Content = res.data;
                tax = deserializer.Deserialize<List<Tax>>(response);
            }

            return tax;
        }
       
        public RECEIPT GetReceipt()
        {
            var client = new RestClient(url + "lookUpCtrl/getReceiptLayout");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("posID", GLO_POSID);
            IRestResponse response = client.Execute(request);

            LogJSON(response.Content, "GetReciept()");
            DisplayJSONString(response.Content, "lookUpCtrl/getReceiptLayout");
            //API bug, remove if necessary
            response.Content = response.Content.TrimStart('q');

            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
            res = deserializer.Deserialize<Response>(response);
            if (res.statusCode == 1)
            {

                response.Content = res.data;
                receipt = deserializer.Deserialize<RECEIPT>(response);
            }

            return receipt;
        }
        public CONFIG GetConfig()
        {
            var client = new RestClient(url + "lookUpCtrl/getPOSTerminal");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("posID", GLO_POSID);
            IRestResponse response = client.Execute(request);

            LogJSON(response.Content, "GetConfig()");
            DisplayJSONString(response.Content, "lookUpCtrl/getPOSTerminal");
            //API bug, remove if necessary
            response.Content = response.Content.TrimStart('q');

            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
            res = deserializer.Deserialize<Response>(response);
            if (res.statusCode == 1)
            {
                response.Content = res.data;
                config = deserializer.Deserialize<CONFIG>(response);
            }
            return config;
        }

        

        public string GetCustomerAccount(string accNum)
        {
            var client = new RestClient(url + "lookUpCtrl/getSubAccDetails");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("subAccNum", accNum);
            IRestResponse response = client.Execute(request);

            LogJSON(response.Content, "GetCustomerAccount()");
            DisplayJSONString(response.Content, "lookUpCtrl/getSubAccDetails");
            //API bug, remove if necessary
            response.Content = response.Content.TrimStart('q');


            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
            res = deserializer.Deserialize<Response>(response);
            if (res.statusCode == 1)
            {
                return res.data;
            }
            else
            {
                return "false";
            }
        }
        public bool CheckCash(int cashierID, decimal amount)
        {
            var client = new RestClient(url + "cashdrawCtrl/safeDropAmtChecker");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("amount", amount);
            request.AddParameter("cashierID", cashierID);
            request.AddParameter("posID", GLO_POSID);
            IRestResponse response = client.Execute(request);

            LogJSON(response.Content, "CheckCash()");
            DisplayJSONString(response.Content, "cashdrawCtrl/safeDropAmtChecker");
            //MessageBox.Show(response.Content);
            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
            res = deserializer.Deserialize<Response>(response);

            if (res.statusCode == 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        
        public List<PeriodList> GetPeriodList(int periodType)
        {
            int posID = GLO_POSID;

            var client = new RestClient(url + "reportsCtrl/getAllPeriod");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("type", periodType);
            request.AddParameter("posID", posID);
            IRestResponse response = client.Execute(request);

            LogJSON(response.Content, "GetPeriodList()");
            DisplayJSONString(response.Content, "reportsCtrl/getAllPeriod");
            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();

            try
            {
                pl = deserializer.Deserialize<List<PeriodList>>(response);
                Console.WriteLine("PERIOD COUNTS ======================" + pl.Count);
                return pl;
            }
            catch (Exception e)
            {
                MessageBox.Show(response.Content, e.Message);
                return null;
            }

        }

        public string GetPeriodReport(int periodID, int posID, int state)
        {
            var client = new RestClient(url + "reportsCtrl/getPeriodReport");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("periodID", periodID);
            request.AddParameter("posID", posID);
            request.AddParameter("state", state);
            IRestResponse response = client.Execute(request);

            LogJSON(response.Content, "GetPeriodReport()");
            DisplayJSONString(response.Content, "reportsCtrl/getPeriodReport");
            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();

            try
            {
                res = deserializer.Deserialize<Response>(response);
                if (res.statusCode == 1)
                {
                    return res.data;
                }
                else
                {
                    return "false";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(response.Content, e.Message);
                return "false";
            }


        }

        public string CashDrawList()
        {
            int posID = GLO_POSID;

            var client = new RestClient(url + "reportsCtrl/getAllCashDrawByPosID");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("posID", posID);
            IRestResponse response = client.Execute(request);

            LogJSON(response.Content, "CashDrawList()");
            DisplayJSONString(response.Content, "reportsCtrl/getAllCashDrawByPosID");
            try
            {
                RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
                res = deserializer.Deserialize<Response>(response);
                if (res.statusCode == 1)
                {
                    return res.data;
                }
                else
                {
                    return "false";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to get Cashdraw list" + Environment.NewLine + "Please try again.", "", MessageBoxButtons.OK);
                return "false";
            }


        }
        public string GetCashDrawReport(string cdpID)
        {
            var client = new RestClient(url + "reportsCtrl/getCDrawHistByCDPeriod");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("cdrawPeriodID", cdpID);
            IRestResponse response = client.Execute(request);

            LogJSON(response.Content, "GetCashDrawReport()");
            DisplayJSONString(response.Content, "reportsCtrl/getCDrawHistByCDPeriod");
            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
            try
            {
                res = deserializer.Deserialize<Response>(response);
                if (res.statusCode == 1)
                {
                    return res.data;
                }
                else
                {
                    return "false";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Try Again");
                return "false";
            }

        }

        public string GetCummulativeSale()
        {
            int posID = GLO_POSID;
            Console.WriteLine("POSID " + posID.ToString());
            var client = new RestClient(url + "reportsCtrl/getCumulativeSales");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("posID", posID);
            IRestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);

            LogJSON(response.Content, "GetCummulativeSale()");
            DisplayJSONString(response.Content, "reportsCtrl/getCumulativeSales");
            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();

            res = deserializer.Deserialize<Response>(response);

            if (res.statusCode == 1)
            {
                return res.data;
            }
            else
            {
                return "false";
            }

        }
       

        public string closePeriod(int periodType)
        {
            var client = new RestClient(url + "/periodCtrl/closePeriod");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("periodType", periodType);
            request.AddParameter("posID", GLO_POSID);
            IRestResponse response = client.Execute(request);


            LogJSON(response.Content, "ClosePeriod()");
            DisplayJSONString(response.Content, "/periodCtrl/closePeriod");
            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();

            try
            {
                res = deserializer.Deserialize<Response>(response);
                Console.WriteLine(res);
                Console.WriteLine("closeperiod");
                if (res.statusCode == 1)
                {
                    return res.data;
                }
                else
                {
                    MessageBox.Show(res.statusDescription, "Unable to close period", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return "false";
                }
            }
            catch
            {
                MessageBox.Show(response.Content);
                return "false";
            }

        }

        public string CloseCashDraw(int cashierID)
        {
            var client = new RestClient(url + "/periodCtrl/closeCDrawPeriod");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("cashierID", cashierID);
            request.AddParameter("posID", GLO_POSID);
            IRestResponse response = client.Execute(request);

            LogJSON(response.Content, "CloseCashDraw()");
            DisplayJSONString(response.Content, "/periodCtrl/closeCDrawPeriod");
            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
            try
            {
                res = deserializer.Deserialize<Response>(response);
                if (res.statusCode == 1)
                {
                    return res.data;
                }
                else
                {
                    return "false";
                }
            }
            catch
            {
                MessageBox.Show(response.Content);
                return "false";
            }

        }

        public string GetTransactionsByDate(string dateStr)
        {
            var client = new RestClient(url + "eJournalCtrl/getTransJournalByDate");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("dateTo", dateStr);
            request.AddParameter("posID", GLO_POSID);

            IRestResponse response = client.Execute(request);

            LogJSON(response.Content, "GetTransactionByDate()");
            DisplayJSONString(response.Content, "eJournalCtrl/getTransJournalByDate");
            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
            try
            {
                res = deserializer.Deserialize<Response>(response);
                if (res.statusCode == 1)
                {
                    return res.data;
                }
                else
                {
                    return "false";
                }
            }
            catch
            {
                MessageBox.Show(response.Content);
                return "false";
            }
        }

        /*public string GetTransactionData(string transNum)
        {
            var client = new RestClient(url + "TransactionCtrl/getTransactionData");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("birTransNum", transNum);

            IRestResponse response = client.Execute(request);

            LogJSON(response.Content, "GetTransactionByDate()");
            DisplayJSONString(response.Content, "TransactionCtrl/getTransactionData");

            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
            try
            {
                res = deserializer.Deserialize<Response>(response);
                if (res.statusCode == 1)
                {
                    return res.data;
                }
                else
                {
                    return "false";
                }
            }
            catch
            {
                FlexibleMessageBox.Show(response.Content);
                return "false";
            }
        }
        public string GetTransactionRefundData(string transNum, string resetter, string posid)
        {
            var client = new RestClient(url + "TransactionCtrl/getTransactionData");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("birTransNum", transNum);
            request.AddParameter("transactionResetter", resetter);
            request.AddParameter("posID", posid);
            IRestResponse response = client.Execute(request);

            LogJSON(response.Content, "GetTransactionByDate()");
            DisplayJSONString(response.Content, "TransactionCtrl/getTransactionData");
            Console.WriteLine(response.Content);
            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
            try
            {
                res = deserializer.Deserialize<Response>(response);
                if (res.statusCode == 1)
                {
                    return res.data;
                }
                else
                {
                    return "false";
                }
            }
            catch
            {
                FlexibleMessageBox.Show(response.Content);
                return "false";
            }
        }*/


       

        public void RegisterVoid(int cashierID, decimal amount)
        {
            var client = new RestClient(url + "/TransactionCtrl/registerVoid");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("cashier_id", cashierID);
            request.AddParameter("amount", amount);

            IRestResponse response = client.Execute(request);
        }
        public void RegisterNoSale(int cashierID)
        {
            var client = new RestClient(url + "/TransactionCtrl/registerNoSale");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("cashier_id", cashierID);

            IRestResponse response = client.Execute(request);
        }




        public string CheckCloseShiftTodayStatus(int periodType)
        {
            var client = new RestClient(url + "/lookUpCtrl/CheckCloseDateToday");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("periodType", periodType);


            IRestResponse response = client.Execute(request);
            LogJSON(response.Content, "CheckCloseShiftTodayStatus()");
            DisplayJSONString(response.Content, "lookUpCtrl/CheckCloseDateToday");
            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();

            try
            {
                res = deserializer.Deserialize<Response>(response);
                Console.WriteLine($"STATUS IS {res.statusCode}, MESSAGE IS {res.statusDescription}");
                if (res.statusCode == 1)
                {
                    return res.data;
                }

            }
            catch
            {
                //MessageBox.Show(response.Content);
                return "continue";
            }


            //MessageBox.Show(res.data, "Unable to close period", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return "continue";

        }

        public string GetVoidRefundData(string startDate, string endDate)
        {
            //Console.WriteLine("GetVoidRefundData from[{0}] to[{1}]", startDate, endDate);

            var client = new RestClient(url + "/reportsCtrl/getVoidRefundReport");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("dateFrom", startDate);
            request.AddParameter("dateTo", endDate);
            request.AddParameter("posID", GLO_POSID);

            IRestResponse response = client.Execute(request);
            LogJSON(response.Content, "GetVoidRefundData()");
            DisplayJSONString(response.Content, "reportsCtrl/getVoidRefundReport");
            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();

            try
            {
                res = deserializer.Deserialize<Response>(response);
                if (res.statusCode == 1)
                {
                    return res.data;
                }
                else
                {
                    //MessageBox.Show(res.data, "Unable to close period", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return "false";
                }
            }
            catch
            {
                MessageBox.Show(response.Content);
                return "false";
            }
        }

        public string GetPeriodReport(int periodID, int posID)
        {
            var client = new RestClient(url + "reportsCtrl/getPeriodReport");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("periodID", periodID);
            request.AddParameter("posID", posID);
            //request.AddParameter("state", state);
            IRestResponse response = client.Execute(request);

            LogJSON(response.Content, "GetPeriodReport()");
            DisplayJSONString(response.Content, "reportsCtrl/getPeriodReport");
            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();

            try
            {
                res = deserializer.Deserialize<Response>(response);
                if (res.statusCode == 1)
                {
                    return res.data;
                }
                else
                {
                    return "false";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(response.Content, e.Message);
                return "false";
            }


        }


    }

}
