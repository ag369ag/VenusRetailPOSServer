using IniParser.Model;
using IniParser;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace VenusRetailPOSServer
{
    public class EnbDB
    {
        public List<string> rl = new List<string>();
        public IniData config;
        private int posID;

        public class CSVHeader
        {
            public int POS_ID { get; set; }
            public string Data { get; set; }
            public string SerialNo { get; set; }
            public string Description { get; set; }
            public string POS_Min { get; set; }
        }


        public EnbDB(int posNum)
        {
            var parser = new FileIniDataParser();
            config = parser.ReadFile("db.ini");
            posID = posNum;
        }
        public string CreateConnectionString()
        {
            string dbFromINI = $"DATABASE{posID}";
            string server = config[dbFromINI]["Host"];
            string user = config[dbFromINI]["Username"];
            string pass = config[dbFromINI]["Password"];
            string db = config[dbFromINI]["Database"];
            return "Driver={SQL Server};Server=" + server + ";UID=" + user + ";PWD=" + pass + ";Database=" + db + ";";
        }
        public string CreateConnectionString(string dbName)
        {
            string journalFromINI = $"JOURNAL{posID}";
            string server = config[journalFromINI]["Host"];
            string user = config[journalFromINI]["Username"];
            string pass = config[journalFromINI]["Password"];
            string db = config[journalFromINI]["Database"];
            return "Server = " + server + "; Database =" + db + "; User Id =" + user + ";Password = " + pass + ";";
        }
        private string CreateConnectionString2()
        {
            string server = null, user = null, pass = null, db = null;

            string dbFromINI = $"DATABASE{posID}";

            server = config[dbFromINI]["Host"];
            user = config[dbFromINI]["Username"];
            pass = config[dbFromINI]["Password"];
            db = config[dbFromINI]["Database"];

            return "Server = " + server + "; Database =" + db + "; User Id =" + user + ";Password = " + pass + ";";
        }

       
        public StringBuilder ConvertToCSV(IDataRecord record, int idx)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = idx; i < record.FieldCount; i++)
            {
                string str = record[i].ToString();
                sb.Append(str + ',');
            }

            sb.Length--;
            return sb;
        }
        public int CountTransaction(string pos_id, string dt_from, string dt_to)
        {
            string conn = CreateConnectionString("JOURNAL");

            int result;

            using (SqlConnection con = new SqlConnection(conn))
            {
                string querystr = "SELECT Count(*) as trans_count from electric_journal where pos_id = @posid AND transaction_date BETWEEN @date_from AND @date_to";

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
        public List<int> GetPeriodIDs(string to, string from)
        {
            string conn = CreateConnectionString2();

            using (SqlConnection con = new SqlConnection(conn))
            {
                string querystr = "SELECT Period_ID FROM Periods WHERE CAST(Period_Close_DT as DATE) BETWEEN @from and @to ";
                using (SqlCommand cmd = new SqlCommand(querystr, con))
                {
                    cmd.Parameters.AddWithValue("@from", from);
                    cmd.Parameters.AddWithValue("@to", to);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<int> pid = new List<int>();
                    while (reader.Read())
                    {
                        pid.Add((int)reader[0]);
                    }
                    return pid;
                }
            }
        }
        public List<string> BIRSalesSummary(int POSID, string range)
        {
            string conn = CreateConnectionString2();

            using (SqlConnection con = new SqlConnection(conn))
            {
                string querystr = "SELECT POS_ID, Period_ID, Date_Generated, SI_Start, SI_End, CAST(Acc_Sales_Ending AS DECIMAL(18,2)), CAST(Acc_Sales_Begin AS DECIMAL(18,2)), CAST(Manual_Sales AS DECIMAL(18,2)), CAST(Gross_Sales AS DECIMAL(18,2)), CAST(VATable_Sales AS DECIMAL(18,2)), CAST(VAT_Amount AS DECIMAL(18,2)), CAST(VAT_Exempt AS DECIMAL(18,2)), CAST(Zero_Rated AS DECIMAL(18,2)), CAST(Deduct_SC AS DECIMAL(18,2)), CAST(Deduct_PWD AS DECIMAL(18,2)), CAST(Deduct_NAAC AS DECIMAL(18,2)), CAST(Deduct_SP AS DECIMAL(18,2)), CAST(Deduct_MoV AS DECIMAL(18,2)), CAST(Deduct_Others AS DECIMAL(18,2)), CAST(Deduct_Return AS DECIMAL(18,2)), CAST(Deduct_Voids AS DECIMAL(18,2)), CAST(Deduct_Total AS DECIMAL(18,2)), CAST(Adj_SC AS DECIMAL(18,2)), CAST(Adj_PWD AS DECIMAL(18,2)), CAST(Adj_Disc_Others AS DECIMAL(18,2)), CAST(Adj_VAT_Returns AS DECIMAL(18,2)), CAST(Adj_Others AS DECIMAL(18,2))," +
                    " CAST(Adj_Total AS DECIMAL(18,2)), CAST(VAT_Payable AS DECIMAL(18,2)), CAST(Net_Sales AS DECIMAL(18,2)), CAST(Sales_Overrun AS DECIMAL(18,2)), CAST(Total_Income AS DECIMAL(18,2)), Reset_Counter, ZCounter, Remarks FROM EoD_Reports where POS_ID = @param1 " + range;
                Console.WriteLine(querystr);
                Console.WriteLine("query");
                using (SqlCommand cmd = new SqlCommand(querystr, con))
                {
                    cmd.Parameters.AddWithValue("@param1", POSID);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<string> lines = new List<string>();
                    while (reader.Read())
                    {
                        lines.Add(ConvertToCSV(reader, 2).ToString());
                    }
                    foreach (var i in lines)
                    {

                        Console.WriteLine(i);
                    }
                    return lines;
                }
            }
        }
        public async Task<int> LogPOSEvent(string description, int eventType, int posid, string chrname)
        {
            string connStr = CreateConnectionString2();
            Console.WriteLine("log1");
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {

                    using (SqlCommand cmd = new SqlCommand("sp_Log_Event", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", 1);
                        cmd.Parameters.AddWithValue("@device_type", 15);
                        cmd.Parameters.AddWithValue("@device_id", 0);
                        cmd.Parameters.AddWithValue("@device_number", posid);
                        cmd.Parameters.AddWithValue("@event_type", eventType);
                        cmd.Parameters.AddWithValue("@desc", description);
                        cmd.Parameters.AddWithValue("@username", chrname);
                        Console.WriteLine("log2");

                        await con.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        return 1;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("log3");

                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public List<string> GetSCReports(int POSID, string range)
        {
            string conn = CreateConnectionString2();

            using (SqlConnection con = new SqlConnection(conn))
            {
                string querystr = "SELECT ID, Transaction_ID, Period_ID, POS_ID, Transaction_Date, Senior_Name, OSCAID_Number, SC_TIN, SI_OR_Number, CAST(VATABLE_Sales AS DECIMAL(18,2)), CAST(VAT_Amount AS DECIMAL(18,2)), CAST(VATEXEMPT_Sales AS DECIMAL(18,2)), CAST(FivePercent_Discount AS DECIMAL(18,2)), CAST(TwentyPercent_Discount AS DECIMAL(18,2)), CAST(Net_Sales AS DECIMAL(18,2)) FROM Transaction_History_SeniorCitizen where POS_ID = @param1 and isrefund <> 1 " + range;

                using (SqlCommand cmd = new SqlCommand(querystr, con))
                {
                    cmd.Parameters.AddWithValue("@param1", POSID);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<string> lines = new List<string>();
                    while (reader.Read())
                    {
                        lines.Add(ConvertToCSV(reader, 4).ToString());
                    }
                    return lines;
                }
            }
        }

        public List<string> GetMoVReports(int POSID, string range)
        {
            string conn = CreateConnectionString2();

            using (SqlConnection con = new SqlConnection(conn))
            {
                string querystr = "SELECT ID, Transaction_ID, Period_ID, POS_ID, Transaction_Date, MOV_Name, MOVID_Number, MOV_TIN, SI_OR_Number, CAST(VATABLE_Sales AS DECIMAL(18,2)), CAST(VAT_Amount AS DECIMAL(18,2)), CAST(VATEXEMPT_Sales AS DECIMAL(18,2)), CAST(FivePercent_Discount AS DECIMAL(18,2)), CAST(TwentyPercent_Discount AS DECIMAL(18,2)), CAST(Net_Sales AS DECIMAL(18,2)) FROM Transaction_History_MedalofValor where POS_ID = @param1 and isrefund <> 1 " + range;

                using (SqlCommand cmd = new SqlCommand(querystr, con))
                {
                    cmd.Parameters.AddWithValue("@param1", POSID);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<string> lines = new List<string>();
                    while (reader.Read())
                    {
                        lines.Add(ConvertToCSV(reader, 4).ToString());
                    }
                    return lines;
                }
            }
        }
        public List<string> GetPWDReports(int POSID, string range)
        {
            string conn = CreateConnectionString2();

            using (SqlConnection con = new SqlConnection(conn))
            {
                string querystr = "SELECT ID, Transaction_ID, Period_ID, POS_ID, Transaction_Date, PWD_Name, PWD_ID_Number, PWD_TIN_Number, SI_OR_Number, CAST(VATABLE_Sales AS DECIMAL(18,2)), CAST(VAT_Amount AS DECIMAL(18,2)), CAST(VATEXEMPT_Sales AS DECIMAL(18,2)), CAST(FivePercent_Discount AS DECIMAL(18,2)), CAST(TwentyPercent_Discount AS DECIMAL(18,2)), CAST(Net_Sales AS DECIMAL(18,2)) FROM Transaction_History_PWD where POS_ID = @param1 and isrefund <> 1 " + range;

                using (SqlCommand cmd = new SqlCommand(querystr, con))
                {
                    cmd.Parameters.AddWithValue("@param1", POSID);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<string> lines = new List<string>();
                    while (reader.Read())
                    {
                        lines.Add(ConvertToCSV(reader, 4).ToString());
                    }
                    return lines;
                }
            }
        }
        public List<string> GetNAACReports(int POSID, string range)
        {
            string conn = CreateConnectionString2();

            using (SqlConnection con = new SqlConnection(conn))
            {
                string querystr = "SELECT ID, Transaction_ID, Period_ID, POS_ID, Transaction_Date, AthleteCoach_Name, PNSTMID_Number, SI_OR_Number, CAST(Gross_Sales AS DECIMAL(18, 2)), CAST(Sales_Discount AS DECIMAL(18, 2)), CAST(Net_Sales AS DECIMAL(18, 2)) FROM Transaction_History_NationalAthlete where POS_ID = @param1 and isrefund <> 1 " + range;

                using (SqlCommand cmd = new SqlCommand(querystr, con))
                {
                    cmd.Parameters.AddWithValue("@param1", POSID);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<string> lines = new List<string>();
                    while (reader.Read())
                    {
                        lines.Add(ConvertToCSV(reader, 4).ToString());
                    }
                    return lines;
                }
            }
        }
        public List<string> GetSPReports(int POSID, string range)
        {
            string conn = CreateConnectionString2();

            using (SqlConnection con = new SqlConnection(conn))
            {
                string querystr = "SELECT ID, Transaction_ID, Period_ID, POS_ID, Transaction_Date, Parent_Name, SPIC_Number, Child_Name, Child_DOB, Child_Age, SI_OR_Number, CAST(Gross_Sales AS DECIMAL(18, 2)), CAST(Sales_Discount AS DECIMAL(18, 2)), CAST(Net_Sales AS DECIMAL(18, 2)) FROM Transaction_History_SoloParent where POS_ID = @param1 " + range;

                using (SqlCommand cmd = new SqlCommand(querystr, con))
                {
                    cmd.Parameters.AddWithValue("@param1", POSID);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<string> lines = new List<string>();
                    while (reader.Read())
                    {
                        lines.Add(ConvertToCSV(reader, 4).ToString());
                    }
                    return lines;
                }
            }
        }
        public int SaveEoDReport(EoD_Report eod)
        {
            Console.WriteLine($"{eod.SI_Start} - {eod.SI_End}");
            string conn = CreateConnectionString2();
            using (SqlConnection con = new SqlConnection(conn))
            {
                string sel = "(SELECT 1 FROM EoD_Reports WHERE POS_ID = @param1 AND Period_ID = @param2)";

                string ins = "INSERT INTO EoD_Reports (POS_ID, Period_ID, Date_Generated, SI_Start, SI_End, Acc_Sales_Ending, Acc_Sales_Begin, Gross_Sales, VATable_Sales, VAT_Amount, VAT_Exempt, Zero_Rated, Deduct_SC, Deduct_PWD, Deduct_NAAC, Deduct_SP, Deduct_Others, Deduct_Return, Deduct_Total, Net_Sales, Reset_Counter, ZCounter, Deduct_MoV) " +
                        "VALUES(@param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9, @param10, @param11, @param12, @param13, @param14, @param15, @param16, @param17, @param18, @param19, @param20, @param21, @param22, @param23)";

                string upd = "UPDATE EoD_Reports SET Date_Generated = @param3, SI_Start = @param4, SI_End = @param5, Acc_Sales_Ending = @param6, Acc_Sales_Begin = @param7, Gross_Sales = @param8, VATable_Sales = @param9, VAT_Amount = @param10, VAT_Exempt = @param11, " +
                    "Zero_Rated = @param12, Deduct_SC = @param13, Deduct_PWD = @param14, Deduct_NAAC = @param15, Deduct_SP = @param16, Deduct_Others = @param17, Deduct_Return = @param18, Deduct_Total = @param19, Net_Sales = @param20, Reset_Counter = @param21, ZCounter = @param22, Deduct_MoV = @param23 " +
                    "WHERE POS_ID = @param1 AND Period_ID = @param2";

                string querystr = @"begin tran if exists "
                    + sel
                    + " begin "
                    + upd
                    + " end "
                    + "else "
                    + "begin "
                    + ins
                    + " end commit tran";

                Console.WriteLine($"{eod.Deduct_MoV}==============================");

                using (SqlCommand cmd = new SqlCommand(querystr, con))
                {
                    cmd.Parameters.AddWithValue("@param1", eod.POSID);
                    cmd.Parameters.AddWithValue("@param2", eod.Period_ID);
                    cmd.Parameters.Add("@param3", SqlDbType.DateTime2).Value = eod.DT_Generated;
                    cmd.Parameters.AddWithValue("@param4", eod.SI_Start);
                    cmd.Parameters.AddWithValue("@param5", eod.SI_End);
                    cmd.Parameters.AddWithValue("@param6", eod.AccSalesEnd);
                    cmd.Parameters.AddWithValue("@param7", eod.AccSalesBegin);
                    cmd.Parameters.AddWithValue("@param8", eod.GrossSales);
                    cmd.Parameters.AddWithValue("@param9", eod.VATABLE_Sales);
                    cmd.Parameters.AddWithValue("@param10", eod.VAT_Amount);
                    cmd.Parameters.AddWithValue("@param11", eod.VAT_Exempt);
                    cmd.Parameters.AddWithValue("@param12", eod.ZeroRated);
                    cmd.Parameters.AddWithValue("@param13", eod.Deduct_SC);
                    cmd.Parameters.AddWithValue("@param14", eod.Deduct_PWD);
                    cmd.Parameters.AddWithValue("@param15", eod.Deduct_NAAC);
                    cmd.Parameters.AddWithValue("@param16", eod.Deduct_SP);
                    cmd.Parameters.AddWithValue("@param17", eod.Deduct_Others);
                    cmd.Parameters.AddWithValue("@param18", eod.Deduct_Returns);
                    cmd.Parameters.AddWithValue("@param19", eod.Deduct_Total);
                    cmd.Parameters.AddWithValue("@param20", eod.NetSales);
                    cmd.Parameters.AddWithValue("@param21", eod.ResetCounter);
                    cmd.Parameters.AddWithValue("@param22", eod.ZCounter);
                    cmd.Parameters.AddWithValue("@param23", eod.Deduct_MoV);

                    try
                    {
                        con.Open();
                        var result = cmd.ExecuteNonQuery();
                        return result;
                    }
                    catch (SqlException s)
                    {
                        Console.WriteLine($"[SaveEoDReport] Error : {s}\n" +
                            $"{querystr}");

                        return 0;
                    }

                }
            }
        }
        public List<string> ReadJournalData(string pos_id, string dt_from, string dt_to)
        {
            string conn = CreateConnectionString("JOURNAL");

            using (SqlConnection con = new SqlConnection(conn))
            {
                string querystr = "SELECT si_number, data as trans_count from electric_journal where pos_id = @posid AND transaction_date BETWEEN @date_from AND @date_to";

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
        public List<string> GetAccumulatingData(int POSID)
        {
            string conn = CreateConnectionString("JOURNAL");

            using (SqlConnection con = new SqlConnection(conn))
            {
                string querystr = "SELECT * FROM Accumulating_Report where POS_ID = @param1";

                using (SqlCommand cmd = new SqlCommand(querystr, con))
                {
                    cmd.Parameters.AddWithValue("@param1", POSID);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<string> lines = new List<string>();
                    while (reader.Read())
                    {
                        lines.Add(ConvertToCSV(reader).ToString());
                    }
                    return lines;
                }
            }
        }
        public StringBuilder ConvertToCSV(IDataRecord record)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < record.FieldCount; i++)
            {
                string str = record[i].ToString();
                sb.Append(str + ',');
            }

            sb.Length--;
            return sb;
        }
        public List<CSVHeader> GetHeaders()
        {
            string conn = CreateConnectionString("JOURNAL");
            List<CSVHeader> CSV_Headers = new List<CSVHeader>();
            using (SqlConnection con = new SqlConnection(conn))
            {
                string querystr = "SELECT * FROM Headers";

                using (SqlCommand cmd = new SqlCommand(querystr, con))
                {

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<string> lines = new List<string>();
                    while (reader.Read())
                    {
                        CSVHeader x = new CSVHeader
                        {
                            POS_ID = int.Parse(reader["POS_ID"].ToString()),
                            Data = reader["Data"].ToString(),
                            Description = reader["Description"].ToString(),
                            POS_Min = reader["POS_MIN"].ToString(),
                            SerialNo = reader["SerialNo"].ToString()
                        };
                        CSV_Headers.Add(x);
                    }
                    return CSV_Headers;
                }
            }
        }

    }
}
