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
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void AddLoadingInfo(string text)
        {
            infoList.Items.Add(text);
            infoList.SelectedIndex = infoList.Items.Count - 1;
        }
        public void EnableCloseButton(bool y)
        {
            if (y == true)
            {
                btn_close.Enabled = true;
            }
            else
            {
                return;
            }
        }

        private void infoList_DrawItem(object sender, DrawItemEventArgs e)
        {

        }

        private void Splash_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btn_close.Enabled == false)
            {
                return;
            }
        }

        private void btn_close_EnabledChanged(object sender, EventArgs e)
        {

        }

        private void Splash_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
