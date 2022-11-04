using Common;
using Common.Persistance;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IvsTray
{
    public partial class MainForm : Form
    {
        private readonly Image NotFoundImage = Properties.Resources.gray;
        private readonly Image ErrorImage = Properties.Resources.red;
        private readonly Image WarnImage = Properties.Resources.yellow;
        private readonly Image OkImage = Properties.Resources.green;

        private readonly ILogger _logger = Log.ForContext<MainForm>();

        private readonly ToolRepository toolRepository;

        private readonly IDictionary<string, RunningStatus> toolStatuses = new Dictionary<string, RunningStatus>();

        public MainForm()
        {
            _logger.Information("Loading MainForm");

            InitializeComponent();

            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            MouseDownFilter mouseFilter = new MouseDownFilter(this);
            mouseFilter.FormClicked += FormClicked;
            Application.AddMessageFilter(mouseFilter);

            toolRepository = new ToolRepository();

            var log = new EventLog(Constants.LogGroupName)
            {
                EnableRaisingEvents = true
            };

            log.EntryWritten += Log_EntryWritten;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Log_EntryWritten(object sender, EntryWrittenEventArgs e)
        {
            var toolStatus = new ToolStatus(e.Entry.InstanceId);
            UpdateToolStatus(toolStatus, true);            
        }

        private void MainFormOnLoad(object sender, EventArgs e)
        {
            _logger.Information("Loading all tools from db");
            var allTools = toolRepository.GetTools();
            _logger.Information($"Tools loaded: {allTools.Count()}");

            toolStatuses.Clear();
            foreach (var toolDetail in allTools)
            {
                toolStatuses.Add(toolDetail.Name, toolDetail.RunningStatus);
                UpdateToolStatus(new ToolStatus(toolDetail.Name, toolDetail.InstallStatus, toolDetail.RunningStatus));
            }
        }

        private void UpdateToolStatus(ToolStatus toolStatus, bool showNotification = false)
        {
            if(toolStatuses.ContainsKey(toolStatus.Name))
            {
                toolStatuses[toolStatus.Name] = toolStatus.RunningStatus;
            }

            var pb = pbInvinsense;

            switch (toolStatus.Name)
            {
                case ToolName.Wazuuh:
                    pb = pbWazuh;
                    break;
                case ToolName.Dbytes:
                    pb = pbDbytes;
                    break;
                case ToolName.Sysmon:
                    pb = pbSysmon;
                    break;
                case ToolName.Av:
                    pb = pbDefender;
                    break;
                case ToolName.OsQuery:
                case ToolName.Lmp:
                    pb = pbLmp;
                    break;
            }

            ToolTipIcon icon;
            if (toolStatus.RunningStatus == RunningStatus.NotFound)
            {
                icon = ToolTipIcon.Error;
                pb.Image = NotFoundImage;
                _logger.Fatal($"{toolStatus.Name} not found");
            }
            else if (toolStatus.RunningStatus == RunningStatus.Running)
            {
                icon = ToolTipIcon.Info;
                pb.Image = OkImage;
                _logger.Fatal($"{toolStatus.Name} running");
            }
            else if (toolStatus.RunningStatus == RunningStatus.Warning)
            {
                icon = ToolTipIcon.Warning;
                pb.Image = WarnImage;
                _logger.Fatal($"{toolStatus.Name} warning state");
            }
            else
            {
                icon = ToolTipIcon.Error;
                pb.Image = ErrorImage;
                _logger.Fatal($"{toolStatus.Name} error state");
            }

            if (toolStatuses.Any(x=>x.Value != RunningStatus.Running))
            {
                notifyIcon.Icon = Properties.Resources.red_logo_22_22;
                notifyIcon.Text = "Invinsense 3.0 - Not all services are healthy";
            }
            else
            {
                notifyIcon.Icon = Properties.Resources.green_logo_22_22;
                notifyIcon.Text = "Invinsense 3.0 - Healthy";
            }

            if(showNotification)
            {
                notifyIcon.ShowBalloonTip(5000, toolStatus.Name, $"Status: {toolStatus.RunningStatus}", icon);
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