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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.brandingPanel = new System.Windows.Forms.Panel();
            this.tsc = new IvsTray.UserControls.ToolStatusContainer();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.brandingPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Invinsense 4.0";
            this.notifyIcon.Visible = true;
            this.notifyIcon.Click += new System.EventHandler(this.ShowClick);
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
            // brandingPanel
            // 
            this.brandingPanel.BackColor = System.Drawing.Color.LightGray;
            this.brandingPanel.Controls.Add(this.lblVersion);
            this.brandingPanel.Controls.Add(this.pictureBox1);
            this.brandingPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.brandingPanel.Location = new System.Drawing.Point(0, 20);
            this.brandingPanel.Name = "brandingPanel";
            this.brandingPanel.Size = new System.Drawing.Size(400, 90);
            this.brandingPanel.TabIndex = 19;
            // 
            // tsc
            // 
            this.tsc.Dock = System.Windows.Forms.DockStyle.Top;
            this.tsc.Location = new System.Drawing.Point(0, 0);
            this.tsc.Name = "tsc";
            this.tsc.Size = new System.Drawing.Size(400, 20);
            this.tsc.TabIndex = 20;
            this.tsc.SizeChanged += new System.EventHandler(this.ToolContainerSizeChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 110);
            this.Controls.Add(this.tsc);
            this.Controls.Add(this.brandingPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Invinsense 4.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.MainFormOnLoad);
            this.DpiChanged += new System.Windows.Forms.DpiChangedEventHandler(this.OnDpiChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.brandingPanel.ResumeLayout(false);
            this.brandingPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private NotifyIcon notifyIcon;
        private PictureBox pictureBox1;
        private PictureBox pbInvinsense;

        public MainForm(PictureBox pbInvinsense)
        {
            this.pbInvinsense = pbInvinsense;
        }
        private Label lblVersion;
        private Panel brandingPanel;
        private UserControls.ToolStatusContainer tsc;
    }
}