using IniParser.Model;
using IniParser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VenusRetailPOSServer
{
    public class JournalDB
    {
        public List<string> rl = new List<string>();
        public IniData config;
        private int posID = 0;

        public JournalDB(int posNum)
        {
            var parser = new FileIniDataParser();
            config = parser.ReadFile("db.ini");
            posID = posNum;
        }
        private string CreateConnectionString()
        {
            string journal = $"JOURNAL{posID}";
            string server = config[journal]["Host"];
            string user = config[journal]["Username"];
            string pass = config[journal]["Password"];
            string db = config[journal]["Database"];
            //return "Driver={SQL Server};Server=" + server + ";UID=" + user + ";PWD=" + pass + ";Database=" + db + ";";
            return "Server = " + server + "; Database =" + db + "; User Id =" + user + ";Password = " + pass + ";";
        }

        public string GetReciept(int pos_id, string ornumber)
        {
            string connStr = CreateConnectionString();
            DataSet ds = new DataSet();

            using (OdbcConnection connection = new OdbcConnection(connStr))
            {
                string querystr = "SELECT data as Data from electric_journal where pos_id =" + pos_id + " AND si_number = \'" + ornumber + "\'";

                OdbcDataAdapter adapter = new OdbcDataAdapter(querystr, connStr);

                try
                {
                    connection.Open();
                    adapter.Fill(ds);
                    string json = JsonConvert.SerializeObject(ds, Newtonsoft.Json.Formatting.Indented);
                    return json;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return "false";
                }


            }
        }
        public string GetRCTString(int pos_id, string ornumber)
        {
            string conn = CreateConnectionString();

            string result = "";

            using (SqlConnection con = new SqlConnection(conn))
            {
                string querystr = "SELECT data as Data from electric_journal where pos_id = @posid AND si_number = @ornum";

                using (SqlCommand cmd = new SqlCommand(querystr, con))
                {

                    cmd.Parameters.Add("@posid", SqlDbType.Int).Value = pos_id;
                    cmd.Parameters.Add("@ornum", SqlDbType.Char).Value = ornumber;

                    con.Open();
                    result = (string)cmd.ExecuteScalar();
                }
            }
            return result;
        }

        public int InsertReport(AccumulatingReport report)
        {
            string conn = CreateConnectionString();

            using (SqlConnection con = new SqlConnection(conn))
            {
                string querystr = "INSERT INTO Accumulating_Report(POS_ID, LastReportDate, StartDate, EndDate, Start_TransNumber, End_TransNumber, Previous_AccNetSales, Current_AccNetSales, Difference_AccNetSales, Discount_SeniorPWD, Discount_Other, Refund_Total, Shortage, Overage, Net_VatableSales, Net_VatAmount, Net_ExemptSales, Net_ZeroRated, Z_Counter) " +
                    "VALUES(@param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9, @param10, @param11, @param12, @param13, @param14, @param15, @param16, @param17, @param18, @param19)";

                using (SqlCommand cmd = new SqlCommand(querystr, con))
                {
                    cmd.Parameters.AddWithValue("@param1", report.PosID);
                    cmd.Parameters.Add("@param2", SqlDbType.DateTime2).Value = report.LastReportDate;
                    cmd.Parameters.Add("@param3", SqlDbType.DateTime2).Value = report.StartDate;
                    cmd.Parameters.Add("@param4", SqlDbType.DateTime2).Value = report.EndDate;
                    //cmd.Parameters.AddWithValue("@param2", report.LastReportDate);
                    //cmd.Parameters.AddWithValue("@param3", report.StartDate);
                    //cmd.Parameters.AddWithValue("@param4", report.EndDate);
                    cmd.Parameters.AddWithValue("@param5", report.StartTransNumber);
                    cmd.Parameters.AddWithValue("@param6", report.EndTransNumber);
                    cmd.Parameters.AddWithValue("@param7", report.PreviousNetSales);
                    cmd.Parameters.AddWithValue("@param8", report.CurrentNetSales);
                    cmd.Parameters.AddWithValue("@param9", report.DifferenceNetSales);
                    cmd.Parameters.AddWithValue("@param10", report.DiscountSeniorPWD);
                    cmd.Parameters.AddWithValue("@param11", report.DiscountOther);
                    cmd.Parameters.AddWithValue("@param12", report.RefundTotal);
                    cmd.Parameters.AddWithValue("@param13", report.Shortage);
                    cmd.Parameters.AddWithValue("@param14", report.Overage);
                    cmd.Parameters.AddWithValue("@param15", report.NetVatableSales);
                    cmd.Parameters.AddWithValue("@param16", report.NetVatAmount);
                    cmd.Parameters.AddWithValue("@param17", report.NetExemptSales);
                    cmd.Parameters.AddWithValue("@param18", report.NetZeroRated);
                    cmd.Parameters.AddWithValue("@param19", report.ZCounter);

                    try
                    {
                        con.Open();
                        var result = cmd.ExecuteNonQuery();
                        return result;
                    }
                    catch (SqlException s)
                    {
                        Console.WriteLine(s.Message);
                        return 0;
                    }

                }
            }
        }
        public int CountTransaction(string pos_id, string dt_from, string dt_to)
        {
            string conn = CreateConnectionString();

            int result;

            using (SqlConnection con = new SqlConnection(conn))
            {
                // string querystr = "SELECT Count(*) as trans_count from electric_journal where pos_id = @posid AND transaction_date BETWEEN @date_from AND @date_to";
                string querystr = "SELECT Count(*) as trans_count from Print_Log where pos_id = @posid AND Print_Date BETWEEN @date_from AND @date_to";

                using (SqlCommand cmd = new SqlCommand(querystr, con))
                {

                    cmd.Parameters.Add("@posid", SqlDbType.Int).Value = pos_id;
                    cmd.Parameters.Add("@date_from", SqlDbType.DateTime).Value = dt_from;
                    cmd.Parameters.Add("@date_to", SqlDbType.DateTime).Value = dt_to;

                    Console.WriteLine(dt_from);
                    Console.WriteLine(dt_to);

                    con.Open();
                    result = (int)cmd.ExecuteScalar();

                }
            }
            return result;
        }
        public List<string> ReadJournalData(string pos_id, string dt_from, string dt_to)
        {
            string conn = CreateConnectionString();

            using (SqlConnection con = new SqlConnection(conn))
            {
                //string querystr = "SELECT si_number, data as trans_count from electric_journal where pos_id = @posid AND transaction_date BETWEEN @date_from AND @date_to";
                string querystr = "SELECT id, data as trans_count from Print_Log where pos_id = @posid AND Print_Date BETWEEN @date_from AND @date_to";

                using (SqlCommand cmd = new SqlCommand(querystr, con))
                {

                    cmd.Parameters.Add("@posid", SqlDbType.Int).Value = pos_id;
                    cmd.Parameters.Add("@date_from", SqlDbType.DateTime).Value = dt_from;
                    cmd.Parameters.Add("@date_to", SqlDbType.DateTime).Value = dt_to;

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ReadSingleRow(reader);
                    }

                    return rl;
                }
            }
        }
        private void ReadSingleRow(IDataRecord record)
        {
            rl.Add((string)record[1]);
        }
        public string GetSerialNumber(string posid)
        {
            string srn = null;
            string conn = CreateConnectionString();
            using (SqlConnection con = new SqlConnection(conn))
            {
                string querystr = "SELECT SerialNo FROM Headers WHERE POS_ID = @param1";

                using (SqlCommand cmd = new SqlCommand(querystr, con))
                {
                    cmd.Parameters.Add("@param1", SqlDbType.Int).Value = posid;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        srn = reader[0].ToString();
                    }
                    return srn;
                }
            }
        }
    }
}
