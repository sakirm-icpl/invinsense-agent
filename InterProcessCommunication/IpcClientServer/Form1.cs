using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PipesClientTest.Client;
using PipesClientTest.Server;

namespace PipesClientTest
{
    public partial class Form1 : Form
    {
        private readonly List<ClientPipe> clientPipes;
        private readonly List<ServerPipe> serverPipes;
        private readonly List<TextBox> tbServerSenders;
        private readonly List<TextBox> tbServerReceivers;
        private readonly List<TextBox> tbClientSenders;
        private readonly List<TextBox> tbClientReceivers;
        private ServerPipe nextServer;

        public Form1()
        {
            InitializeComponent();
            serverPipes = new List<ServerPipe>();
            clientPipes = new List<ClientPipe>();
            tbServerSenders = new List<TextBox>() { tbServerSend };
            tbServerReceivers = new List<TextBox>() { tbServerReceived };
            tbClientSenders = new List<TextBox>() { tbClientSend };
            tbClientReceivers = new List<TextBox>() { tbClientReceived };
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            CreateServer();
            btnCreateClient.Enabled = true;
            btnStartServer.Enabled = false;
        }

        private ServerPipe CreateServer()
        {
            int serverIdx = serverPipes.Count;
			ServerPipe serverPipe = new ServerPipe("Test", p => p.StartStringReaderAsync());
            serverPipes.Add(serverPipe);

			serverPipe.DataReceived += (sndr, args) =>
				this.BeginInvoke(() =>
					tbServerReceivers[serverIdx].Text = args.String);

            serverPipe.Connected += (sndr, args) =>
                this.BeginInvoke(() =>
                    {
                        CreateServerUI();
                        nextServer = CreateServer();
                    });

            return serverPipe;
        }

        private void btnCreateClient_Click(object sender, EventArgs e)
        {
            btnSendToClient.Enabled = true;
            btnSendToServer.Enabled = true;
            int clientIdx = clientPipes.Count;
            ClientPipe clientPipe = new ClientPipe(".", "Test", p=>p.StartStringReaderAsync());
            clientPipes.Add(clientPipe);

            CreateClientUI();

			clientPipe.DataReceived += (sndr, args) =>
				this.BeginInvoke(() =>
					tbClientReceivers[clientIdx].Text = args.String);

            clientPipe.Connect();
        }

        private void btnSendToServer_Click(object sender, EventArgs e)
        {
            int clientIdx = Convert.ToInt32(((Control)sender).Tag);
            clientPipes[clientIdx].WriteString(tbClientSenders[clientIdx].Text);
        }

        private void btnSendToClient_Click(object sender, EventArgs e)
        {
            int serverIdx = Convert.ToInt32(((Control)sender).Tag);
            serverPipes[serverIdx].WriteString(tbServerSenders[serverIdx].Text);
        }

        protected void CreateServerUI()
        {
            if (serverPipes.Count > 1)
            {
                CreateServerControls(serverPipes.Count - 1);
            }
        }

        protected void CreateClientUI()
        {
            if (clientPipes.Count > 1)
            {
                CreateClientControls(clientPipes.Count - 1);
            }
        }

        protected void CreateServerControls(int n)
        {
            ServerConnection cc = new ServerConnection();
            Button btnSend = (Button)cc.Controls.Find("btnSendToClient", true)[0];
            btnSend.Click += btnSendToClient_Click;
            btnSend.Tag = n;
            tbServerSenders.Add((TextBox)cc.Controls.Find("tbServerSend", true)[0]);
            tbServerReceivers.Add((TextBox)cc.Controls.Find("tbServerReceived", true)[0]);
            cc.Location = new Point(gbServer.Location.X - 3, gbServer.Location.Y + (gbServer.Size.Height + 10) * n);
            Controls.Add(cc);
        }

        protected void CreateClientControls(int n)
        {
            ClientConnection cc = new ClientConnection();
            Button btnSend = (Button)cc.Controls.Find("btnSendToServer", true)[0];
            btnSend.Click += btnSendToServer_Click;
            btnSend.Tag = n;
            tbClientSenders.Add((TextBox)cc.Controls.Find("tbClientSend", true)[0]);
            tbClientReceivers.Add((TextBox)cc.Controls.Find("tbClientReceived", true)[0]);
            cc.Location = new Point(gbClient.Location.X - 3, gbClient.Location.Y + (gbClient.Size.Height + 10) * n);
            Controls.Add(cc);
        }
    }
}
