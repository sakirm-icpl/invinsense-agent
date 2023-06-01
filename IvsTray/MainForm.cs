using Common;
using Common.NamedPipes;
using Common.Persistance;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
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

        private const int Margine = 10;

        private readonly IDictionary<string, RunningStatus> _toolRunningStatuses = new Dictionary<string, RunningStatus>();

        private readonly ClientPipe _clientPipe;

        public MainForm()
        {
            _logger.Information("Loading MainForm");

            InitializeComponent();

            SetWindowPosition();

            _clientPipe = new ClientPipe(".", Constants.IvsName, p => p.StartStringReaderAsync());

            _clientPipe.DataReceived += (sndr, args) => { UpdateStatus(args.String); };
        }

        private void OnDpiChanged(object sender, DpiChangedEventArgs e)
        {
            SetWindowPosition();
        }

        /// <summary>
        /// Setting location of window.
        /// </summary>
        private void SetWindowPosition()
        {
            Rectangle workingArea = Screen.GetWorkingArea(this);
            Location = new Point(workingArea.Right - Size.Width - Margine, workingArea.Bottom - Size.Height - Margine);
        }

        /// <summary>
        /// Starting configuration for the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainFormOnLoad(object sender, EventArgs e)
        {
            //By default form will be in visible by making opacity to 0.
            Opacity = 0;

            //Making form borderless and rounded in the corners.
            Region = Region.FromHrgn(MainFormHelpers.CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            MouseDownFilter mouseFilter = new MouseDownFilter(this);
            mouseFilter.FormClicked += FormClicked;
            Application.AddMessageFilter(mouseFilter);

            _logger.Debug("Connecting to server");
            _clientPipe.Connect();
        }

        private void UpdateStatus(string status)
        {
            // Updates a label on the form with the new status. 
            // Must be done on the UI thread if this method isn't called from the UI thread.
            if (InvokeRequired)
            {
                Invoke((Action<string>)UpdateStatus, status);
            }
            else
            {
                var toolStatuses = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ToolStatus>>(status);

                var requireToFillInitialValues = _toolRunningStatuses.Count == 0;

                foreach (var toolStatus in toolStatuses)
                {
                    if (requireToFillInitialValues)
                    {
                        _toolRunningStatuses.Add(toolStatus.Name, toolStatus.RunningStatus);
                    }

                    UpdateToolStatus(toolStatus);
                }

                //Resize window based on number of tools
                if (requireToFillInitialValues)
                {
                    var toolsCount = _toolRunningStatuses.Count;
                    _logger.Information($"Number of tools: {toolsCount}");
                }
            }
        }

        private void UpdateToolStatus(ToolStatus toolStatus, bool showNotification = false)
        {
            if (_toolRunningStatuses.ContainsKey(toolStatus.Name))
            {
                _toolRunningStatuses[toolStatus.Name] = toolStatus.RunningStatus;
            }

            var pb = pbInvinsense;

            switch (toolStatus.Name)
            {
                case ToolName.EndpointDecetionAndResponse:
                    pb = pbWazuh;
                    break;
                case ToolName.EndpointDeception:
                    pb = pbDbytes;
                    break;
                case ToolName.AdvanceTelemetry:
                    pb = pbSysmon;
                    break;
                case ToolName.EndpointProtection:
                    pb = pbDefender;
                    break;
                case ToolName.UserBehaviorAnalytics:
                    pb = pbOsquery;
                    break;
                case ToolName.LateralMovementProtection:
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

            if (_toolRunningStatuses.Any(x => x.Value != RunningStatus.Running))
            {
                notifyIcon.Icon = Properties.Resources.red_logo_22_22;
                notifyIcon.Text = $"{Constants.IvsDescription} - Not all services are healthy";
            }
            else
            {
                notifyIcon.Icon = Properties.Resources.green_logo_22_22;
                notifyIcon.Text = $"{Constants.IvsDescription} - Healthy";
            }

            if (showNotification)
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

        /// <summary>
        /// This method brings the form to top without stealing focus
        /// </summary>
        private void BringToTop()
        {
            //When we click on notification icon the form gets visible by making Opacity to true.
            Opacity = 1;

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
    }
}