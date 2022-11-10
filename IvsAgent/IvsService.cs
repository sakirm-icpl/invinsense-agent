﻿using IvsAgent.Monitor;
using Serilog;
using IvsTray.Monitor;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using Common.Extensions;
using Common.Utils;
using Common.Persistance;
using ToolManager.AgentWrappers;
using System.Threading.Tasks;

namespace IvsAgent
{
    public partial class IvsService : ServiceBase
    {
        private readonly Timer avTimer = new Timer(60000);

        private readonly ExtendedServiceController wazuh;
        private readonly ExtendedServiceController Dbytes;
        private readonly ExtendedServiceController OsQuery;
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

            OsQuery = new ExtendedServiceController("osqueryd");
            OsQuery.StatusChanged += (object sender, ServiceStatusEventArgs e) => OsQueryUpdateStatus(e.Status);

            Sysmon = new ExtendedServiceController("Sysmon64");
            Sysmon.StatusChanged += (object sender, ServiceStatusEventArgs e) => SysmonUpdateStatus(e.Status);

            LmpService = new ExtendedServiceController("IvsAgent");
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
                toolRepository.CaptureEvent(ToolName.Wazuuh, InstallStatus.NotFound, RunningStatus.NotFound);
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running:
                    toolRepository.CaptureEvent(ToolName.Wazuuh, InstallStatus.Installed, RunningStatus.Running);
                    return;
                case ServiceControllerStatus.Stopped:
                    toolRepository.CaptureEvent(ToolName.Wazuuh, InstallStatus.Installed, RunningStatus.Stopped);
                    return;
                default:
                    toolRepository.CaptureEvent(ToolName.Wazuuh, InstallStatus.Installed, RunningStatus.Warning);
                    return;
            }
        }

        private void DbytesUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                toolRepository.CaptureEvent(ToolName.Dbytes, InstallStatus.NotFound, RunningStatus.NotFound);
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running:
                    toolRepository.CaptureEvent(ToolName.Dbytes, InstallStatus.Installed, RunningStatus.Running);
                    return;
                case ServiceControllerStatus.Stopped:
                    toolRepository.CaptureEvent(ToolName.Dbytes, InstallStatus.Installed, RunningStatus.Stopped);
                    return;
                default:
                    toolRepository.CaptureEvent(ToolName.Dbytes, InstallStatus.Installed, RunningStatus.Error);
                    return;
            }
        }

        private void OsQueryUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                toolRepository.CaptureEvent(ToolName.OsQuery, InstallStatus.NotFound, RunningStatus.NotFound);
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running:
                    toolRepository.CaptureEvent(ToolName.OsQuery, InstallStatus.Installed, RunningStatus.Running);
                    return;
                case ServiceControllerStatus.Stopped:
                    toolRepository.CaptureEvent(ToolName.OsQuery, InstallStatus.Installed, RunningStatus.Stopped);
                    return;
                default:
                    toolRepository.CaptureEvent(ToolName.OsQuery, InstallStatus.Installed, RunningStatus.Error);
                    return;
            }
        }

        private void SysmonUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                toolRepository.CaptureEvent(ToolName.Sysmon, InstallStatus.NotFound, RunningStatus.NotFound);
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running:
                    toolRepository.CaptureEvent(ToolName.Sysmon, InstallStatus.Installed, RunningStatus.Running);
                    return;
                case ServiceControllerStatus.Stopped:
                    toolRepository.CaptureEvent(ToolName.Sysmon, InstallStatus.Installed, RunningStatus.Stopped);
                    return;
                default:
                    toolRepository.CaptureEvent(ToolName.Sysmon, InstallStatus.Installed, RunningStatus.Error);
                    return;
            }
        }

        private void LmpStatusUpdate(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                toolRepository.CaptureEvent(ToolName.Lmp, InstallStatus.NotFound, RunningStatus.NotFound);
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running:
                    toolRepository.CaptureEvent(ToolName.Lmp, InstallStatus.Installed, RunningStatus.Running);
                    return;
                case ServiceControllerStatus.Stopped:
                    toolRepository.CaptureEvent(ToolName.Lmp, InstallStatus.Installed, RunningStatus.Stopped);
                    return;
                default:
                    toolRepository.CaptureEvent(ToolName.Lmp, InstallStatus.Installed, RunningStatus.Error);
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

            avTimer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            avTimer.Start();

            WazuhUpdateStatus(wazuh.Status);
            DbytesUpdateStatus(Dbytes.Status);
            SysmonUpdateStatus(Sysmon.Status);
            LmpStatusUpdate(LmpService.Status);

            Task task = Task.Delay(5000)
                .ContinueWith(t => VerifyDependencyAndInstall());

            task.Start();
        }

        protected override void OnStop()
        {
            _logger.Information("Stopping service");

            avTimer.Stop();

            toolRepository.CaptureEvent(ToolName.Lmp, InstallStatus.Installed, RunningStatus.Stopped);

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

                toolRepository.CaptureEvent(ToolName.Av, status, status == InstallStatus.Installed ? RunningStatus.Running : RunningStatus.Warning);
            }

            inTimer = false;
        }

        private void VerifyDependencyAndInstall()
        {
            if (SysmonWrapper.Verify(true) == 0)
            {
                _logger.Information($"Sysmon verified with status: {Sysmon.Status}");
                Sysmon.StartListening();
                _logger.Information($"Sysmon service listening: {Sysmon.Status}");
            }
            else
            {
                _logger.Error("Error in Sysmon installer");
            }

            if (OsQueryWrapper.Verify(true) == 0)
            {
                _logger.Information($"OSQuery verified with status: {OsQuery.Status}");
                OsQuery.StartListening();
                _logger.Information($"OSQuery service listening: {OsQuery.Status}");
            }
            else
            {
                _logger.Error("Error in OSQuery installer");
            }

            if (WazuhWrapper.Verify(true) == 0)
            {
                _logger.Information($"Wazuh verified with status: {wazuh.Status}");
                wazuh.StartListening();
                _logger.Information($"Wazuh service listening: {wazuh.Status}");
            }
            else
            {
                _logger.Error("Error in Wazuh installer");
            }

            if (DBytesWrapper.Verify(true) == 0)
            {
                _logger.Information($"dBytes verified with status: {Dbytes.Status}");
                Dbytes.StartListening();
                _logger.Information($"dBytes service listening: {Dbytes.Status}");
            }
            else
            {
                _logger.Error("Error in dBytes installer");
            }
        }
    }
}
