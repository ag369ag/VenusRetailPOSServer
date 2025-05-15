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
    public partial class CashDrawListForm : Form
    {
        public API api;
        public List<CashDrawList> cdlist = new List<CashDrawList>();
        public string formTitle;
        public int posID;
        public string cashdrawData;
        public CashDrawListForm(int posID)
        {
            InitializeComponent();
            api = new API(posID);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        private void btn_closePrintFrm_Click(object sender, EventArgs e)
        {
            CashDrawListForm.ActiveForm.Close();
        }

        private void btn_select_Click(object sender, EventArgs e)
        {

            var periodID = cdraw_dgv.Rows[cdraw_dgv.CurrentRow.Index].Cells[0].Value.ToString();

            cashdrawData = api.GetCashDrawReport(periodID);
            //MessageBox.Show(cashdrawData);
            if (cashdrawData == "false")
            {
                DialogResult = DialogResult.Cancel;
                CashDrawListForm.ActiveForm.Close();
            }
            else
            {
                DialogResult = DialogResult.OK;
                CashDrawListForm.ActiveForm.Close();
            }
        }

        private void CashDrawList_Load(object sender, EventArgs e)
        {
            CreateList();
        }

        private void CreateList()
        {
            cdlist = cdlist.Where(x => !String.IsNullOrEmpty(x.CDraw_Close_Date)).OrderByDescending(x => x.CDraw_Close_Date).ToList();

            for (int i = 0; i < cdlist.Count; i++)
            {
                DataGridViewRow row = new DataGridViewRow();

                cdraw_dgv.Rows.Add(row);
                int idx = cdraw_dgv.RowCount - 1;
                cdraw_dgv.Rows[idx].Cells[0].Value = cdlist[i].CDraw_Period_ID;
                cdraw_dgv.Rows[idx].Cells[1].Value = cdlist[i].CDraw_Close_Date;
                cdraw_dgv.Rows[idx].Cells[2].Value = cdlist[i].Cashier_Name;
            }
        }
    }
}
