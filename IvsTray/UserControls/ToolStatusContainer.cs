using Common;
using Common.Events;
using Common.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IvsTray.UserControls
{
    public partial class ToolStatusContainer : UserControl
    {
        private readonly ILogger _logger = Log.ForContext<ToolStatusContainer>();

        private event EventHandler<EventArgs> ToolNumberModified;
        public event EventHandler<NotifyEventArgs> Notify;

        private readonly IDictionary<ToolGroup, ToolStatusBar> _toolRunningStatuses = new Dictionary<ToolGroup, ToolStatusBar>();

        public ToolStatusContainer()
        {
            InitializeComponent();
            ToolNumberModified += HandleToolNumberModified;
        }

        private void HandleToolNumberModified(object sender, EventArgs e)
        {
            if (Controls.Count == _toolRunningStatuses.Count)
            {
                return;
            }

            if (InvokeRequired)
            {
                Invoke(new Action<object, EventArgs>(HandleToolNumberModified), sender, e);
                return;
            }

            _logger.Verbose("Tool number changed. Count = {0}", _toolRunningStatuses.Count);

            Controls.Clear();

            var index = 0;
            foreach (var item in _toolRunningStatuses.Keys)
            {
                var ctrl = _toolRunningStatuses[item];
                ctrl.Location = new Point(0, 10 + index * 40);
                Controls.Add(ctrl);
                index++;
            }

            var newHeight = 40 * index + 20;
            Size = new Size(Size.Width, newHeight);
        }

        public void UpdateToolRunningStatus(List<ToolStatus> statuses)
        {
            var toolNumbersChanged = false;

            foreach (var item in statuses)
            {
                var group = ToolGroup.FromId<ToolGroup>(item.Group) as ToolGroup;

                if (_toolRunningStatuses.ContainsKey(group))
                {
                    _toolRunningStatuses[group].UpdateRunningStatus(item.RunningStatus);
                }
                else
                {
                    _logger.Verbose("Adding new control for {ToolName}", group.Name);

                    var control = new ToolStatusBar(group, item.RunningStatus)
                    {
                        Tag = group
                    };

                    control.Notify += (sender, args) => { Notify?.Invoke(sender, args); };

                    _toolRunningStatuses.Add(group, control);

                    toolNumbersChanged = true;
                }
            }

            if (toolNumbersChanged)
            {
                ToolNumberModified?.Invoke(this, new EventArgs());
            }

            if (_toolRunningStatuses.Values.Any(x => x.GetRunningStatus() != RunningStatus.Running))
            {
                Notify?.Invoke(this, new NotifyEventArgs(NotifyType.Error, Constants.IvsTrayName, "Not all services are healthy"));
            }
            else
            {
                Notify?.Invoke(this, new NotifyEventArgs(NotifyType.Info, Constants.IvsTrayName, "All services are healthy"));
            }
        }
    }
}
