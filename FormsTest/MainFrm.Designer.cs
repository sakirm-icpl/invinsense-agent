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
            this.btnReadDefault = new System.Windows.Forms.Button();
            this.lblConfigValue = new System.Windows.Forms.Label();
            this.lblCulture = new System.Windows.Forms.Label();
            this.btnReadSingapore = new System.Windows.Forms.Button();
            this.restartButton = new System.Windows.Forms.Button();
            this.btnReadPhilippines = new System.Windows.Forms.Button();
            this.btnReadThailand = new System.Windows.Forms.Button();
            this.btnReadVietnam = new System.Windows.Forms.Button();
            this.btnReadIndonesia = new System.Windows.Forms.Button();
            this.btnReadMalaysia = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.showDialogButton = new System.Windows.Forms.Button();
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
            // btnReadDefault
            // 
            resources.ApplyResources(this.btnReadDefault, "btnReadDefault");
            this.btnReadDefault.Name = "btnReadDefault";
            this.btnReadDefault.Tag = "";
            this.btnReadDefault.UseVisualStyleBackColor = true;
            this.btnReadDefault.Click += new System.EventHandler(this.ReadConfigFileClick);
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
            // btnReadSingapore
            // 
            resources.ApplyResources(this.btnReadSingapore, "btnReadSingapore");
            this.btnReadSingapore.Name = "btnReadSingapore";
            this.btnReadSingapore.Tag = "en-SG";
            this.btnReadSingapore.UseVisualStyleBackColor = true;
            this.btnReadSingapore.Click += new System.EventHandler(this.ReadConfigFileClick);
            // 
            // restartButton
            // 
            resources.ApplyResources(this.restartButton, "restartButton");
            this.restartButton.Name = "restartButton";
            this.restartButton.UseVisualStyleBackColor = true;
            this.restartButton.Click += new System.EventHandler(this.RestartButtonClick);
            // 
            // btnReadPhilippines
            // 
            resources.ApplyResources(this.btnReadPhilippines, "btnReadPhilippines");
            this.btnReadPhilippines.Name = "btnReadPhilippines";
            this.btnReadPhilippines.Tag = "fil-PH";
            this.btnReadPhilippines.UseVisualStyleBackColor = true;
            this.btnReadPhilippines.Click += new System.EventHandler(this.ReadConfigFileClick);
            // 
            // btnReadThailand
            // 
            resources.ApplyResources(this.btnReadThailand, "btnReadThailand");
            this.btnReadThailand.Name = "btnReadThailand";
            this.btnReadThailand.Tag = "th-TH";
            this.btnReadThailand.UseVisualStyleBackColor = true;
            this.btnReadThailand.Click += new System.EventHandler(this.ReadConfigFileClick);
            // 
            // btnReadVietnam
            // 
            resources.ApplyResources(this.btnReadVietnam, "btnReadVietnam");
            this.btnReadVietnam.Name = "btnReadVietnam";
            this.btnReadVietnam.Tag = "vi-VN";
            this.btnReadVietnam.UseVisualStyleBackColor = true;
            this.btnReadVietnam.Click += new System.EventHandler(this.ReadConfigFileClick);
            // 
            // btnReadIndonesia
            // 
            resources.ApplyResources(this.btnReadIndonesia, "btnReadIndonesia");
            this.btnReadIndonesia.Name = "btnReadIndonesia";
            this.btnReadIndonesia.Tag = "id-ID";
            this.btnReadIndonesia.UseVisualStyleBackColor = true;
            this.btnReadIndonesia.Click += new System.EventHandler(this.ReadConfigFileClick);
            // 
            // btnReadMalaysia
            // 
            resources.ApplyResources(this.btnReadMalaysia, "btnReadMalaysia");
            this.btnReadMalaysia.Name = "btnReadMalaysia";
            this.btnReadMalaysia.Tag = "ms-MY";
            this.btnReadMalaysia.UseVisualStyleBackColor = true;
            this.btnReadMalaysia.Click += new System.EventHandler(this.ReadConfigFileClick);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // showDialogButton
            // 
            resources.ApplyResources(this.showDialogButton, "showDialogButton");
            this.showDialogButton.Name = "showDialogButton";
            this.showDialogButton.UseVisualStyleBackColor = true;
            this.showDialogButton.Click += new System.EventHandler(this.ShowDialogButtonClick);
            // 
            // MainFrm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.showDialogButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnReadMalaysia);
            this.Controls.Add(this.btnReadIndonesia);
            this.Controls.Add(this.btnReadVietnam);
            this.Controls.Add(this.btnReadThailand);
            this.Controls.Add(this.btnReadPhilippines);
            this.Controls.Add(this.restartButton);
            this.Controls.Add(this.btnReadSingapore);
            this.Controls.Add(this.lblCulture);
            this.Controls.Add(this.lblConfigValue);
            this.Controls.Add(this.btnReadDefault);
            this.Controls.Add(this.ToolTipButton);
            this.Name = "MainFrm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon NotifyTrayIcon;
        private System.Windows.Forms.Button ToolTipButton;
        private System.Windows.Forms.Button btnReadDefault;
        private System.Windows.Forms.Label lblConfigValue;
        private System.Windows.Forms.Label lblCulture;
        private System.Windows.Forms.Button btnReadSingapore;
        private System.Windows.Forms.Button restartButton;
        private System.Windows.Forms.Button btnReadPhilippines;
        private System.Windows.Forms.Button btnReadThailand;
        private System.Windows.Forms.Button btnReadVietnam;
        private System.Windows.Forms.Button btnReadIndonesia;
        private System.Windows.Forms.Button btnReadMalaysia;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button showDialogButton;
    }
}

