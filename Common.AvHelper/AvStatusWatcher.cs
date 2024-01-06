using Common.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Management;

namespace Common.AvHelper
{
    public sealed class AvStatusWatcher
    {
        private readonly ILogger _logger = Log.ForContext<AvStatusWatcher>();

        public delegate void AvStatusChangedHandler(object sender, ToolStatus e);
        public event AvStatusChangedHandler AvStatusChanged;

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
                    AvStatusChanged?.Invoke(this, new ToolStatus(ToolGroup.EndpointProtection.Id, InstallStatus.Installed, RunningStatus.Stopped));
                }
                else if (arg.EventRecord.Id == 5000)
                {
                    AvStatusChanged?.Invoke(this, new ToolStatus(ToolGroup.EndpointProtection.Id, InstallStatus.Installed, RunningStatus.Running));
                }
            }
            else
            {
                _logger.Debug("Windows Defender Event reading error: {Message}", arg.EventException.Message);
            }
        }

        public ToolStatus GetStatus()
        {
            var installedAntiviruses = ListAvStatuses();
            ToolStatus avStatus;
            if (installedAntiviruses.Any(x => x.IsAvEnabled && x.AvName == "Windows Defender"))
            {
                var defenderStatus = installedAntiviruses.FirstOrDefault(x => x.IsAvEnabled && x.AvName == "Windows Defender");
                var runningStatus = defenderStatus.IsAvEnabled && defenderStatus.IsAvUptoDate ? RunningStatus.Running : RunningStatus.Warning;
                avStatus = new ToolStatus(ToolGroup.EndpointProtection.Id, InstallStatus.Installed, runningStatus);
            }
            else
            {
                avStatus = new ToolStatus(ToolGroup.EndpointProtection.Id, InstallStatus.Installed, RunningStatus.Unknown);
            }

            return avStatus;
        }

        public static IEnumerable<AvStatus> ListAvStatuses()
        {
            //Windows Defender
            //393472 (060100) = disabled and up to date
            //397584 (061110) = enabled and out of date
            //397568 (061100) = enabled and up to date

            ManagementObjectSearcher wmiData = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntiVirusProduct");
            ManagementObjectCollection data = wmiData.Get();

            List<AvStatus> avStatuses = new List<AvStatus>();

            foreach (ManagementObject mo in data.OfType<ManagementObject>())
            {
                avStatuses.Add(new AvStatus(mo));
            }

            return avStatuses;
        }
    }
}
