namespace VenusRetailPOSServer
{
    partial class RefundReportForm
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
            this.btn_printFull = new System.Windows.Forms.Button();
            this.dt_to = new System.Windows.Forms.DateTimePicker();
            this.dt_from = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_printFull
            // 
            this.btn_printFull.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_printFull.Location = new System.Drawing.Point(37, 239);
            this.btn_printFull.Name = "btn_printFull";
            this.btn_printFull.Size = new System.Drawing.Size(242, 45);
            this.btn_printFull.TabIndex = 9;
            this.btn_printFull.Text = "Print";
            this.btn_printFull.UseVisualStyleBackColor = true;
            this.btn_printFull.Click += new System.EventHandler(this.btn_printFull_Click);
            // 
            // dt_to
            // 
            this.dt_to.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.dt_to.Location = new System.Drawing.Point(16, 140);
            this.dt_to.Name = "dt_to";
            this.dt_to.Size = new System.Drawing.Size(287, 29);
            this.dt_to.TabIndex = 8;
            // 
            // dt_from
            // 
            this.dt_from.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.dt_from.Location = new System.Drawing.Point(16, 52);
            this.dt_from.Name = "dt_from";
            this.dt_from.Size = new System.Drawing.Size(287, 29);
            this.dt_from.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 21);
            this.label2.TabIndex = 6;
            this.label2.Text = "To Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(21, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 21);
            this.label1.TabIndex = 5;
            this.label1.Text = "From Date";
            // 
            // RefundReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(318, 312);
            this.Controls.Add(this.btn_printFull);
            this.Controls.Add(this.dt_to);
            this.Controls.Add(this.dt_from);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "RefundReportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RefundReport";
            this.Load += new System.EventHandler(this.VoidRefund_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_printFull;
        private System.Windows.Forms.DateTimePicker dt_to;
        private System.Windows.Forms.DateTimePicker dt_from;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}