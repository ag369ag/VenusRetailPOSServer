namespace VenusRetailPOSServer
{
    partial class Login
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_notif = new System.Windows.Forms.Label();
            this.lbl_datetime = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txt_preview = new System.Windows.Forms.TextBox();
            this.btn_enter = new System.Windows.Forms.Button();
            this.btn_clr = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btn_0 = new VenusRetailPOSServer.NoSelectButton();
            this.btn_3 = new VenusRetailPOSServer.NoSelectButton();
            this.btn_2 = new VenusRetailPOSServer.NoSelectButton();
            this.btn_1 = new VenusRetailPOSServer.NoSelectButton();
            this.btn_6 = new VenusRetailPOSServer.NoSelectButton();
            this.btn_5 = new VenusRetailPOSServer.NoSelectButton();
            this.btn_4 = new VenusRetailPOSServer.NoSelectButton();
            this.btn_9 = new VenusRetailPOSServer.NoSelectButton();
            this.btn_8 = new VenusRetailPOSServer.NoSelectButton();
            this.btn_7 = new VenusRetailPOSServer.NoSelectButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(122, 170);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 12);
            this.label2.TabIndex = 94;
            this.label2.Text = "Venus Retail POS Server";
            // 
            // lbl_notif
            // 
            this.lbl_notif.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_notif.Location = new System.Drawing.Point(57, 170);
            this.lbl_notif.Name = "lbl_notif";
            this.lbl_notif.Size = new System.Drawing.Size(226, 46);
            this.lbl_notif.TabIndex = 93;
            this.lbl_notif.Text = "lbl_notif";
            this.lbl_notif.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lbl_datetime
            // 
            this.lbl_datetime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_datetime.Location = new System.Drawing.Point(13, 145);
            this.lbl_datetime.Name = "lbl_datetime";
            this.lbl_datetime.Size = new System.Drawing.Size(315, 15);
            this.lbl_datetime.TabIndex = 92;
            this.lbl_datetime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Image = global::VenusRetailPOSServer.Properties.Resources.company_logo_modified;
            this.pictureBox1.Location = new System.Drawing.Point(110, 22);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(120, 120);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 91;
            this.pictureBox1.TabStop = false;
            // 
            // txt_preview
            // 
            this.txt_preview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_preview.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_preview.Location = new System.Drawing.Point(57, 219);
            this.txt_preview.Name = "txt_preview";
            this.txt_preview.Size = new System.Drawing.Size(226, 32);
            this.txt_preview.TabIndex = 90;
            this.txt_preview.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_preview.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CheckEnter);
            // 
            // btn_enter
            // 
            this.btn_enter.BackColor = System.Drawing.Color.LimeGreen;
            this.btn_enter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_enter.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_enter.Location = new System.Drawing.Point(225, 255);
            this.btn_enter.Margin = new System.Windows.Forms.Padding(1);
            this.btn_enter.Name = "btn_enter";
            this.btn_enter.Size = new System.Drawing.Size(58, 110);
            this.btn_enter.TabIndex = 97;
            this.btn_enter.Text = "Enter";
            this.btn_enter.UseVisualStyleBackColor = false;
            this.btn_enter.Click += new System.EventHandler(this.btn_enter_Click);
            // 
            // btn_clr
            // 
            this.btn_clr.BackColor = System.Drawing.Color.DimGray;
            this.btn_clr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_clr.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_clr.Location = new System.Drawing.Point(113, 423);
            this.btn_clr.Margin = new System.Windows.Forms.Padding(1);
            this.btn_clr.Name = "btn_clr";
            this.btn_clr.Size = new System.Drawing.Size(110, 54);
            this.btn_clr.TabIndex = 105;
            this.btn_clr.Text = "Clear";
            this.btn_clr.UseVisualStyleBackColor = false;
            this.btn_clr.Click += new System.EventHandler(this.btn_clr_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btn_0
            // 
            this.btn_0.BackColor = System.Drawing.Color.Yellow;
            this.btn_0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_0.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_0.Location = new System.Drawing.Point(57, 423);
            this.btn_0.Margin = new System.Windows.Forms.Padding(1);
            this.btn_0.Name = "btn_0";
            this.btn_0.Size = new System.Drawing.Size(54, 54);
            this.btn_0.TabIndex = 115;
            this.btn_0.Tag = "0";
            this.btn_0.Text = "0";
            this.btn_0.UseVisualStyleBackColor = false;
            this.btn_0.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // btn_3
            // 
            this.btn_3.BackColor = System.Drawing.Color.Yellow;
            this.btn_3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_3.Location = new System.Drawing.Point(169, 367);
            this.btn_3.Margin = new System.Windows.Forms.Padding(1);
            this.btn_3.Name = "btn_3";
            this.btn_3.Size = new System.Drawing.Size(54, 54);
            this.btn_3.TabIndex = 114;
            this.btn_3.Tag = "3";
            this.btn_3.Text = "3";
            this.btn_3.UseVisualStyleBackColor = false;
            this.btn_3.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // btn_2
            // 
            this.btn_2.BackColor = System.Drawing.Color.Yellow;
            this.btn_2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_2.Location = new System.Drawing.Point(113, 367);
            this.btn_2.Margin = new System.Windows.Forms.Padding(1);
            this.btn_2.Name = "btn_2";
            this.btn_2.Size = new System.Drawing.Size(54, 54);
            this.btn_2.TabIndex = 113;
            this.btn_2.Tag = "2";
            this.btn_2.Text = "2";
            this.btn_2.UseVisualStyleBackColor = false;
            this.btn_2.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // btn_1
            // 
            this.btn_1.BackColor = System.Drawing.Color.Yellow;
            this.btn_1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_1.Location = new System.Drawing.Point(57, 367);
            this.btn_1.Margin = new System.Windows.Forms.Padding(1);
            this.btn_1.Name = "btn_1";
            this.btn_1.Size = new System.Drawing.Size(54, 54);
            this.btn_1.TabIndex = 112;
            this.btn_1.Tag = "1";
            this.btn_1.Text = "1";
            this.btn_1.UseVisualStyleBackColor = false;
            this.btn_1.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // btn_6
            // 
            this.btn_6.BackColor = System.Drawing.Color.Yellow;
            this.btn_6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_6.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_6.Location = new System.Drawing.Point(169, 311);
            this.btn_6.Margin = new System.Windows.Forms.Padding(1);
            this.btn_6.Name = "btn_6";
            this.btn_6.Size = new System.Drawing.Size(54, 54);
            this.btn_6.TabIndex = 111;
            this.btn_6.Tag = "6";
            this.btn_6.Text = "6";
            this.btn_6.UseVisualStyleBackColor = false;
            this.btn_6.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // btn_5
            // 
            this.btn_5.BackColor = System.Drawing.Color.Yellow;
            this.btn_5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_5.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_5.Location = new System.Drawing.Point(113, 311);
            this.btn_5.Margin = new System.Windows.Forms.Padding(1);
            this.btn_5.Name = "btn_5";
            this.btn_5.Size = new System.Drawing.Size(54, 54);
            this.btn_5.TabIndex = 110;
            this.btn_5.Tag = "5";
            this.btn_5.Text = "5";
            this.btn_5.UseVisualStyleBackColor = false;
            this.btn_5.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // btn_4
            // 
            this.btn_4.BackColor = System.Drawing.Color.Yellow;
            this.btn_4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_4.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_4.Location = new System.Drawing.Point(57, 311);
            this.btn_4.Margin = new System.Windows.Forms.Padding(1);
            this.btn_4.Name = "btn_4";
            this.btn_4.Size = new System.Drawing.Size(54, 54);
            this.btn_4.TabIndex = 109;
            this.btn_4.Tag = "4";
            this.btn_4.Text = "4";
            this.btn_4.UseVisualStyleBackColor = false;
            this.btn_4.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // btn_9
            // 
            this.btn_9.BackColor = System.Drawing.Color.Yellow;
            this.btn_9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_9.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_9.Location = new System.Drawing.Point(169, 255);
            this.btn_9.Margin = new System.Windows.Forms.Padding(1);
            this.btn_9.Name = "btn_9";
            this.btn_9.Size = new System.Drawing.Size(54, 54);
            this.btn_9.TabIndex = 108;
            this.btn_9.Tag = "9";
            this.btn_9.Text = "9";
            this.btn_9.UseVisualStyleBackColor = false;
            this.btn_9.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // btn_8
            // 
            this.btn_8.BackColor = System.Drawing.Color.Yellow;
            this.btn_8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_8.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_8.Location = new System.Drawing.Point(113, 255);
            this.btn_8.Margin = new System.Windows.Forms.Padding(1);
            this.btn_8.Name = "btn_8";
            this.btn_8.Size = new System.Drawing.Size(54, 54);
            this.btn_8.TabIndex = 107;
            this.btn_8.Tag = "8";
            this.btn_8.Text = "8";
            this.btn_8.UseVisualStyleBackColor = false;
            this.btn_8.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // btn_7
            // 
            this.btn_7.BackColor = System.Drawing.Color.Yellow;
            this.btn_7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_7.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_7.Location = new System.Drawing.Point(57, 255);
            this.btn_7.Margin = new System.Windows.Forms.Padding(1);
            this.btn_7.Name = "btn_7";
            this.btn_7.Size = new System.Drawing.Size(54, 54);
            this.btn_7.TabIndex = 106;
            this.btn_7.Tag = "7";
            this.btn_7.Text = "7";
            this.btn_7.UseVisualStyleBackColor = false;
            this.btn_7.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 493);
            this.Controls.Add(this.btn_0);
            this.Controls.Add(this.btn_3);
            this.Controls.Add(this.btn_2);
            this.Controls.Add(this.btn_1);
            this.Controls.Add(this.btn_6);
            this.Controls.Add(this.btn_5);
            this.Controls.Add(this.btn_4);
            this.Controls.Add(this.btn_9);
            this.Controls.Add(this.btn_8);
            this.Controls.Add(this.btn_7);
            this.Controls.Add(this.btn_clr);
            this.Controls.Add(this.btn_enter);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbl_notif);
            this.Controls.Add(this.lbl_datetime);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.txt_preview);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.Load += new System.EventHandler(this.Login_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_notif;
        private System.Windows.Forms.Label lbl_datetime;
        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.TextBox txt_preview;
        private System.Windows.Forms.Button btn_enter;
        private System.Windows.Forms.Button btn_clr;
        private System.Windows.Forms.Timer timer1;
        private NoSelectButton btn_7;
        private NoSelectButton btn_8;
        private NoSelectButton btn_9;
        private NoSelectButton btn_4;
        private NoSelectButton btn_5;
        private NoSelectButton btn_6;
        private NoSelectButton btn_1;
        private NoSelectButton btn_2;
        private NoSelectButton btn_3;
        private NoSelectButton btn_0;
    }
}