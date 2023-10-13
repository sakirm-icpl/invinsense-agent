using Common.Persistence;
using IvsTray.Notifier;
using Serilog;
using System;
using System.Windows.Forms;

namespace IvsTray
{
    public partial class ToolStatusBar : UserControl
    {
        private readonly ILogger _logger = Log.ForContext<ToolStatusBar>();

        public event EventHandler<NotifyEventArgs> Notify;

        public ToolStatusBar(string toolCategory, RunningStatus runningStatus)
        {
            InitializeComponent();

            CategoryName = toolCategory;

            switch (toolCategory)
            {
                case ToolName.EndpointDetectionAndResponse:
                    lblToolName.Text = "Endpoint Detection and Response";
                    break;
                case ToolName.EndpointDeception:
                    lblToolName.Text = "Endpoint Deception";
                    break;
                case ToolName.AdvanceTelemetry:
                    lblToolName.Text = "Advance Telemetry";
                    break;
                case ToolName.EndpointProtection:
                    lblToolName.Text = "Endpoint Protection";
                    break;
                case ToolName.UserBehaviorAnalytics:
                    lblToolName.Text = "User Behavior Analytics";
                    break;
                case ToolName.LateralMovementProtection:
                    lblToolName.Text = "Lateral Movement Protection";
                    break;
            }

            _runningStatus = runningStatus; 
            pbStatus.Image = StatusIconExtensions.Convert(runningStatus);
        }

        public string CategoryName { get; }

        private RunningStatus _runningStatus;

        public void UpdateRunningStatus(RunningStatus runningStatus)
        {
            if(_runningStatus == runningStatus)
            {
                return;
            }

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => UpdateRunningStatus(runningStatus)));
                return;
            }

            _logger.Verbose("Updating running status for {ToolCategory} to {RunningStatus}", CategoryName, runningStatus);

            _runningStatus = runningStatus;
            pbStatus.Image = StatusIconExtensions.Convert(runningStatus);

            Notify?.Invoke(this, new NotifyEventArgs(NotifyEventArgsExtensions.ConvertStatus(runningStatus), lblToolName.Text, NotifyEventArgsExtensions.BuildMessage("Service", _runningStatus)));
        }

        public RunningStatus GetRunningStatus()
        {
            return _runningStatus;
        }
    }
}
