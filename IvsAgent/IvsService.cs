using Common;
using IvsAgent.Monitor;
using Serilog;
using IvsTray.Monitor;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using System.IO;
using System.Reflection;
using Common.Extensions;
using IvsAgent.AgentWrappers;

namespace IvsAgent
{
    public partial class IvsService : ServiceBase
    {
        private readonly Timer timer = new Timer();

        private readonly ExtendedServiceController Dbytes;
        private readonly ExtendedServiceController wazuh;
        private readonly ExtendedServiceController Sysmon;
        private readonly ExtendedServiceController LmpService;

        private readonly ILogger _logger = Log.ForContext<IvsService>();

        private bool _isRunning = false;

        public IvsService()
        {
            InitializeComponent();

            AutoLog = false;
            CanShutdown = true;

            CanPauseAndContinue = true;
            CanHandleSessionChangeEvent = true;


            wazuh = new ExtendedServiceController("WazuhSvc");
            wazuh.StatusChanged += (object sender, ServiceStatusEventArgs e) => WazuhUpdateStatus(e.Status);

            Dbytes = new ExtendedServiceController("DBytesService");
            Dbytes.StatusChanged += (object sender, ServiceStatusEventArgs e) => DbytesUpdateStatus(e.Status);

            Sysmon = new ExtendedServiceController("Sysmon64");
            Sysmon.StatusChanged += (object sender, ServiceStatusEventArgs e) => SysmonUpdateStatus(e.Status);

            LmpService = new ExtendedServiceController("osqueryd");
            LmpService.StatusChanged += (object sender, ServiceStatusEventArgs e) => LmpStatusUpdate(e.Status);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            EventLog.WriteEntry($"IvsService.OnSessionChange {DateTime.Now.ToLongTimeString()} - Session change notice received: {changeDescription.Reason}  Session ID: {changeDescription.SessionId}");

            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogon:
                    EventLog.WriteEntry("IvsService.OnSessionChange: Logon");
                    break;

                case SessionChangeReason.SessionLogoff:
                    EventLog.WriteEntry("IvsService.OnSessionChange Logoff");
                    break;
            }
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
            if (_isRunning)
            {
                _logger.Information("Invinsense service is already running.");
                return;
            }

            _isRunning = true;

            _logger.Information("Starting Invinsense service...");

            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 15000; //number in milisecinds  
            timer.Enabled = true;

            UpdateStatus(EventId.IvsRunning);

            if (wazuh != null)
            {
                WazuhUpdateStatus(wazuh.Status);
            }

            if (Dbytes != null)
            {
                DbytesUpdateStatus(Dbytes.Status);
            }

            if (Sysmon != null)
            {
                SysmonUpdateStatus(Sysmon.Status);
            }

            if (LmpService != null)
            {
                LmpStatusUpdate(LmpService.Status);
            }
        }

        protected override void OnStop()
        {
            _logger.Information("Stopping service");

            UpdateStatus(EventId.IvsStopped);

            _isRunning = false;
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            _logger.Information("System is shutting down");
        }

        private bool inTimer = false;
        private EventId avLastStatus = EventId.None;
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            if (inTimer)
            {
                return;
            }

            inTimer = true;

            if (ProcessExtensions.CheckProcessAsCurrentUser("IvsTray"))
            {
                _logger.Information("IvsTray is running.");
            }
            else
            {
                var ivsTrayFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "IvsTray.exe");
                _logger.Information($"IvsTray is not running. Starting... {ivsTrayFile}");
                ProcessExtensions.StartProcessAsCurrentUser(null, ivsTrayFile);
            }

            _logger.Information("Checking windows defender service");

            var status = ServiceHelper.AVStatus("Windows Defender");

            if (avLastStatus != status)
            {
                _logger.Information("Windows defender service status : Last: {avLastStatus}, New: {status}", avLastStatus, status);

                avLastStatus = status;

                UpdateStatus(status);

            }

            if (SysmonWrapper.Verify(true) == 0)
            {
                _logger.Information("Sysmon verified");
            }
            else
            {
                _logger.Information("Sysmon not avaiable");
            }

            if (OsQueryWrapper.Verify(true) == 0)
            {
                _logger.Information("OSQuery verified");
            }
            else
            {
                _logger.Information("OSQuery not available");
            }

            inTimer = false;
        }

        private void UpdateStatus(EventId eventId)
        {
            if (!_isRunning) return;

            try
            {
                var eventDetail = TrackingEventProvider.Instance.GetEventDetail(eventId);

                EventLog.WriteEntry(eventDetail.Message, eventDetail.EventType, eventDetail.Id);

                if (eventDetail.EventType == EventLogEntryType.SuccessAudit || eventDetail.EventType == EventLogEntryType.Information)
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
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
            }
        }
    }
}
