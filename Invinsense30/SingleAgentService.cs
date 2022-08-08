using Common;
using Invinsense30.Monitor;
using Serilog;
using SingleAgent.Monitor;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;

namespace Invinsense30
{
    public partial class SingleAgentService : ServiceBase
    {
        private readonly Timer timer = new Timer();

        private readonly ExtendedServiceController Dbytes;
        private readonly ExtendedServiceController wazuh;
        private readonly ExtendedServiceController Sysmon;
        private readonly ExtendedServiceController Dejavu;

        private readonly ILogger _logger = Log.ForContext<SingleAgentService>();

        public SingleAgentService()
        {
            InitializeComponent();

            wazuh = new ExtendedServiceController("WazuhSvc");
            wazuh.StatusChanged += (object sender, ServiceStatusEventArgs e) => WazuhUpdateStatus(e.Status);
            WazuhUpdateStatus(wazuh.Status);

            Dbytes = new ExtendedServiceController("DBytesService");
            Dbytes.StatusChanged += (object sender, ServiceStatusEventArgs e) => DbytesUpdateStatus(e.Status);
            DbytesUpdateStatus(Dbytes.Status);

            Sysmon = new ExtendedServiceController("Sysmon64");
            Sysmon.StatusChanged += (object sender, ServiceStatusEventArgs e) => SysmonUpdateStatus(e.Status);

            Dejavu = new ExtendedServiceController("Spooler");
            Dejavu.StatusChanged += (object sender, ServiceStatusEventArgs e) => LmpStatusUpdate(e.Status);
        }

        private void WazuhUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                UpdateStatus(EventId.WazuhNotFound);
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.Running:
                    UpdateStatus(EventId.WazuhRunning);
                    return;
                case ServiceControllerStatus.Stopped:
                    UpdateStatus(EventId.WazuhStopped);
                    return;
                default:
                    UpdateStatus(EventId.WazuhWarning);
                    return;
            }
        }

        private void DbytesUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                UpdateStatus(EventId.DbytesNotFound);
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.Running:
                    UpdateStatus(EventId.DbytesRunning);
                    return;
                case ServiceControllerStatus.Stopped:
                    UpdateStatus(EventId.DbytesStopped);
                    return;
                default:
                    UpdateStatus(EventId.DbytesWarning);
                    return;
            }
        }

        private void SysmonUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                UpdateStatus(EventId.SysmonNotFound);
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.Running:
                    UpdateStatus(EventId.SysmonRunning);
                    return;
                case ServiceControllerStatus.Stopped:
                    UpdateStatus(EventId.SysmonStopped);
                    return;
                default:
                    UpdateStatus(EventId.SysmonWarning);
                    return;
            }
        }

        private void LmpStatusUpdate(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                UpdateStatus(EventId.LmpNotFound);
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.Running:
                    UpdateStatus(EventId.LmpRunning);
                    return;
                case ServiceControllerStatus.Stopped:
                    UpdateStatus(EventId.LmpStopped);
                    return;
                default:
                    UpdateStatus(EventId.LmpWarning);
                    return;
            }
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds  
            timer.Enabled = true;

            UpdateStatus(EventId.IvsRunning);
        }

        protected override void OnStop()
        {
            UpdateStatus(EventId.IvsStopped);
        }

        private EventId avLastStatus = EventId.None;
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            Log.Information("Checking windows defender service");

            var status = ServiceHelper.AVStatus("Windows Defender");

            if (avLastStatus == status)
            {
                return;
            }

            avLastStatus = status;

            UpdateStatus(status);
        }

        private void UpdateStatus(EventId eventId)
        {
            var eventDetail = TrackingEventProvider.Instance.GetEventDetail(eventId);
            EventLogWriter.Log(eventDetail.EventType, "SingleAgent", eventId);
            
            if(eventDetail.EventType == EventLogEntryType.SuccessAudit || eventDetail.EventType == EventLogEntryType.Information)
            {
                _logger.Information(eventDetail.Message);
            }
            else if (eventDetail.EventType == EventLogEntryType.Warning)
            {
                _logger.Warning(eventDetail.Message);
            }
            else
            {
                _logger.Error(eventDetail.Message);
            }
        }
    }

}
