using Common.Events;
using Common.Models;
using IvsTray.Extensions;
using Serilog;
using System;
using System.Windows.Forms;

namespace IvsTray
{
    public partial class ToolStatusBar : UserControl
    {
        private readonly ILogger _logger = Log.ForContext<ToolStatusBar>();

        public event EventHandler<NotifyEventArgs> Notify;

        public ToolStatusBar(ToolGroup toolGroup, RunningStatus runningStatus)
        {
            InitializeComponent();

            lblToolName.Text = toolGroup.ToString();

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
