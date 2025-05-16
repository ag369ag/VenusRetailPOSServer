namespace VenusRetailPOSServer
{
    partial class ReportsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportsForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btn_print_cashdraw = new System.Windows.Forms.Button();
            this.btn_print_shift = new System.Windows.Forms.Button();
            this.btn_print_day = new System.Windows.Forms.Button();
            this.btn_print_month = new System.Windows.Forms.Button();
            this.btn_refund_report = new System.Windows.Forms.Button();
            this.btn_cumulative_sale = new System.Windows.Forms.Button();
            this.btn_generate_csv = new System.Windows.Forms.Button();
            this.btn_generate_accu_csv = new System.Windows.Forms.Button();
            this.btn_pos_config = new System.Windows.Forms.Button();
            this.btn_ejournal_consolidator = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 7;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.btn_print_cashdraw, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btn_print_shift, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btn_print_day, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.btn_print_month, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.btn_refund_report, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.btn_cumulative_sale, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.btn_generate_csv, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.btn_generate_accu_csv, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.btn_pos_config, 5, 3);
            this.tableLayoutPanel1.Controls.Add(this.btn_ejournal_consolidator, 4, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Segoe UI Symbol", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btn_print_cashdraw
            // 
            this.btn_print_cashdraw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_print_cashdraw.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_print_cashdraw.Location = new System.Drawing.Point(83, 48);
            this.btn_print_cashdraw.Name = "btn_print_cashdraw";
            this.btn_print_cashdraw.Size = new System.Drawing.Size(122, 151);
            this.btn_print_cashdraw.TabIndex = 7;
            this.btn_print_cashdraw.Text = "Print Cash Draw";
            this.btn_print_cashdraw.UseVisualStyleBackColor = true;
            this.btn_print_cashdraw.Click += new System.EventHandler(this.btn_print_cashdraw_Click);
            // 
            // btn_print_shift
            // 
            this.btn_print_shift.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_print_shift.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_print_shift.Location = new System.Drawing.Point(211, 48);
            this.btn_print_shift.Name = "btn_print_shift";
            this.btn_print_shift.Size = new System.Drawing.Size(122, 151);
            this.btn_print_shift.TabIndex = 8;
            this.btn_print_shift.Text = "Print Shift";
            this.btn_print_shift.UseVisualStyleBackColor = true;
            this.btn_print_shift.Click += new System.EventHandler(this.btn_print_shift_Click);
            // 
            // btn_print_day
            // 
            this.btn_print_day.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_print_day.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_print_day.Location = new System.Drawing.Point(339, 48);
            this.btn_print_day.Name = "btn_print_day";
            this.btn_print_day.Size = new System.Drawing.Size(122, 151);
            this.btn_print_day.TabIndex = 9;
            this.btn_print_day.Text = "Print Day";
            this.btn_print_day.UseVisualStyleBackColor = true;
            this.btn_print_day.Click += new System.EventHandler(this.btn_print_day_Click);
            // 
            // btn_print_month
            // 
            this.btn_print_month.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_print_month.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_print_month.Location = new System.Drawing.Point(467, 48);
            this.btn_print_month.Name = "btn_print_month";
            this.btn_print_month.Size = new System.Drawing.Size(122, 151);
            this.btn_print_month.TabIndex = 10;
            this.btn_print_month.Text = "Print Month";
            this.btn_print_month.UseVisualStyleBackColor = true;
            this.btn_print_month.Click += new System.EventHandler(this.btn_print_month_Click);
            // 
            // btn_refund_report
            // 
            this.btn_refund_report.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_refund_report.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_refund_report.Location = new System.Drawing.Point(83, 250);
            this.btn_refund_report.Name = "btn_refund_report";
            this.btn_refund_report.Size = new System.Drawing.Size(122, 151);
            this.btn_refund_report.TabIndex = 4;
            this.btn_refund_report.Text = "Refund Report";
            this.btn_refund_report.UseVisualStyleBackColor = true;
            this.btn_refund_report.Click += new System.EventHandler(this.btn_refund_report_Click);
            // 
            // btn_cumulative_sale
            // 
            this.btn_cumulative_sale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_cumulative_sale.Font = new System.Drawing.Font("Segoe UI Symbol", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_cumulative_sale.Location = new System.Drawing.Point(595, 48);
            this.btn_cumulative_sale.Name = "btn_cumulative_sale";
            this.btn_cumulative_sale.Size = new System.Drawing.Size(122, 151);
            this.btn_cumulative_sale.TabIndex = 5;
            this.btn_cumulative_sale.Text = "Cumulative Sale";
            this.btn_cumulative_sale.UseVisualStyleBackColor = true;
            this.btn_cumulative_sale.Click += new System.EventHandler(this.btn_cumulative_sale_Click);
            // 
            // btn_generate_csv
            // 
            this.btn_generate_csv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_generate_csv.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_generate_csv.Location = new System.Drawing.Point(339, 250);
            this.btn_generate_csv.Name = "btn_generate_csv";
            this.btn_generate_csv.Size = new System.Drawing.Size(122, 151);
            this.btn_generate_csv.TabIndex = 11;
            this.btn_generate_csv.Text = "Generate CSV";
            this.btn_generate_csv.UseVisualStyleBackColor = true;
            this.btn_generate_csv.Click += new System.EventHandler(this.btn_generate_csv_Click);
            // 
            // btn_generate_accu_csv
            // 
            this.btn_generate_accu_csv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_generate_accu_csv.Font = new System.Drawing.Font("Segoe UI Symbol", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_generate_accu_csv.Location = new System.Drawing.Point(211, 250);
            this.btn_generate_accu_csv.Name = "btn_generate_accu_csv";
            this.btn_generate_accu_csv.Size = new System.Drawing.Size(122, 151);
            this.btn_generate_accu_csv.TabIndex = 12;
            this.btn_generate_accu_csv.Text = "Generate Accumulating CSV";
            this.btn_generate_accu_csv.UseVisualStyleBackColor = true;
            this.btn_generate_accu_csv.Click += new System.EventHandler(this.btn_generate_accu_csv_Click);
            // 
            // btn_pos_config
            // 
            this.btn_pos_config.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_pos_config.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_pos_config.Location = new System.Drawing.Point(595, 250);
            this.btn_pos_config.Name = "btn_pos_config";
            this.btn_pos_config.Size = new System.Drawing.Size(122, 151);
            this.btn_pos_config.TabIndex = 13;
            this.btn_pos_config.Text = "POS Config";
            this.btn_pos_config.UseVisualStyleBackColor = true;
            this.btn_pos_config.Click += new System.EventHandler(this.btn_pos_config_Click);
            // 
            // btn_ejournal_consolidator
            // 
            this.btn_ejournal_consolidator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_ejournal_consolidator.Font = new System.Drawing.Font("Segoe UI Symbol", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ejournal_consolidator.Location = new System.Drawing.Point(467, 250);
            this.btn_ejournal_consolidator.Name = "btn_ejournal_consolidator";
            this.btn_ejournal_consolidator.Size = new System.Drawing.Size(122, 151);
            this.btn_ejournal_consolidator.TabIndex = 6;
            this.btn_ejournal_consolidator.Text = "E-Journal Consolidator";
            this.btn_ejournal_consolidator.UseVisualStyleBackColor = true;
            this.btn_ejournal_consolidator.Click += new System.EventHandler(this.btn_ejournal_consolidator_Click);
            // 
            // ReportsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(36)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReportsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Venus Retail POS Server";
            this.Load += new System.EventHandler(this.ReportsForm_load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btn_refund_report;
        private System.Windows.Forms.Button btn_cumulative_sale;
        private System.Windows.Forms.Button btn_ejournal_consolidator;
        private System.Windows.Forms.Button btn_print_cashdraw;
        private System.Windows.Forms.Button btn_print_shift;
        private System.Windows.Forms.Button btn_print_day;
        private System.Windows.Forms.Button btn_print_month;
        private System.Windows.Forms.Button btn_generate_csv;
        private System.Windows.Forms.Button btn_generate_accu_csv;
        private System.Windows.Forms.Button btn_pos_config;
    }
}