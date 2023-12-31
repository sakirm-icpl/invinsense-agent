namespace PipesClientTest
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
            this.btnSendToClient = new System.Windows.Forms.Button();
            this.tbServerSend = new System.Windows.Forms.TextBox();
            this.tbServerReceived = new System.Windows.Forms.TextBox();
            this.gbServer = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.gbClient = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbClientSend = new System.Windows.Forms.TextBox();
            this.tbClientReceived = new System.Windows.Forms.TextBox();
            this.btnSendToServer = new System.Windows.Forms.Button();
            this.btnCreateClient = new System.Windows.Forms.Button();
            this.gbServer.SuspendLayout();
            this.gbClient.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSendToClient
            // 
            this.btnSendToClient.Enabled = false;
            this.btnSendToClient.Location = new System.Drawing.Point(239, 19);
            this.btnSendToClient.Margin = new System.Windows.Forms.Padding(2);
            this.btnSendToClient.Name = "btnSendToClient";
            this.btnSendToClient.Size = new System.Drawing.Size(77, 25);
            this.btnSendToClient.TabIndex = 0;
            this.btnSendToClient.Tag = "0";
            this.btnSendToClient.Text = "Send";
            this.btnSendToClient.UseVisualStyleBackColor = true;
            this.btnSendToClient.Click += new System.EventHandler(this.btnSendToClient_Click);
            // 
            // tbServerSend
            // 
            this.tbServerSend.Location = new System.Drawing.Point(15, 22);
            this.tbServerSend.Margin = new System.Windows.Forms.Padding(2);
            this.tbServerSend.Name = "tbServerSend";
            this.tbServerSend.Size = new System.Drawing.Size(220, 20);
            this.tbServerSend.TabIndex = 1;
            this.tbServerSend.Text = "Test Message";
            // 
            // tbServerReceived
            // 
            this.tbServerReceived.Location = new System.Drawing.Point(15, 68);
            this.tbServerReceived.Name = "tbServerReceived";
            this.tbServerReceived.ReadOnly = true;
            this.tbServerReceived.Size = new System.Drawing.Size(301, 20);
            this.tbServerReceived.TabIndex = 2;
            // 
            // gbServer
            // 
            this.gbServer.Controls.Add(this.label1);
            this.gbServer.Controls.Add(this.tbServerSend);
            this.gbServer.Controls.Add(this.tbServerReceived);
            this.gbServer.Controls.Add(this.btnSendToClient);
            this.gbServer.Location = new System.Drawing.Point(12, 44);
            this.gbServer.Name = "gbServer";
            this.gbServer.Size = new System.Drawing.Size(329, 97);
            this.gbServer.TabIndex = 3;
            this.gbServer.TabStop = false;
            this.gbServer.Text = "Server";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Received:";
            // 
            // btnStartServer
            // 
            this.btnStartServer.Location = new System.Drawing.Point(12, 11);
            this.btnStartServer.Margin = new System.Windows.Forms.Padding(2);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(100, 25);
            this.btnStartServer.TabIndex = 4;
            this.btnStartServer.Text = "Start";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // gbClient
            // 
            this.gbClient.Controls.Add(this.label2);
            this.gbClient.Controls.Add(this.tbClientSend);
            this.gbClient.Controls.Add(this.tbClientReceived);
            this.gbClient.Controls.Add(this.btnSendToServer);
            this.gbClient.Location = new System.Drawing.Point(347, 45);
            this.gbClient.Name = "gbClient";
            this.gbClient.Size = new System.Drawing.Size(329, 96);
            this.gbClient.TabIndex = 5;
            this.gbClient.TabStop = false;
            this.gbClient.Text = "Client";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Received:";
            // 
            // tbClientSend
            // 
            this.tbClientSend.Location = new System.Drawing.Point(13, 21);
            this.tbClientSend.Margin = new System.Windows.Forms.Padding(2);
            this.tbClientSend.Name = "tbClientSend";
            this.tbClientSend.Size = new System.Drawing.Size(220, 20);
            this.tbClientSend.TabIndex = 1;
            this.tbClientSend.Text = "Test Message";
            // 
            // tbClientReceived
            // 
            this.tbClientReceived.Location = new System.Drawing.Point(13, 67);
            this.tbClientReceived.Name = "tbClientReceived";
            this.tbClientReceived.ReadOnly = true;
            this.tbClientReceived.Size = new System.Drawing.Size(301, 20);
            this.tbClientReceived.TabIndex = 2;
            // 
            // btnSendToServer
            // 
            this.btnSendToServer.Enabled = false;
            this.btnSendToServer.Location = new System.Drawing.Point(237, 18);
            this.btnSendToServer.Margin = new System.Windows.Forms.Padding(2);
            this.btnSendToServer.Name = "btnSendToServer";
            this.btnSendToServer.Size = new System.Drawing.Size(77, 25);
            this.btnSendToServer.TabIndex = 0;
            this.btnSendToServer.Tag = "0";
            this.btnSendToServer.Text = "Send";
            this.btnSendToServer.UseVisualStyleBackColor = true;
            this.btnSendToServer.Click += new System.EventHandler(this.btnSendToServer_Click);
            // 
            // btnCreateClient
            // 
            this.btnCreateClient.Enabled = false;
            this.btnCreateClient.Location = new System.Drawing.Point(347, 11);
            this.btnCreateClient.Margin = new System.Windows.Forms.Padding(2);
            this.btnCreateClient.Name = "btnCreateClient";
            this.btnCreateClient.Size = new System.Drawing.Size(100, 25);
            this.btnCreateClient.TabIndex = 4;
            this.btnCreateClient.Text = "Create Client";
            this.btnCreateClient.UseVisualStyleBackColor = true;
            this.btnCreateClient.Click += new System.EventHandler(this.btnCreateClient_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 596);
            this.Controls.Add(this.btnCreateClient);
            this.Controls.Add(this.btnStartServer);
            this.Controls.Add(this.gbClient);
            this.Controls.Add(this.gbServer);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Pipe Client/Server Test";
            this.gbServer.ResumeLayout(false);
            this.gbServer.PerformLayout();
            this.gbClient.ResumeLayout(false);
            this.gbClient.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSendToClient;
        private System.Windows.Forms.TextBox tbServerSend;
        private System.Windows.Forms.TextBox tbServerReceived;
        private System.Windows.Forms.GroupBox gbServer;
        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbClient;
        private System.Windows.Forms.Button btnCreateClient;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbClientSend;
        private System.Windows.Forms.TextBox tbClientReceived;
        private System.Windows.Forms.Button btnSendToServer;
    }
}

