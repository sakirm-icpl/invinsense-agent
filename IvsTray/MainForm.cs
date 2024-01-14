using Common;
using Common.Events;
using Common.Models;
using Common.NamedPipes;
using Common.RegistryHelpers;
using IvsTray.Extensions;
using IvsTray.Properties;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IvsTray
{
    public partial class MainForm : Form
    {
        private readonly ILogger _logger = Log.ForContext<MainForm>();

        private readonly ClientPipe _clientPipe;

        public MainForm()
        {
            _logger.Information("Loading MainForm");
            InitializeComponent();

            Icon = Resources.green_logo_22_22;
            notifyIcon.Icon = Resources.green_logo_22_22;

            _clientPipe = new ClientPipe(".", Constants.BrandName, p => p.StartStringReaderAsync());
            tsc.Notify += NotifyTray;
        }

        /// <summary>
        /// Starting configuration for the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainFormOnLoad(object sender, EventArgs e)
        {
            _logger.Verbose("Form Loaded");

            //By default form will be in visible by making opacity to 0.
            Opacity = 0;

            //Adding mouse filter to the form.
            MouseDownFilter mouseFilter = new MouseDownFilter(this);
            mouseFilter.FormClicked += FormClicked;
            Application.AddMessageFilter(mouseFilter);

            _clientPipe.DataReceived += (sander, args) =>
            {
                _logger.Verbose("Data Received: {0}", args.String);
                var trayStatus = JsonConvert.DeserializeObject<TrayStatus>(args.String);
                if (trayStatus.ErrorCode != 0)
                {
                    MessageBox.Show(trayStatus.ErrorMessage);
                }

                tsc.UpdateToolRunningStatus(trayStatus.ToolStatuses);
            };


            Task.Run(() =>
            {
                _logger.Verbose("Connecting to Agent Service...");
                _clientPipe.Connect();
                _logger.Debug("Agent Service connected...");
            });

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                    var isolationState = Environment.GetEnvironmentVariable("isolation_state", EnvironmentVariableTarget.Machine);
                    if (isolationState != "true")
                    {
                        continue;
                    }

                    var i18Path = $"{Constants.CompanyName}\\i18n";
                    var title = WinRegistryHelper.GetPropertyByName(i18Path, "IsolationTitle");
                    var message = WinRegistryHelper.GetPropertyByName(i18Path, "IsolationMessage");

                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            });
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

        private void ToolContainerSizeChanged(object sender, EventArgs e)
        {
            _logger.Verbose("ToolContainerSizeChanged");
            ResizeWindowAndLocate();
        }

        private void OnDpiChanged(object sender, DpiChangedEventArgs e)
        {
            _logger.Verbose("OnDpiChanged");
            ResizeWindowAndLocate();
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            _logger.Verbose("OnFormClosed");
            _clientPipe?.Close();
        }

        /// <summary>
        /// Setting location of window.
        /// </summary>
        private void ResizeWindowAndLocate()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => { ResizeWindowAndLocate(); }));
                return;
            }

            const int margin = 10;

            var newHeight = tsc.Height + fuc.Height;

            Rectangle workingArea = Screen.GetWorkingArea(this);
            Location = new Point(workingArea.Right - Size.Width - margin, workingArea.Bottom - newHeight - margin);
            Size = new Size(Size.Width, newHeight);

            //Making form borderless and rounded in the corners.
            Region = Region.FromHrgn(MainFormHelpers.CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        /// <summary>
        /// This method brings the form to top without stealing focus
        /// </summary>
        private void BringToTop()
        {
            //When we click on notification icon, the form gets visible by making Opacity to true.
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

        private void NotifyTray(object sender, NotifyEventArgs e)
        {
            _logger.Verbose("NotifyTray: {Message}", e.Message);
            UpdateIcon(e.Title, e.Message, e.NotifyType);
        }

        private void UpdateIcon(string title, string message, NotifyType notifyType)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateIcon(title, message, notifyType)));
                return;
            }

            if (notifyType == NotifyType.Info)
            {
                Icon = Resources.green_logo_22_22;
                notifyIcon.Icon = Resources.green_logo_22_22;
            }
            else
            {
                Icon = Resources.red_logo_22_22;
                notifyIcon.Icon = Resources.red_logo_22_22;
            }

            notifyIcon.ShowBalloonTip(5000, title, message, NotifyEventArgsExtensions.ConvertIcon(notifyType));
        }
    }
}