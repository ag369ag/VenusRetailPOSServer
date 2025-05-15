using IniParser.Model;
using IniParser;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VenusRetailPOSServer
{
    public partial class Login : Form
    {
        public Cashier chr;

        public string host;

        public Login(int posNum)
        {
            InitializeComponent();
            string apiNum = $"API{posNum}";
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("config.ini");
            host = data[apiNum]["Url"];
            host += "cashierCtrl/loginWithInnerJoin";
        }

        public string preview;
        public int? CashierID;
        public string psw;


        private void btn_enter_Click(object sender, EventArgs e)
        {
            try
            {
                bool result;

                if (txt_preview.Text != "")
                {
                    if (CashierID == null)
                    {

                        CashierID = Int32.Parse(txt_preview.Text.ToString());

                        txt_preview.Clear();
                        ShowNotif("Enter your password", 1);
                        txt_preview.UseSystemPasswordChar = true;
                        txt_preview.Focus();
                    }
                    else
                    {
                        psw = txt_preview.Text;
                        txt_preview.Clear();
                        
                        result = CashierLogin();
                       // result = true;

                        if (result == true)
                        {
                            txt_preview.Clear();

                            this.Close();
                            DialogResult = DialogResult.OK;

                            //2. login to enbler session

                        }
                        else
                        {
                            ShowNotif("Invalid Cashier ID or Password! Try Again.", 2);
                            CashierID = null;
                            psw = "";
                            txt_preview.Focus();
                            txt_preview.UseSystemPasswordChar = false;
                        }
                    }

                }
            }
            catch (Exception)
            {
                txt_preview.Clear();
            }

        }

        private void Keypad_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Console.WriteLine(btn.Tag.ToString());
            SendKeys.Send(btn.Tag.ToString());
        }

        private void btn_clr_Click(object sender, EventArgs e)
        {
            txt_preview.Clear();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string dt;
            dt = DateTime.Now.ToLongDateString();
            dt = dt + " " + DateTime.Now.ToLongTimeString();

            lbl_datetime.Text = dt;
        }

        public bool CashierLogin()
        {
            var client = new RestClient(host);
            var request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "b3214378-2628-b7db-8fa4-8604f3a742c7");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("number", CashierID);
            request.AddParameter("password", psw);

            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            if (response.StatusCode == 0)
            {
                MessageBox.Show("Unable to Connect to Database" + Environment.NewLine + "Please check the webservice", "Error", MessageBoxButtons.OK);
                this.Text = "POS OFFLINE";
                return false;
            }
            else
            {
                this.Text = "POS ONLINE";

                try
                {
                    RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
                    
                    Response res = deserializer.Deserialize<Response>(response);
                    if (res.statusCode == 1)
                    {
                        var apiResponse = deserializer.Deserialize<Dictionary<string, string>>(response);
                        var data = apiResponse["data"].ToString();

                        response.Content = data;
                        chr = deserializer.Deserialize<Cashier>(response);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {

                    MessageBox.Show(response.Content);

                    return false;
                }

            }
        }



        private void ShowNotif(string message, int type)
        {
            Color color;
            switch (type)
            {
                case 2:
                    color = Color.Red;
                    break;
                case 1:
                default:
                    color = Color.Black;
                    break;

            }
            lbl_notif.ForeColor = color;
            lbl_notif.Text = message;
        }

        private void CheckEnter(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                btn_enter_Click(sender, e);
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            try
            {
                pictureBox1.Image = Image.FromFile(@"assets\logo.png");
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch { }


            //if (CompareKey(productKey) == false)
            if (false)
            {
                MessageBox.Show("Registration key is not valid", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                Application.Exit();
            }

            ShowNotif("Enter your Cashier ID", 1);
        }
    }
}
