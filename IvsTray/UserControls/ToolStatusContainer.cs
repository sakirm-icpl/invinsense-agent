using Common.Persistance;
using IvsTray.Notifier;
using Serilog;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IvsTray.UserControls
{
    public partial class ToolStatusContainer : UserControl
    {
        private readonly ILogger _logger = Log.ForContext<ToolStatusContainer>();

        private event EventHandler<EventArgs> ToolNumberModified;
        public event EventHandler<NotifyEventArgs> Notify;

        private readonly IDictionary<string, ToolStatusBar> _toolRunningStatuses = new Dictionary<string, ToolStatusBar>();

        public ToolStatusContainer()
        {
            InitializeComponent();
            ToolNumberModified += HandleToolNumberModified;
        }

        private void HandleToolNumberModified(object sender, EventArgs e)
        {
            Controls.Clear();

            var index = 0;
            foreach (var item in _toolRunningStatuses.Keys)
            {
                index++;
                var ctrl = _toolRunningStatuses[item];
                ctrl.Location = new System.Drawing.Point(0, 10 + index * ctrl.Height);
                Controls.Add(ctrl);
            }

            Height = 40 * index + 20;
        }

        public void UpdateToolRunningStatus(List<ToolStatus> statuses)
        {
            var toolNumbersChanged = false;

            foreach (var item in statuses)
            {
                if (_toolRunningStatuses.ContainsKey(item.Name))
                {
                    _toolRunningStatuses[item.Name].UpdateRunningStatus(item.RunningStatus);
                }
                else
                {
                    //Add new tool status bar
                    var control = new ToolStatusBar(item.Name, item.RunningStatus)
                    {
                        Tag = item.Name
                    };

                    control.Notify += (sndr, args) => { Notify?.Invoke(sndr, args); };

                    toolNumbersChanged = true;
                }
            }

            if(toolNumbersChanged)
            {
                _logger.Verbose("Tool number changed");
                ToolNumberModified?.Invoke(this, new EventArgs());
            }
        }
    }
}
