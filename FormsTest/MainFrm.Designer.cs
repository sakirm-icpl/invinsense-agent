namespace FormsTest
{
    partial class MainFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
            this.NotifyTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.ToolTipButton = new System.Windows.Forms.Button();
            this.readFirstFile = new System.Windows.Forms.Button();
            this.lblConfigValue = new System.Windows.Forms.Label();
            this.lblCulture = new System.Windows.Forms.Label();
            this.readSecondFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // NotifyTrayIcon
            // 
            this.NotifyTrayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            resources.ApplyResources(this.NotifyTrayIcon, "NotifyTrayIcon");
            // 
            // ToolTipButton
            // 
            resources.ApplyResources(this.ToolTipButton, "ToolTipButton");
            this.ToolTipButton.Name = "ToolTipButton";
            this.ToolTipButton.UseVisualStyleBackColor = true;
            this.ToolTipButton.Click += new System.EventHandler(this.ShowToolTipClick);
            // 
            // readFirstFile
            // 
            resources.ApplyResources(this.readFirstFile, "readFirstFile");
            this.readFirstFile.Name = "readFirstFile";
            this.readFirstFile.UseVisualStyleBackColor = true;
            this.readFirstFile.Click += new System.EventHandler(this.ReadFirstFileClick);
            // 
            // lblConfigValue
            // 
            resources.ApplyResources(this.lblConfigValue, "lblConfigValue");
            this.lblConfigValue.Name = "lblConfigValue";
            // 
            // lblCulture
            // 
            resources.ApplyResources(this.lblCulture, "lblCulture");
            this.lblCulture.Name = "lblCulture";
            // 
            // readSecondFile
            // 
            resources.ApplyResources(this.readSecondFile, "readSecondFile");
            this.readSecondFile.Name = "readSecondFile";
            this.readSecondFile.UseVisualStyleBackColor = true;
            this.readSecondFile.Click += new System.EventHandler(this.ReadSecondFileClick);
            // 
            // MainFrm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.readSecondFile);
            this.Controls.Add(this.lblCulture);
            this.Controls.Add(this.lblConfigValue);
            this.Controls.Add(this.readFirstFile);
            this.Controls.Add(this.ToolTipButton);
            this.Name = "MainFrm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon NotifyTrayIcon;
        private System.Windows.Forms.Button ToolTipButton;
        private System.Windows.Forms.Button readFirstFile;
        private System.Windows.Forms.Label lblConfigValue;
        private System.Windows.Forms.Label lblCulture;
        private System.Windows.Forms.Button readSecondFile;
    }
}

