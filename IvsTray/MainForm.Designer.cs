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
            this.tsc = new IvsTray.UserControls.ToolStatusContainer();
            this.fuc = new IvsTray.UserControls.FooterUserControl();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.Text = Common.Constants.BrandName;
            this.notifyIcon.Visible = true;
            this.notifyIcon.Click += new System.EventHandler(this.ShowClick);
            // 
            // tsc
            // 
            this.tsc.Dock = System.Windows.Forms.DockStyle.Top;
            this.tsc.Location = new System.Drawing.Point(0, 0);
            this.tsc.Name = "tsc";
            this.tsc.Size = new System.Drawing.Size(350, 40);
            this.tsc.TabIndex = 20;
            this.tsc.SizeChanged += new System.EventHandler(this.ToolContainerSizeChanged);
            // 
            // fuc
            // 
            this.fuc.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.fuc.Location = new System.Drawing.Point(0, 40);
            this.fuc.Name = "fuc";
            this.fuc.Size = new System.Drawing.Size(350, 80);
            this.fuc.TabIndex = 21;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(350, 120);
            this.Controls.Add(this.fuc);
            this.Controls.Add(this.tsc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = Common.Constants.IvsTrayName;
            this.ShowInTaskbar = false;
            this.Text = Common.Constants.BrandName;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.MainFormOnLoad);
            this.DpiChanged += new System.Windows.Forms.DpiChangedEventHandler(this.OnDpiChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private NotifyIcon notifyIcon;
        private PictureBox pbInvinsense;

        public MainForm(PictureBox pbInvinsense)
        {
            this.pbInvinsense = pbInvinsense;
        }
        private UserControls.ToolStatusContainer tsc;
        private UserControls.FooterUserControl fuc;
    }
}