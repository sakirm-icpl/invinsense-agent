using Common;
using Common.NamedPipes;
using Common.Persistance;
using IvsTray.Extensions;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
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

            _clientPipe = new ClientPipe(".", Constants.IvsName, p => p.StartStringReaderAsync());
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

            //Making form borderless and rounded in the corners.
            Region = Region.FromHrgn(MainFormHelpers.CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            MouseDownFilter mouseFilter = new MouseDownFilter(this);
            mouseFilter.FormClicked += FormClicked;
            Application.AddMessageFilter(mouseFilter);

            _clientPipe.DataReceived += (sndr, args) =>
            {
                _logger.Verbose("Data Received: {0}", args.String);
                var toolStatuses = JsonConvert.DeserializeObject<List<ToolStatus>>(args.String);
                tsc.UpdateToolRunningStatus(toolStatuses);
            };

            Task.Run(() =>
            {
                _logger.Verbose("Connecting to Agent Service...");
                _clientPipe.Connect();
                _logger.Debug("Agent Service connected...");
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

            const int margine = 10;

            Height = tsc.Height + brandingPanel.Height;
            Rectangle workingArea = Screen.GetWorkingArea(this);
            Location = new Point(workingArea.Right - Size.Width - margine, workingArea.Bottom - Size.Height - margine);
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

        private void NotifyTray(object sender, Notifier.NotifyEventArgs e)
        {
            notifyIcon.ShowBalloonTip(5000, e.Title, e.Message, NotifyEventArgsExtensions.ConvertIcon(e.NotifyType));
        }
    }
}