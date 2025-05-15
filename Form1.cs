using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VenusRetailPOSServer
{
    public partial class Form1 : Form
    {
        Form1 thisForm;
        public Form1()
        {
            InitializeComponent();
            thisForm = this;
        }

        private void POS1_Click(object sender, EventArgs e)
        {
            Login login = new Login(1);
            Form1.ActiveForm.Visible = false;
            

            if(login.ShowDialog() == DialogResult.OK)
            {
                
                ReportsForm reports = new ReportsForm(1, login.chr);
                reports.ShowDialog();
            }

            thisForm.Visible = true;
        }

        private void POS2_Click(object sender, EventArgs e)
        {
            Login login = new Login(2);
            Form1.ActiveForm.Visible = false;


            if (login.ShowDialog() == DialogResult.OK)
            {

                ReportsForm reports = new ReportsForm(2, login.chr);
                reports.ShowDialog();
            }

            thisForm.Visible = true;
        }

        private void POS3_Click(object sender, EventArgs e)
        {
            Login login = new Login(3);
            Form1.ActiveForm.Visible = false;


            if (login.ShowDialog() == DialogResult.OK)
            {

                ReportsForm reports = new ReportsForm(3, login.chr);
                reports.ShowDialog();
            }

            thisForm.Visible = true;
        }
    }
}
