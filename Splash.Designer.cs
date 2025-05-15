namespace VenusRetailPOSServer
{
    partial class Splash
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
            this.infoList = new System.Windows.Forms.ListBox();
            this.btn_close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // infoList
            // 
            this.infoList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoList.FormattingEnabled = true;
            this.infoList.Location = new System.Drawing.Point(0, 0);
            this.infoList.Name = "infoList";
            this.infoList.Size = new System.Drawing.Size(384, 262);
            this.infoList.TabIndex = 4;
            // 
            // btn_close
            // 
            this.btn_close.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btn_close.Enabled = false;
            this.btn_close.Location = new System.Drawing.Point(0, 239);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(384, 23);
            this.btn_close.TabIndex = 5;
            this.btn_close.Text = "&Close";
            this.btn_close.UseVisualStyleBackColor = true;
            // 
            // Splash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 262);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.infoList);
            this.Name = "Splash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Splash";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox infoList;
        private System.Windows.Forms.Button btn_close;
    }
}