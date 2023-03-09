namespace HardwareInformation
{
    partial class Form1
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ProcId = new System.Windows.Forms.Button();
            this.HDDSNo = new System.Windows.Forms.Button();
            this.BoardMake = new System.Windows.Forms.Button();
            this.BIOSMkr = new System.Windows.Forms.Button();
            this.PhysicalMem = new System.Windows.Forms.Button();
            this.CPUSpeed = new System.Windows.Forms.Button();
            this.CPUMkr = new System.Windows.Forms.Button();
            this.lblPid = new System.Windows.Forms.Label();
            this.lblHDD = new System.Windows.Forms.Label();
            this.lblBM = new System.Windows.Forms.Label();
            this.lblBios = new System.Windows.Forms.Label();
            this.lblPM = new System.Windows.Forms.Label();
            this.lblCS = new System.Windows.Forms.Label();
            this.lblCM = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblCM);
            this.groupBox1.Controls.Add(this.lblCS);
            this.groupBox1.Controls.Add(this.lblPM);
            this.groupBox1.Controls.Add(this.lblBios);
            this.groupBox1.Controls.Add(this.lblBM);
            this.groupBox1.Controls.Add(this.lblHDD);
            this.groupBox1.Controls.Add(this.lblPid);
            this.groupBox1.Controls.Add(this.CPUMkr);
            this.groupBox1.Controls.Add(this.CPUSpeed);
            this.groupBox1.Controls.Add(this.PhysicalMem);
            this.groupBox1.Controls.Add(this.BIOSMkr);
            this.groupBox1.Controls.Add(this.BoardMake);
            this.groupBox1.Controls.Add(this.HDDSNo);
            this.groupBox1.Controls.Add(this.ProcId);
            this.groupBox1.Location = new System.Drawing.Point(9, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(525, 285);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Hardware Information";
            // 
            // ProcId
            // 
            this.ProcId.Location = new System.Drawing.Point(40, 19);
            this.ProcId.Name = "ProcId";
            this.ProcId.Size = new System.Drawing.Size(101, 23);
            this.ProcId.TabIndex = 0;
            this.ProcId.Text = "Processor Id";
            this.ProcId.UseVisualStyleBackColor = true;
            this.ProcId.Click += new System.EventHandler(this.ProcId_Click);
            // 
            // HDDSNo
            // 
            this.HDDSNo.Location = new System.Drawing.Point(40, 59);
            this.HDDSNo.Name = "HDDSNo";
            this.HDDSNo.Size = new System.Drawing.Size(101, 23);
            this.HDDSNo.TabIndex = 1;
            this.HDDSNo.Text = "HDD Serail No";
            this.HDDSNo.UseVisualStyleBackColor = true;
            this.HDDSNo.Click += new System.EventHandler(this.HDDSNo_Click);
            // 
            // BoardMake
            // 
            this.BoardMake.Location = new System.Drawing.Point(40, 99);
            this.BoardMake.Name = "BoardMake";
            this.BoardMake.Size = new System.Drawing.Size(101, 23);
            this.BoardMake.TabIndex = 2;
            this.BoardMake.Text = "Board Maker";
            this.BoardMake.UseVisualStyleBackColor = true;
            this.BoardMake.Click += new System.EventHandler(this.BoardMake_Click);
            // 
            // BIOSMkr
            // 
            this.BIOSMkr.Location = new System.Drawing.Point(40, 139);
            this.BIOSMkr.Name = "BIOSMkr";
            this.BIOSMkr.Size = new System.Drawing.Size(101, 23);
            this.BIOSMkr.TabIndex = 3;
            this.BIOSMkr.Text = "BIOS Maker";
            this.BIOSMkr.UseVisualStyleBackColor = true;
            this.BIOSMkr.Click += new System.EventHandler(this.BIOSMkr_Click);
            // 
            // PhysicalMem
            // 
            this.PhysicalMem.Location = new System.Drawing.Point(40, 179);
            this.PhysicalMem.Name = "PhysicalMem";
            this.PhysicalMem.Size = new System.Drawing.Size(101, 23);
            this.PhysicalMem.TabIndex = 4;
            this.PhysicalMem.Text = "Physical Memory";
            this.PhysicalMem.UseVisualStyleBackColor = true;
            this.PhysicalMem.Click += new System.EventHandler(this.PhysicalMem_Click);
            // 
            // CPUSpeed
            // 
            this.CPUSpeed.Location = new System.Drawing.Point(40, 219);
            this.CPUSpeed.Name = "CPUSpeed";
            this.CPUSpeed.Size = new System.Drawing.Size(101, 23);
            this.CPUSpeed.TabIndex = 5;
            this.CPUSpeed.Text = "CPU Speed";
            this.CPUSpeed.UseVisualStyleBackColor = true;
            this.CPUSpeed.Click += new System.EventHandler(this.CPUSpeed_Click);
            // 
            // CPUMkr
            // 
            this.CPUMkr.Location = new System.Drawing.Point(40, 256);
            this.CPUMkr.Name = "CPUMkr";
            this.CPUMkr.Size = new System.Drawing.Size(101, 23);
            this.CPUMkr.TabIndex = 6;
            this.CPUMkr.Text = "CPU Maker";
            this.CPUMkr.UseVisualStyleBackColor = true;
            this.CPUMkr.Click += new System.EventHandler(this.CPUMkr_Click);
            // 
            // lblPid
            // 
            this.lblPid.AutoSize = true;
            this.lblPid.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPid.ForeColor = System.Drawing.Color.DarkRed;
            this.lblPid.Location = new System.Drawing.Point(161, 24);
            this.lblPid.Name = "lblPid";
            this.lblPid.Size = new System.Drawing.Size(51, 16);
            this.lblPid.TabIndex = 7;
            this.lblPid.Text = "label1";
            // 
            // lblHDD
            // 
            this.lblHDD.AutoSize = true;
            this.lblHDD.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHDD.ForeColor = System.Drawing.Color.DarkRed;
            this.lblHDD.Location = new System.Drawing.Point(161, 63);
            this.lblHDD.Name = "lblHDD";
            this.lblHDD.Size = new System.Drawing.Size(51, 16);
            this.lblHDD.TabIndex = 8;
            this.lblHDD.Text = "label2";
            // 
            // lblBM
            // 
            this.lblBM.AutoSize = true;
            this.lblBM.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBM.ForeColor = System.Drawing.Color.DarkRed;
            this.lblBM.Location = new System.Drawing.Point(161, 102);
            this.lblBM.Name = "lblBM";
            this.lblBM.Size = new System.Drawing.Size(51, 16);
            this.lblBM.TabIndex = 9;
            this.lblBM.Text = "label3";
            // 
            // lblBios
            // 
            this.lblBios.AutoSize = true;
            this.lblBios.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBios.ForeColor = System.Drawing.Color.DarkRed;
            this.lblBios.Location = new System.Drawing.Point(161, 141);
            this.lblBios.Name = "lblBios";
            this.lblBios.Size = new System.Drawing.Size(51, 16);
            this.lblBios.TabIndex = 10;
            this.lblBios.Text = "label4";
            // 
            // lblPM
            // 
            this.lblPM.AutoSize = true;
            this.lblPM.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPM.ForeColor = System.Drawing.Color.DarkRed;
            this.lblPM.Location = new System.Drawing.Point(161, 180);
            this.lblPM.Name = "lblPM";
            this.lblPM.Size = new System.Drawing.Size(51, 16);
            this.lblPM.TabIndex = 11;
            this.lblPM.Text = "label5";
            // 
            // lblCS
            // 
            this.lblCS.AutoSize = true;
            this.lblCS.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCS.ForeColor = System.Drawing.Color.DarkRed;
            this.lblCS.Location = new System.Drawing.Point(161, 219);
            this.lblCS.Name = "lblCS";
            this.lblCS.Size = new System.Drawing.Size(51, 16);
            this.lblCS.TabIndex = 12;
            this.lblCS.Text = "label6";
            // 
            // lblCM
            // 
            this.lblCM.AutoSize = true;
            this.lblCM.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCM.ForeColor = System.Drawing.Color.DarkRed;
            this.lblCM.Location = new System.Drawing.Point(161, 258);
            this.lblCM.Name = "lblCM";
            this.lblCM.Size = new System.Drawing.Size(51, 16);
            this.lblCM.TabIndex = 13;
            this.lblCM.Text = "label7";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 301);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HardwareInfo";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button BIOSMkr;
        private System.Windows.Forms.Button BoardMake;
        private System.Windows.Forms.Button HDDSNo;
        private System.Windows.Forms.Button ProcId;
        private System.Windows.Forms.Button CPUSpeed;
        private System.Windows.Forms.Button PhysicalMem;
        private System.Windows.Forms.Label lblCM;
        private System.Windows.Forms.Label lblCS;
        private System.Windows.Forms.Label lblPM;
        private System.Windows.Forms.Label lblBios;
        private System.Windows.Forms.Label lblBM;
        private System.Windows.Forms.Label lblHDD;
        private System.Windows.Forms.Label lblPid;
        private System.Windows.Forms.Button CPUMkr;
    }
}

