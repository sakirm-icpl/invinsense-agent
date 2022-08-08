using Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Windows.Forms;

namespace SingleAgent
{
    public partial class MainForm : Form
    {
        private readonly Image NotFoundImage = Properties.Resources.gray;
        private readonly Image ErrorImage = Properties.Resources.red;
        private readonly Image WarnImage = Properties.Resources.yellow;
        private readonly Image OkImage = Properties.Resources.green;

        private readonly Dictionary<string, bool> _healthStatus = new Dictionary<string, bool>
        {
            {"Deceptive Bytes", true },
            {"Windows Defender", true },
            {"Wazuh", true },
            {"Microsoft Sysmon", true },
            {"Lateral Movement Protection", true },
            {"Invinsense3.0",true }
        };

        private readonly ILogger _logger = Log.ForContext<MainForm>();

        public MainForm()
        {
            _logger.Information("Loading MainForm");

            InitializeComponent();

            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            MouseDownFilter mouseFilter = new MouseDownFilter(this);
            mouseFilter.FormClicked += FormClicked;
            Application.AddMessageFilter(mouseFilter);
        }

        public void UpdateStatus(string name, ServiceControllerStatus? status)
        {
            _logger.Information($"Updating service status: {name} : {status}");
        }

        private void UpdateStatus(EventId evenId)
        {
            var message = $"Status Changed:";
            var icon = ToolTipIcon.Warning;

            notifyIcon.ShowBalloonTip(5000, "", message, icon);
            UpdateNotificationIcon("", icon == ToolTipIcon.Info);
            notifyIcon.ShowBalloonTip(5000, "Windows Defender", message, icon);
        }

        private void UpdateNotificationIcon(string name, bool status)
        {
            _healthStatus[name] = status;

            if(_healthStatus.Any(x=> !x.Value))
            {
                notifyIcon.Icon = Properties.Resources.red_logo_22_22;
                notifyIcon.Text = "Invinsense 3.0 - Not all services are healthy";
            }
            else
            {
                notifyIcon.Icon = Properties.Resources.green_logo_22_22;
                notifyIcon.Text = "Invinsense 3.0 - Healthy"; 
            }
        }        

        private void MainFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void FormClicked(object sender, EventArgs e)
        {
            Hide();
        }

        private void ShowClick(object sender, EventArgs e)
        {
            BringToTop();
        }

        private void BringToTop()
        {
            //Checks if the method is called from UI thread or not
            if (InvokeRequired)
            {
                Invoke(new Action(BringToTop));
            }
            else
            {
                Visible = true;

                WindowState = FormWindowState.Normal;

                //Keeps the current topmost status of form
                bool top = TopMost;

                //Brings the form to top
                TopMost = true;

                //Set form's topmost status back to whatever it was
                TopMost = top;
            }
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
           int nLeftRect,     // x-coordinate of upper-left corner
           int nTopRect,      // y-coordinate of upper-left corner
           int nRightRect,    // x-coordinate of lower-right corner
           int nBottomRect,   // y-coordinate of lower-right corner
           int nWidthEllipse, // width of ellipse
           int nHeightEllipse // height of ellipse
        );

     }
}