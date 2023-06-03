namespace IvsTray
{
    partial class ToolStatusBar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblToolName = new System.Windows.Forms.Label();
            this.pbStatus = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // lblToolName
            // 
            this.lblToolName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblToolName.AutoSize = true;
            this.lblToolName.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblToolName.Location = new System.Drawing.Point(8, 11);
            this.lblToolName.Name = "lblToolName";
            this.lblToolName.Size = new System.Drawing.Size(78, 19);
            this.lblToolName.TabIndex = 9;
            this.lblToolName.Text = "Tool Name";
            // 
            // pbStatus
            // 
            this.pbStatus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pbStatus.Image = global::IvsTray.Properties.Resources.gray;
            this.pbStatus.InitialImage = null;
            this.pbStatus.Location = new System.Drawing.Point(322, 14);
            this.pbStatus.Name = "pbStatus";
            this.pbStatus.Size = new System.Drawing.Size(12, 12);
            this.pbStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbStatus.TabIndex = 8;
            this.pbStatus.TabStop = false;
            // 
            // ToolStatusBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblToolName);
            this.Controls.Add(this.pbStatus);
            this.Name = "ToolStatusBar";
            this.Size = new System.Drawing.Size(350, 40);
            ((System.ComponentModel.ISupportInitialize)(this.pbStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblToolName;
        private System.Windows.Forms.PictureBox pbStatus;
    }
}
