﻿using IvsAgent.Monitor;
using Serilog;
using IvsTray.Monitor;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using Common.Utils;
using Common.Persistance;
using ToolManager.AgentWrappers;
using System.Threading.Tasks;
using IvsAgent.Extensions;
using IvsAgent.AvHelper;
using System.Linq;
using System.IO.Pipes;
using System.IO;
using System.Collections.Generic;
using Common;

namespace IvsAgent
{
    public partial class IvsService : ServiceBase
    {
        private readonly Timer avTimer = new Timer(15000);

        private readonly ExtendedServiceController EdrServiceChecker;
        private readonly ExtendedServiceController DeceptionServiceChecker;
        private readonly ExtendedServiceController UserBehaviorServiceChecker;
        private readonly ExtendedServiceController AdvanceTelemetryServiceChecker;
        private readonly ExtendedServiceController LmpServiceChecker;

        private readonly ILogger _logger = Log.ForContext<IvsService>();

        private bool _isRunning = false;

        private NamedPipeServerStream _pipeServer;
        private StreamWriter _writer;

        public IvsService()
        {
            InitializeComponent();

            AutoLog = false;

            //Allow service to handle shutdown
            CanShutdown = true;

            //TODO: this is for test only. Need to set false when release
            //Stop service to stop
            CanStop = true;

            //Stop service to pause and continue
            CanPauseAndContinue = false;

            CanHandleSessionChangeEvent = true;

            EdrServiceChecker = new ExtendedServiceController("WazuhSvc");
            EdrServiceChecker.StatusChanged += (object sender, ServiceStatusEventArgs e) => EdrUpdateStatus(e.Status);

            DeceptionServiceChecker = new ExtendedServiceController("DBytesService");
            DeceptionServiceChecker.StatusChanged += (object sender, ServiceStatusEventArgs e) => DeceptionUpdateStatus(e.Status);

            UserBehaviorServiceChecker = new ExtendedServiceController("osqueryd");
            UserBehaviorServiceChecker.StatusChanged += (object sender, ServiceStatusEventArgs e) => OsQueryUpdateStatus(e.Status);

            AdvanceTelemetryServiceChecker = new ExtendedServiceController("Sysmon64");
            AdvanceTelemetryServiceChecker.StatusChanged += (object sender, ServiceStatusEventArgs e) => SysmonUpdateStatus(e.Status);

            LmpServiceChecker = new ExtendedServiceController("IvsAgent");
            LmpServiceChecker.StatusChanged += (object sender, ServiceStatusEventArgs e) => LmpStatusUpdate(e.Status);
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

        private void EdrUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                Log.Logger.Information("EndpointDecetionAndResponse NotFound");
                SendStatusUpdate(new ToolStatus(ToolName.EndpointDecetionAndResponse, InstallStatus.NotFound, RunningStatus.NotFound));
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running:
                    Log.Logger.Information("EndpointDecetionAndResponse is Running");
                    SendStatusUpdate(new ToolStatus(ToolName.EndpointDecetionAndResponse, InstallStatus.Installed, RunningStatus.Running));
                    return;
                case ServiceControllerStatus.Stopped:
                    Log.Logger.Information("EndpointDecetionAndResponse is Stopped");
                    SendStatusUpdate(new ToolStatus(ToolName.EndpointDecetionAndResponse, InstallStatus.Installed, RunningStatus.Stopped));
                    return;
                default:
                    Log.Logger.Information("There is Warning in EndpointDecetionAndResponse");
                    SendStatusUpdate(new ToolStatus(ToolName.EndpointDecetionAndResponse, InstallStatus.Installed, RunningStatus.Warning));
                    return;
            }
        }

        private void DeceptionUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                Log.Logger.Information("EndpointDeception NotFound");
                SendStatusUpdate(new ToolStatus(ToolName.EndpointDeception, InstallStatus.NotFound, RunningStatus.NotFound));
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running:
                    Log.Logger.Information("EndpointDeception is Running");
                    SendStatusUpdate(new ToolStatus(ToolName.EndpointDeception, InstallStatus.Installed, RunningStatus.Running));
                    return;
                case ServiceControllerStatus.Stopped:
                    Log.Logger.Information("EndpointDeception is Stopped");
                    SendStatusUpdate(new ToolStatus(ToolName.EndpointDeception, InstallStatus.Installed, RunningStatus.Stopped));
                    return;
                default:
                    Log.Logger.Information("There is Warning in EndpointDeception");
                    SendStatusUpdate(new ToolStatus(ToolName.EndpointDeception, InstallStatus.Installed, RunningStatus.Warning));
                    return;
            }
        }

        private void OsQueryUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                Log.Logger.Information("UserBehaviorAnalytics NotFound");
                SendStatusUpdate(new ToolStatus(ToolName.UserBehaviorAnalytics, InstallStatus.NotFound, RunningStatus.NotFound));
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running:
                    Log.Logger.Information("UserBehaviorAnalytics is Running");
                    SendStatusUpdate(new ToolStatus(ToolName.UserBehaviorAnalytics, InstallStatus.Installed, RunningStatus.Running));
                    return;
                case ServiceControllerStatus.Stopped:
                    Log.Logger.Information("UserBehaviorAnalytics is Stopped");
                    SendStatusUpdate(new ToolStatus(ToolName.UserBehaviorAnalytics, InstallStatus.Installed, RunningStatus.Stopped));
                    return;
                default:
                    Log.Logger.Information("There is Warning in UserBehaviorAnalytics");
                    SendStatusUpdate(new ToolStatus(ToolName.UserBehaviorAnalytics, InstallStatus.Installed, RunningStatus.Warning));
                    return;
            }
        }

        private void SysmonUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                Log.Logger.Information("AdvanceTelemetry NotFound");
                SendStatusUpdate(new ToolStatus(ToolName.AdvanceTelemetry, InstallStatus.NotFound, RunningStatus.NotFound));
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running:
                    Log.Logger.Information("AdvanceTelemetry is Running");
                    SendStatusUpdate(new ToolStatus(ToolName.AdvanceTelemetry, InstallStatus.Installed, RunningStatus.Running));
                    return;
                case ServiceControllerStatus.Stopped:
                    Log.Logger.Information("AdvanceTelemetry is Stopped");
                    SendStatusUpdate(new ToolStatus(ToolName.AdvanceTelemetry, InstallStatus.Installed, RunningStatus.Stopped));
                    return;
                default:
                    Log.Logger.Information("There is Warning in AdvanceTelemetry");
                    SendStatusUpdate(new ToolStatus(ToolName.AdvanceTelemetry, InstallStatus.Installed, RunningStatus.Warning));
                    return;
            }
        }

        private void LmpStatusUpdate(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                Log.Logger.Information("LateralMovementProtection NotFound");
                SendStatusUpdate(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.NotFound, RunningStatus.NotFound));
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running:
                    Log.Logger.Information("LateralMovementProtection is Running");
                    SendStatusUpdate(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.Installed, RunningStatus.Running));
                    return;
                case ServiceControllerStatus.Stopped:
                    Log.Logger.Information("LateralMovementProtection is Stopped");
                    SendStatusUpdate(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.Installed, RunningStatus.Stopped));
                    return;
                default:
                    Log.Logger.Information("There is Warning in LateralMovementProtection");
                    SendStatusUpdate(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.Installed, RunningStatus.Warning));
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

            _logger.Information("Scheduling dependency after 5 sec...");

            //TODO: Need to move tool installation logic to separate class.
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(5000);
                VerifyDependencyAndInstall();
            });

            _logger.Information("Task added...");
            SendStatusUpdate(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.Installed, RunningStatus.Running));

            _pipeServer = new NamedPipeServerStream(Constants.IvsName, PipeDirection.Out, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            Task.Run(HandleClientConnections);
        }

        protected override void OnStop()
        {
            _logger.Information("Stopping service");
            avTimer.Stop();

            SendStatusUpdate(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.Installed, RunningStatus.Stopped));

            _pipeServer?.Dispose();

            _isRunning = false;

            //TODO: Study RequestAdditionalTime functionality for this context.
            //https://docs.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicebase.requestadditionaltime?view=netframework-4.8
            //https://stackoverflow.com/questions/125964/how-to-stop-a-windows-service-that-is-stuck-on-stopping
            //Check if installation is in progress and wait for it to complete.
            //RequestAdditionalTime(1000 * 60 * 2);

            Stop();
        }

        protected override void OnPause()
        {
            _logger.Information("Agent pause requested");
            base.OnPause();
        }

        protected override void OnContinue()
        {
            _logger.Information("Agent resume requested");
            base.OnContinue();
        }

        protected override void OnCustomCommand(int command)
        {
            _logger.Information($"CustomCommand {command}");
            base.OnCustomCommand(command);

            if (command == 130)
            {
                Stop();
            }
            else if (command == 140)
            {
                var avStatuses = AvMonitor.ListAvStatuses();

                var activeAvStatus = avStatuses.FirstOrDefault(x => x.IsAvEnabled);
                if (activeAvStatus.AvName == "Windows Defender")
                {
                    WinDefender wd = new WinDefender($"-Scan -ScanType 2");
                    _logger.Information("Windows Defender Starts Scanning..");

                    var isVirus = WinDefender.IsVirus();
                    _logger.Information($"{isVirus}");
                }
            }
            else if (command == 141)
            {
                var avStatuses = AvMonitor.ListAvStatuses();

                var activeAvStatus = avStatuses.FirstOrDefault(x => x.IsAvEnabled);
                if (activeAvStatus.AvName == "Windows Defender")
                {
                    WinDefender wd = new WinDefender($"-Scan -ScanType 1");
                    _logger.Information("Windows Defender Starts Scanning..");

                    var isVirus = WinDefender.IsVirus();
                    _logger.Information($"{isVirus}");
                }
            }
        }

        protected override void OnShutdown()
        {
            _logger.Information("System is shutting down");
            string serviceName = "IvsAgent";

            ServiceController[] services = ServiceController.GetServices();
            ServiceController sc = services.FirstOrDefault(s => s.ServiceName == serviceName);

            if (sc != null)
            {
                sc.Stop();
            }
            else
            {
                _logger.Information($"Service {serviceName} not found");
            }
            base.OnShutdown();
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

            //Check the user seesion is active or not
            bool isSessionActive = Process.GetProcesses().Any(p => p.SessionId > 0 && p.ProcessName != "Idel");
            if (isSessionActive)
            {
                if (ProcessExtensions.CheckProcessAsCurrentUser("IvsTray"))
                {
                    _logger.Information("IvsTray is running.");
                }
                else
                {
                    var ivsTrayFile = CommonUtils.ConstructFromRoot("..\\IvsTray\\IvsTray.exe");
                    _logger.Information($"IvsTray is not running. Starting... {ivsTrayFile}");
                    ProcessExtensions.StartProcessAsCurrentUser(null, ivsTrayFile);
                }
            }
            var avStatuses = AvMonitor.ListAvStatuses();
            var enabledAvStatus = avStatuses.FirstOrDefault(x => x.IsAvEnabled && x.IsAvUptoDate);
            var disabledAvStatus = avStatuses.FirstOrDefault(x => x.IsAvDisabled);

            InstallStatus currentAvStatus;
            RunningStatus currentRunningStatus;

            if (enabledAvStatus == null && disabledAvStatus == null)
            {
                // No antivirus is detected
                currentAvStatus = InstallStatus.Error;
                currentRunningStatus = RunningStatus.Stopped;
                _logger.Information($"{enabledAvStatus.AvName} is {currentRunningStatus}");
            }
            else if (enabledAvStatus != null)
            {
                // An enabled and up-to-date antivirus is detected
                currentAvStatus = InstallStatus.Installed;
                currentRunningStatus = RunningStatus.Running;
                _logger.Information($"{enabledAvStatus.AvName} is {currentRunningStatus}");
            }
            else
            {
                // A disabled antivirus is detected
                currentAvStatus = InstallStatus.Installed;
                currentRunningStatus = RunningStatus.Error;
                _logger.Information($"{disabledAvStatus.AvName} is having {currentRunningStatus}");
            }

            if (avLastStatus != currentAvStatus)
            {
                _logger.Information($"Antivirus status changed from {avLastStatus} to {currentAvStatus}");
                avLastStatus = currentAvStatus;
            }

            SendStatusUpdate(new ToolStatus(ToolName.EndpointProtection, currentAvStatus, currentRunningStatus));

            inTimer = false;
        }

        private void VerifyDependencyAndInstall()
        {
            if (SysmonWrapper.Verify(true) == 0)
            {
                _logger.Information($"SysmonServiceVerified with status {AdvanceTelemetryServiceChecker.Status}");
                AdvanceTelemetryServiceChecker.StartListening();
                SysmonUpdateStatus(AdvanceTelemetryServiceChecker.Status);
                _logger.Information($"SysmonServiceListening with status {AdvanceTelemetryServiceChecker.Status}");
            }
            else
            {
                _logger.Error("Error in Sysmon installer");
            }

            if (OsQueryWrapper.Verify(true) == 0)
            {
                _logger.Information("OsQueryVerified with status {OsQuery.Status}");
                UserBehaviorServiceChecker.StartListening();
                OsQueryUpdateStatus(UserBehaviorServiceChecker.Status);
                _logger.Information($"OsQueryListening with status {UserBehaviorServiceChecker.Status}");
            }
            else
            {
                _logger.Error("Error in OSQuery installer");
            }

            if (WazuhWrapper.Verify(true) == 0)
            {
                _logger.Information($"WazuhServiceVerified with status {EdrServiceChecker.Status}");
                EdrServiceChecker.StartListening();
                EdrUpdateStatus(EdrServiceChecker.Status);
                _logger.Information($"WazuhServiceListening with status {EdrServiceChecker.Status}");
            }
            else
            {
                _logger.Error("Error in Wazuh installer");
            }

            if (DBytesWrapper.Verify(true) == 0)
            {
                _logger.Information($"DbytesServiceVerified with status {DeceptionServiceChecker.Status}");
                DeceptionServiceChecker.StartListening();
                DeceptionUpdateStatus(DeceptionServiceChecker.Status);
                _logger.Information($"DbytesServiceListening with status {DeceptionServiceChecker.Status}");
            }
            else
            {
                _logger.Information("Error in dBytes installer");
            }

            _logger.Information("Adding fake user");
            UserExtensions.EnsureFakeUser("maintenance", "P@$$w0rd");
        }

        private async Task HandleClientConnections()
        {
            while (true)
            {
                await _pipeServer.WaitForConnectionAsync();
                _writer = new StreamWriter(_pipeServer)
                {
                    AutoFlush = true
                };

                var statuses = new List<ToolStatus>
                {
                    GetToolStatus(ToolName.EndpointDeception),
                    GetToolStatus(ToolName.EndpointProtection),
                    GetToolStatus(ToolName.UserBehaviorAnalytics),
                    GetToolStatus(ToolName.EndpointDecetionAndResponse),
                    GetToolStatus(ToolName.AdvanceTelemetry),
                    GetToolStatus(ToolName.LateralMovementProtection)
                };

                await _writer.WriteLineAsync(Newtonsoft.Json.JsonConvert.SerializeObject(statuses));

                _pipeServer.WaitForPipeDrain();
                _pipeServer.Disconnect();
                _writer = null;
            }
        }

        private ToolStatus GetToolStatus(string toolName)
        {
            switch (toolName)
            {
                case ToolName.AdvanceTelemetry:
                    return new ToolStatus(ToolName.AdvanceTelemetry, AdvanceTelemetryServiceChecker.InstallStatus, AdvanceTelemetryServiceChecker.RunningStatus);
                case ToolName.EndpointDeception:
                    return new ToolStatus(ToolName.EndpointDeception, EdrServiceChecker.InstallStatus, EdrServiceChecker.RunningStatus);
                case ToolName.EndpointDecetionAndResponse:
                    return new ToolStatus(ToolName.EndpointDecetionAndResponse, DeceptionServiceChecker.InstallStatus, DeceptionServiceChecker.RunningStatus);
                case ToolName.EndpointProtection:
                    return new ToolStatus(ToolName.EndpointProtection, InstallStatus.Installed, RunningStatus.Running);
                case ToolName.LateralMovementProtection:
                    return new ToolStatus(ToolName.LateralMovementProtection, LmpServiceChecker.InstallStatus, LmpServiceChecker.RunningStatus);
                case ToolName.UserBehaviorAnalytics:
                    return new ToolStatus(ToolName.UserBehaviorAnalytics, UserBehaviorServiceChecker.InstallStatus, UserBehaviorServiceChecker.RunningStatus);
                default:
                    throw new Exception($"Unknown tool name {toolName}");
            }
        }

        private void SendStatusUpdate(ToolStatus status)
        {
            //Capture the event in the event logger
            ToolRepository.CaptureEvent(status);

            //Send the status to the client
            var statuses = new List<ToolStatus> { status };
            _writer?.WriteLineAsync(Newtonsoft.Json.JsonConvert.SerializeObject(statuses));
        }
    }
}
