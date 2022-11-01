using IvsAgent.Monitor;
using Serilog;
using IvsTray.Monitor;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using Common.Extensions;
using IvsAgent.AgentWrappers;
using Common.Utils;
using Common.Persistance;

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

        private readonly ToolRepository toolRepository;

        private bool _isRunning = false;

        public IvsService()
        {
            InitializeComponent();

            AutoLog = false;
            CanShutdown = true;

            CanPauseAndContinue = true;
            CanHandleSessionChangeEvent = true;

            toolRepository = new ToolRepository();

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
                toolRepository.CaptureInstallationEvent(ToolName.Wazuuh, InstallStatus.NotFound);
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.Running:
                    toolRepository.CaptureRunningEvent(ToolName.Wazuuh, RunningStatus.Running);
                    return;
                case ServiceControllerStatus.Stopped:
                    toolRepository.CaptureRunningEvent(ToolName.Wazuuh, RunningStatus.Stopped);
                    return;
                default:
                    toolRepository.CaptureRunningEvent(ToolName.Wazuuh, RunningStatus.Warning);
                    return;
            }
        }

        private void DbytesUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                toolRepository.CaptureInstallationEvent(ToolName.Dbytes, InstallStatus.NotFound);
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.Running:
                    toolRepository.CaptureRunningEvent(ToolName.Dbytes, RunningStatus.Running);
                    return;
                case ServiceControllerStatus.Stopped:
                    toolRepository.CaptureRunningEvent(ToolName.Dbytes, RunningStatus.Stopped);
                    return;
                default:
                    toolRepository.CaptureRunningEvent(ToolName.Dbytes, RunningStatus.Error);
                    return;
            }
        }

        private void SysmonUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                toolRepository.CaptureInstallationEvent(ToolName.Sysmon, InstallStatus.NotFound);
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.Running:
                    toolRepository.CaptureRunningEvent(ToolName.Sysmon, RunningStatus.Running);
                    return;
                case ServiceControllerStatus.Stopped:
                    toolRepository.CaptureRunningEvent(ToolName.Sysmon, RunningStatus.Stopped);
                    return;
                default:
                    toolRepository.CaptureRunningEvent(ToolName.Sysmon, RunningStatus.Error);
                    return;
            }
        }

        private void LmpStatusUpdate(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                toolRepository.CaptureInstallationEvent(ToolName.Lmp, InstallStatus.NotFound);
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.Running:
                    toolRepository.CaptureRunningEvent(ToolName.Lmp, RunningStatus.Running);
                    return;
                case ServiceControllerStatus.Stopped:
                    toolRepository.CaptureRunningEvent(ToolName.Lmp, RunningStatus.Stopped);
                    return;
                default:
                    toolRepository.CaptureRunningEvent(ToolName.Lmp, RunningStatus.Error);
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

            toolRepository.CaptureRunningEvent(ToolName.Wazuuh, RunningStatus.Running);

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

            toolRepository.CaptureRunningEvent(ToolName.Wazuuh, RunningStatus.Stopped);

            _isRunning = false;
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            _logger.Information("System is shutting down");
        }

        private bool inTimer = false;
        private InstallStatus avLastStatus = InstallStatus.NotFound;

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
                var ivsTrayFile = CommonUtils.GetAbsoletePath("..\\IvsTray\\IvsTray.exe");
                _logger.Information($"IvsTray is not running. Starting... {ivsTrayFile}");
                ProcessExtensions.StartProcessAsCurrentUser(null, ivsTrayFile);
            }

            _logger.Information("Checking windows defender service");

            var status = ServiceHelper.AVStatus("Windows Defender");

            if (avLastStatus != status)
            {
                _logger.Information("Windows defender service status : Last: {avLastStatus}, New: {status}", avLastStatus, status);

                avLastStatus = status;

                toolRepository.CaptureInstallationEvent(ToolName.Av, status);
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
    }
}
