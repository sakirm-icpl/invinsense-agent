using System.Windows.Forms;

namespace IvsTray
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.pbDbytes = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pbSysmon = new System.Windows.Forms.PictureBox();
            this.pbDefender = new System.Windows.Forms.PictureBox();
            this.pbOsquery = new System.Windows.Forms.PictureBox();
            this.pbLmp = new System.Windows.Forms.PictureBox();
            this.lblWindowsDefender = new System.Windows.Forms.Label();
            this.lblWazuh = new System.Windows.Forms.Label();
            this.lblMicrosoftSysmon = new System.Windows.Forms.Label();
            this.lblLmp = new System.Windows.Forms.Label();
            this.lblOsquery = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.lblDeceptiveBytes = new System.Windows.Forms.Label();
            this.pbWazuh = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pbDbytes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSysmon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDefender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOsquery)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLmp)).BeginInit();
            this.tlpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbWazuh)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Invinsense 3.0";
            this.notifyIcon.Visible = true;
            this.notifyIcon.Click += new System.EventHandler(this.ShowClick);
            // 
            // pbDbytes
            // 
            this.pbDbytes.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pbDbytes.Image = global::IvsTray.Properties.Resources.gray;
            this.pbDbytes.InitialImage = null;
            this.pbDbytes.Location = new System.Drawing.Point(330, 14);
            this.pbDbytes.Name = "pbDbytes";
            this.pbDbytes.Size = new System.Drawing.Size(12, 12);
            this.pbDbytes.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbDbytes.TabIndex = 0;
            this.pbDbytes.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::IvsTray.Properties.Resources.invinsence_logo;
            this.pictureBox1.Location = new System.Drawing.Point(18, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(266, 54);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // pbSysmon
            // 
            this.pbSysmon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pbSysmon.Image = global::IvsTray.Properties.Resources.gray;
            this.pbSysmon.InitialImage = null;
            this.pbSysmon.Location = new System.Drawing.Point(330, 174);
            this.pbSysmon.Name = "pbSysmon";
            this.pbSysmon.Size = new System.Drawing.Size(12, 12);
            this.pbSysmon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbSysmon.TabIndex = 4;
            this.pbSysmon.TabStop = false;
            // 
            // pbDefender
            // 
            this.pbDefender.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pbDefender.Image = global::IvsTray.Properties.Resources.gray;
            this.pbDefender.InitialImage = null;
            this.pbDefender.Location = new System.Drawing.Point(330, 54);
            this.pbDefender.Name = "pbDefender";
            this.pbDefender.Size = new System.Drawing.Size(12, 12);
            this.pbDefender.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbDefender.TabIndex = 1;
            this.pbDefender.TabStop = false;
            // 
            // pbOsquery
            // 
            this.pbOsquery.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pbOsquery.Image = global::IvsTray.Properties.Resources.gray;
            this.pbOsquery.InitialImage = null;
            this.pbOsquery.Location = new System.Drawing.Point(330, 94);
            this.pbOsquery.Name = "pbOsquery";
            this.pbOsquery.Size = new System.Drawing.Size(12, 12);
            this.pbOsquery.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbOsquery.TabIndex = 2;
            this.pbOsquery.TabStop = false;
            // 
            // pbLmp
            // 
            this.pbLmp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pbLmp.Image = global::IvsTray.Properties.Resources.gray;
            this.pbLmp.InitialImage = null;
            this.pbLmp.Location = new System.Drawing.Point(330, 214);
            this.pbLmp.Name = "pbLmp";
            this.pbLmp.Size = new System.Drawing.Size(12, 12);
            this.pbLmp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbLmp.TabIndex = 5;
            this.pbLmp.TabStop = false;
            // 
            // lblWindowsDefender
            // 
            this.lblWindowsDefender.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblWindowsDefender.AutoSize = true;
            this.lblWindowsDefender.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWindowsDefender.Location = new System.Drawing.Point(3, 50);
            this.lblWindowsDefender.Name = "lblWindowsDefender";
            this.lblWindowsDefender.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblWindowsDefender.Size = new System.Drawing.Size(140, 19);
            this.lblWindowsDefender.TabIndex = 8;
            this.lblWindowsDefender.Text = "End Point Protection";
            // 
            // lblWazuh
            // 
            this.lblWazuh.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblWazuh.AutoSize = true;
            this.lblWazuh.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWazuh.Location = new System.Drawing.Point(3, 130);
            this.lblWazuh.Name = "lblWazuh";
            this.lblWazuh.Size = new System.Drawing.Size(232, 19);
            this.lblWazuh.TabIndex = 9;
            this.lblWazuh.Text = "End Point Detection and Response";
            // 
            // lblMicrosoftSysmon
            // 
            this.lblMicrosoftSysmon.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblMicrosoftSysmon.AutoSize = true;
            this.lblMicrosoftSysmon.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMicrosoftSysmon.Location = new System.Drawing.Point(3, 170);
            this.lblMicrosoftSysmon.Name = "lblMicrosoftSysmon";
            this.lblMicrosoftSysmon.Size = new System.Drawing.Size(132, 19);
            this.lblMicrosoftSysmon.TabIndex = 10;
            this.lblMicrosoftSysmon.Text = "Advance Telemetry";
            // 
            // lblLmp
            // 
            this.lblLmp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLmp.AutoSize = true;
            this.lblLmp.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLmp.Location = new System.Drawing.Point(3, 210);
            this.lblLmp.Name = "lblLmp";
            this.lblLmp.Size = new System.Drawing.Size(197, 19);
            this.lblLmp.TabIndex = 11;
            this.lblLmp.Text = "Lateral Movement Protection";
            // 
            // lblOsquery
            // 
            this.lblOsquery.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOsquery.AutoSize = true;
            this.lblOsquery.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOsquery.Location = new System.Drawing.Point(3, 90);
            this.lblOsquery.Name = "lblOsquery";
            this.lblOsquery.Size = new System.Drawing.Size(171, 19);
            this.lblOsquery.TabIndex = 9;
            this.lblOsquery.Text = "User Behaviour Analytics";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Font = new System.Drawing.Font("Calibri", 33F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.Green;
            this.lblVersion.Location = new System.Drawing.Point(294, 11);
            this.lblVersion.Margin = new System.Windows.Forms.Padding(0);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(105, 54);
            this.lblVersion.TabIndex = 17;
            this.lblVersion.Text = "V4.0";
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 296F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpMain.Controls.Add(this.lblDeceptiveBytes, 0, 0);
            this.tlpMain.Controls.Add(this.lblWindowsDefender, 0, 1);
            this.tlpMain.Controls.Add(this.lblOsquery, 0, 2);
            this.tlpMain.Controls.Add(this.lblWazuh, 0, 3);
            this.tlpMain.Controls.Add(this.lblMicrosoftSysmon, 0, 4);
            this.tlpMain.Controls.Add(this.lblLmp, 0, 5);
            this.tlpMain.Controls.Add(this.pbDefender, 1, 1);
            this.tlpMain.Controls.Add(this.pbOsquery, 1, 2);
            this.tlpMain.Controls.Add(this.pbWazuh, 1, 3);
            this.tlpMain.Controls.Add(this.pbSysmon, 1, 4);
            this.tlpMain.Controls.Add(this.pbLmp, 1, 5);
            this.tlpMain.Controls.Add(this.pbDbytes, 1, 0);
            this.tlpMain.Location = new System.Drawing.Point(12, 9);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 6;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpMain.Size = new System.Drawing.Size(376, 240);
            this.tlpMain.TabIndex = 18;
            // 
            // lblDeceptiveBytes
            // 
            this.lblDeceptiveBytes.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDeceptiveBytes.AutoSize = true;
            this.lblDeceptiveBytes.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDeceptiveBytes.Location = new System.Drawing.Point(3, 10);
            this.lblDeceptiveBytes.Name = "lblDeceptiveBytes";
            this.lblDeceptiveBytes.Size = new System.Drawing.Size(237, 19);
            this.lblDeceptiveBytes.TabIndex = 7;
            this.lblDeceptiveBytes.Text = "End Point Deception [0.2% / 20MB]";
            // 
            // pbWazuh
            // 
            this.pbWazuh.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pbWazuh.Image = global::IvsTray.Properties.Resources.gray;
            this.pbWazuh.InitialImage = null;
            this.pbWazuh.Location = new System.Drawing.Point(330, 134);
            this.pbWazuh.Name = "pbWazuh";
            this.pbWazuh.Size = new System.Drawing.Size(12, 12);
            this.pbWazuh.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbWazuh.TabIndex = 3;
            this.pbWazuh.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LightGray;
            this.panel1.Controls.Add(this.lblVersion);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 250);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 90);
            this.panel1.TabIndex = 19;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 340);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tlpMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Invinsense 3.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormClosing);
            this.Load += new System.EventHandler(this.MainFormOnLoad);
            ((System.ComponentModel.ISupportInitialize)(this.pbDbytes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSysmon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDefender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOsquery)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLmp)).EndInit();
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbWazuh)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private NotifyIcon notifyIcon;
        private PictureBox pbDbytes;
        private PictureBox pictureBox1;
        private PictureBox pbSysmon;
        private PictureBox pbDefender;
        private PictureBox pbOsquery;
        private PictureBox pbLmp;
        private PictureBox pbInvinsense;

        public MainForm(PictureBox pbInvinsense)
        {
            this.pbInvinsense = pbInvinsense;
        }
        private Label lblWindowsDefender;
        private Label lblWazuh;
        private Label lblMicrosoftSysmon;
        private Label lblLmp;
        private Label lblOsquery;
        private Label lblVersion;
        private TableLayoutPanel tlpMain;
        private PictureBox pbWazuh;
        private Label lblDeceptiveBytes;
        private Panel panel1;
    }
}