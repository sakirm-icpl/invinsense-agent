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

        private bool _isRunning = false;

        public SingleAgentService()
        {
            InitializeComponent();

            AutoLog = true;
            CanShutdown = true;

            wazuh = new ExtendedServiceController("WazuhSvc");
            
            Dbytes = new ExtendedServiceController("DBytesService");
            
            Sysmon = new ExtendedServiceController("Sysmon64");
            
            Dejavu = new ExtendedServiceController("Spooler");


            wazuh.StatusChanged += (object sender, ServiceStatusEventArgs e) => WazuhUpdateStatus(e.Status);
            Dbytes.StatusChanged += (object sender, ServiceStatusEventArgs e) => DbytesUpdateStatus(e.Status);
            Sysmon.StatusChanged += (object sender, ServiceStatusEventArgs e) => SysmonUpdateStatus(e.Status);
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
            _isRunning = true;

            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds  
            timer.Enabled = true;

            UpdateStatus(EventId.IvsRunning);

            WazuhUpdateStatus(wazuh.Status);
            DbytesUpdateStatus(Dbytes.Status);
            SysmonUpdateStatus(Sysmon.Status);
            LmpStatusUpdate(Dejavu.Status);
        }

        protected override void OnStop()
        {
            UpdateStatus(EventId.IvsStopped);

            _isRunning = false;
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            _logger.Information("System is shutting down");
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
            if (!_isRunning) return;

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
