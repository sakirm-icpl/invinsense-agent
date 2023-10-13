using Common.Persistence;
using IvsAgent.AvHelper;
using Serilog;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace IvsAgent.Monitor
{
    public sealed class AvStatusWatcher
    {
        private readonly ILogger _logger = Log.ForContext<IvsService>();

        public delegate void AvStatusChanedHnadler(object sender, ToolStatus e);
        public event AvStatusChanedHnadler AvStatusChaned;

        private readonly EventLogWatcher avWatcher;

        private static AvStatusWatcher instance = null;

        private static readonly object padlock = new object();


        AvStatusWatcher()
        {
            //Adding AV Watcher. Can be moved to separate component
            avWatcher = new EventLogWatcher("Microsoft-Windows-Windows Defender/Operational");
            avWatcher.EventRecordWritten += new EventHandler<EventRecordWrittenEventArgs>(DefenderEventWritten);
        }

        public static AvStatusWatcher Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new AvStatusWatcher();
                    }
                    return instance;
                }
            }
        }

        public void StartMonitoring()
        {
            avWatcher.Enabled = true;
        }

        public void StopMonitoring()
        {
            avWatcher.Enabled = false;
        }

        /// <summary>
        /// This method is called when there is any change in windows defender
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="arg"></param>
        public void DefenderEventWritten(object obj, EventRecordWrittenEventArgs arg)
        {
            if (arg.EventRecord != null)
            {
                _logger.Debug("Defender EventId: {Id}, Publisher: {ProviderName}", arg.EventRecord.Id, arg.EventRecord.ProviderName);

                if (arg.EventRecord.Id == 5001)
                {
                    AvStatusChaned?.Invoke(this, new ToolStatus(ToolName.EndpointProtection, InstallStatus.Installed, RunningStatus.Stopped));
                }
                else if (arg.EventRecord.Id == 5000)
                {
                    AvStatusChaned?.Invoke(this, new ToolStatus(ToolName.EndpointProtection, InstallStatus.Installed, RunningStatus.Running));
                }
            }
            else
            {
                _logger.Debug("Windows Defender Event reading error: {Message}", arg.EventException.Message);
            }
        }

        public ToolStatus GetStatus()
        {
            var installedAntiviruses = AvMonitor.ListAvStatuses();
            ToolStatus avStatus;
            if (installedAntiviruses.Any(x => x.IsAvEnabled && x.AvName == "Windows Defender"))
            {
                var defenderStatus = installedAntiviruses.FirstOrDefault(x => x.IsAvEnabled && x.AvName == "Windows Defender");
                var runningStatus = (defenderStatus.IsAvEnabled && defenderStatus.IsAvUptoDate) ? RunningStatus.Running : RunningStatus.Warning;
                avStatus = new ToolStatus(ToolName.EndpointProtection, InstallStatus.Installed, runningStatus);
            }
            else
            {
                avStatus = new ToolStatus(ToolName.EndpointProtection, InstallStatus.Installed, RunningStatus.NotFound);
            }

            return avStatus;
        }
    }
}
