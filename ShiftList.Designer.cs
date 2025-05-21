namespace VenusRetailPOSServer
{
    partial class ShiftList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btn_closePrintFrm = new System.Windows.Forms.Button();
            this.btn_select = new System.Windows.Forms.Button();
            this.period_dgv = new System.Windows.Forms.DataGridView();
            this.OpnDT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clsDT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Shift = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PeriodID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.form_label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.period_dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_closePrintFrm
            // 
            this.btn_closePrintFrm.BackColor = System.Drawing.Color.Red;
            this.btn_closePrintFrm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_closePrintFrm.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_closePrintFrm.Location = new System.Drawing.Point(318, 460);
            this.btn_closePrintFrm.Name = "btn_closePrintFrm";
            this.btn_closePrintFrm.Size = new System.Drawing.Size(104, 42);
            this.btn_closePrintFrm.TabIndex = 9;
            this.btn_closePrintFrm.Text = "&Close";
            this.btn_closePrintFrm.UseVisualStyleBackColor = false;
            this.btn_closePrintFrm.Click += new System.EventHandler(this.btn_closePrintFrm_Click);
            // 
            // btn_select
            // 
            this.btn_select.BackColor = System.Drawing.Color.Lime;
            this.btn_select.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_select.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_select.Location = new System.Drawing.Point(12, 460);
            this.btn_select.Name = "btn_select";
            this.btn_select.Size = new System.Drawing.Size(104, 42);
            this.btn_select.TabIndex = 8;
            this.btn_select.Text = "&Select";
            this.btn_select.UseVisualStyleBackColor = false;
            this.btn_select.Click += new System.EventHandler(this.btn_select_Click);
            // 
            // period_dgv
            // 
            this.period_dgv.AllowUserToAddRows = false;
            this.period_dgv.AllowUserToDeleteRows = false;
            this.period_dgv.AllowUserToResizeRows = false;
            this.period_dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.period_dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.period_dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.period_dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.OpnDT,
            this.clsDT,
            this.Shift,
            this.PeriodID});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.period_dgv.DefaultCellStyle = dataGridViewCellStyle2;
            this.period_dgv.Location = new System.Drawing.Point(12, 84);
            this.period_dgv.Name = "period_dgv";
            this.period_dgv.ReadOnly = true;
            this.period_dgv.RowHeadersVisible = false;
            this.period_dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.period_dgv.Size = new System.Drawing.Size(410, 370);
            this.period_dgv.TabIndex = 7;
            // 
            // OpnDT
            // 
            this.OpnDT.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.OpnDT.FillWeight = 71.36106F;
            this.OpnDT.HeaderText = "Open Date";
            this.OpnDT.Name = "OpnDT";
            this.OpnDT.ReadOnly = true;
            this.OpnDT.Width = 170;
            // 
            // clsDT
            // 
            this.clsDT.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clsDT.FillWeight = 228.4264F;
            this.clsDT.HeaderText = "Close Date";
            this.clsDT.Name = "clsDT";
            this.clsDT.ReadOnly = true;
            // 
            // Shift
            // 
            this.Shift.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Shift.FillWeight = 0.2125148F;
            this.Shift.HeaderText = "Shift";
            this.Shift.Name = "Shift";
            this.Shift.ReadOnly = true;
            this.Shift.Width = 67;
            // 
            // PeriodID
            // 
            this.PeriodID.HeaderText = "PeriodID";
            this.PeriodID.Name = "PeriodID";
            this.PeriodID.ReadOnly = true;
            this.PeriodID.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Select a Period";
            // 
            // form_label
            // 
            this.form_label.AutoSize = true;
            this.form_label.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.form_label.Location = new System.Drawing.Point(12, 11);
            this.form_label.Name = "form_label";
            this.form_label.Size = new System.Drawing.Size(178, 25);
            this.form_label.TabIndex = 5;
            this.form_label.Text = "Print Shift Report";
            // 
            // ShiftList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 512);
            this.Controls.Add(this.btn_closePrintFrm);
            this.Controls.Add(this.btn_select);
            this.Controls.Add(this.period_dgv);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.form_label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ShiftList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShiftList";
            this.Load += new System.EventHandler(this.ShiftList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.period_dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_closePrintFrm;
        private System.Windows.Forms.Button btn_select;
        private System.Windows.Forms.DataGridView period_dgv;
        private System.Windows.Forms.DataGridViewTextBoxColumn OpnDT;
        private System.Windows.Forms.DataGridViewTextBoxColumn clsDT;
        private System.Windows.Forms.DataGridViewTextBoxColumn Shift;
        private System.Windows.Forms.DataGridViewTextBoxColumn PeriodID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label form_label;
    }
}