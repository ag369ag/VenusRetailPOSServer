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
    public partial class ShiftList : Form
    {
        public API api;
        public List<PeriodList> pl = new List<PeriodList>();
        public string formTitle;
        public int posID;
        public string periodData;

        public ShiftList(int loggedInPOSID)
        {
            InitializeComponent();
            api = new API(loggedInPOSID);
            posID = loggedInPOSID;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        private void ShiftList_Load(object sender, EventArgs e)
        {
            ResetPeriodList();
            CreateList();
            form_label.Text = formTitle;
            if (formTitle == "Print Day Report" || formTitle == "Print Month Report")
            {
                period_dgv.Columns["Shift"].Visible = false;
            }
        }

        private void CreateList()
        {
            pl = pl.OrderByDescending(x => x.Period_Create_TS).ToList();

            for (int i = 0; i < pl.Count; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                period_dgv.Rows.Add(row);
                int idx = period_dgv.RowCount - 1;
                period_dgv.Rows[idx].Cells[0].Value = pl[i].Period_Create_TS;
                period_dgv.Rows[idx].Cells[1].Value = pl[i].Period_Close_DT;
                period_dgv.Rows[idx].Cells[2].Value = pl[i].Shift_number;
                period_dgv.Rows[idx].Cells[3].Value = pl[i].Period_ID;

            }

        }

        private void ResetPeriodList()
        {
            period_dgv.Rows.Clear();
        }

        private void btn_closePrintFrm_Click(object sender, EventArgs e)
        {

            ShiftList.ActiveForm.Close();
            DialogResult = DialogResult.Cancel;
        }

        private void btn_select_Click(object sender, EventArgs e)
        {
            var periodID = period_dgv.Rows[period_dgv.CurrentRow.Index].Cells[3].Value.ToString();
            // int state = string.IsNullOrEmpty(period_dgv.Rows[period_dgv.CurrentRow.Index].Cells[2].Value.ToString()) ? 1 : 2;

            periodData = api.GetPeriodReport(Int32.Parse(periodID), posID);

            if (periodData != "false")
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.Cancel;
            }

            ShiftList.ActiveForm.Close();
        }
    }
}
