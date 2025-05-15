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
    public partial class RefundReportForm : Form
    {
        public bool FullReport { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public RefundReportForm()
        {
            InitializeComponent();
        }

        private void btn_printFull_Click(object sender, EventArgs e)
        {
            FullReport = true;
            CheckDate();
            DialogResult = DialogResult.OK;
            ActiveForm.Close();
        }

        private void VoidRefund_Load(object sender, EventArgs e)
        {
            dt_from.MaxDate = DateTime.Today;
            dt_to.MaxDate = DateTime.Today;
        }

        private void CheckDate()
        {
            TimeSpan startTime = new TimeSpan(00, 00, 00);
            TimeSpan endTime = new TimeSpan(23, 59, 59);

            var x = dt_from.Value.Date;
            var y = dt_to.Value.Date;

            StartDate = x.Add(startTime);
            EndDate = y.Add(endTime);


            //Console.WriteLine("Start Date => {0} , End Date => {1}", StartDate, EndDate);
        }
    }
}
