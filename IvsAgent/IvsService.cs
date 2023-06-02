using IvsAgent.Monitor;
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
using System.Collections.Generic;
using Common;
using System.Diagnostics.Eventing.Reader;
using Common.NamedPipes;

namespace IvsAgent
{
    public partial class IvsService : ServiceBase
    {
        private readonly ExtendedServiceController EdrServiceChecker;
        private readonly ExtendedServiceController DeceptionServiceChecker;
        private readonly ExtendedServiceController UserBehaviorServiceChecker;
        private readonly ExtendedServiceController AdvanceTelemetryServiceChecker;
        private readonly ExtendedServiceController LmpServiceChecker;

        private ServerPipe _serverPipe;

        private readonly EventLogWatcher avWatcher;

        private readonly ILogger _logger = Log.ForContext<IvsService>();

        private bool _isRunning = false;

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

            //Adding AV Watcher
            //TODO: Need to check if this is the best way to do it
            avWatcher = new EventLogWatcher("Microsoft-Windows-Windows Defender/Operational");
            avWatcher.EventRecordWritten += new EventHandler<EventRecordWrittenEventArgs>(DefenderEventWritten);
            avWatcher.Enabled = true;

            _sysTrayTimer.Elapsed += new ElapsedEventHandler(CheckUserSystemTray);

            CreateServerPipe();
        }

        private void CreateServerPipe()
        {
            _serverPipe = new ServerPipe(Constants.IvsName, p => p.StartStringReaderAsync());

            // Data received from client
            _serverPipe.DataReceived += (sndr, args) =>
            {
                _logger.Verbose($"Message received: {args.String}");
            };

            // Client connected
            _serverPipe.Connected += (sndr, args) =>
            {
                _logger.Debug("Client is connected.");
                SendToolStatuses();
            };

            _serverPipe.PipeClosed += (sndr, args) =>
            {
                _logger.Debug("Client is disconnected. Creating new server pipe...");
                CreateServerPipe();
            };
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
                    SendStatusUpdate(new ToolStatus(ToolName.EndpointProtection, InstallStatus.Installed, RunningStatus.Stopped));
                }
                else if (arg.EventRecord.Id == 5000)
                {
                    SendStatusUpdate(new ToolStatus(ToolName.EndpointProtection, InstallStatus.Installed, RunningStatus.Running));
                }
            }
            else
            {
                _logger.Debug("Windows Defender Event reading error: {Message}", arg.EventException.Message);
            }
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            var message = $"IvsService.OnSessionChange {DateTime.Now.ToLongTimeString()} - Session change notice received: {changeDescription.Reason}  Session ID: {changeDescription.SessionId}";

            _logger.Information(message);
            EventLog.WriteEntry(message);

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

        #region ToolStatusCheck

        private void EdrUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                _logger.Information("{Name} not found", ToolName.EndpointDetectionAndResponse);
                SendStatusUpdate(new ToolStatus(ToolName.EndpointDetectionAndResponse, InstallStatus.NotFound, RunningStatus.NotFound));
                return;
            }

            _logger.Information("{Name} is {Status}", ToolName.EndpointDetectionAndResponse, status.Value);

            switch (status.Value)
            {
                case ServiceControllerStatus.Running:
                    SendStatusUpdate(new ToolStatus(ToolName.EndpointDetectionAndResponse, InstallStatus.Installed, RunningStatus.Running));
                    return;
                case ServiceControllerStatus.Stopped:
                    SendStatusUpdate(new ToolStatus(ToolName.EndpointDetectionAndResponse, InstallStatus.Installed, RunningStatus.Stopped));
                    return;
                default:
                    return;
            }
        }

        private void DeceptionUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                _logger.Information("{Name} not found", ToolName.EndpointDeception);
                SendStatusUpdate(new ToolStatus(ToolName.EndpointDeception, InstallStatus.NotFound, RunningStatus.NotFound));
                return;
            }

            _logger.Information("{Name} is {Status}", ToolName.EndpointDeception, status.Value);

            switch (status.Value)
            {
                case ServiceControllerStatus.Running:
                    SendStatusUpdate(new ToolStatus(ToolName.EndpointDeception, InstallStatus.Installed, RunningStatus.Running));
                    return;
                case ServiceControllerStatus.Stopped:
                    SendStatusUpdate(new ToolStatus(ToolName.EndpointDeception, InstallStatus.Installed, RunningStatus.Stopped));
                    return;
                default:
                    return;
            }
        }

        private void OsQueryUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                _logger.Information("{Name} not found", ToolName.UserBehaviorAnalytics);
                SendStatusUpdate(new ToolStatus(ToolName.UserBehaviorAnalytics, InstallStatus.NotFound, RunningStatus.NotFound));
                return;
            }

            _logger.Information("{Name} is {Status}", ToolName.UserBehaviorAnalytics, status.Value);

            switch (status.Value)
            {
                case ServiceControllerStatus.Running:
                    SendStatusUpdate(new ToolStatus(ToolName.UserBehaviorAnalytics, InstallStatus.Installed, RunningStatus.Running));
                    return;
                case ServiceControllerStatus.Stopped:
                    SendStatusUpdate(new ToolStatus(ToolName.UserBehaviorAnalytics, InstallStatus.Installed, RunningStatus.Stopped));
                    return;
                default:
                    return;
            }
        }

        private void SysmonUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                _logger.Information("{Name} not found", ToolName.AdvanceTelemetry);
                SendStatusUpdate(new ToolStatus(ToolName.AdvanceTelemetry, InstallStatus.NotFound, RunningStatus.NotFound));
                return;
            }

            _logger.Information("{Name} is {Status}", ToolName.AdvanceTelemetry, status.Value);

            switch (status.Value)
            {
                case ServiceControllerStatus.Running:
                    SendStatusUpdate(new ToolStatus(ToolName.AdvanceTelemetry, InstallStatus.Installed, RunningStatus.Running));
                    return;
                case ServiceControllerStatus.Stopped:
                    SendStatusUpdate(new ToolStatus(ToolName.AdvanceTelemetry, InstallStatus.Installed, RunningStatus.Stopped));
                    return;
                default:
                    return;
            }
        }

        private void LmpStatusUpdate(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                _logger.Information("{Name} not found", ToolName.LateralMovementProtection);
                SendStatusUpdate(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.NotFound, RunningStatus.NotFound));
                return;
            }

            _logger.Information("{Name} is {Status}", ToolName.LateralMovementProtection, status.Value);

            switch (status.Value)
            {
                case ServiceControllerStatus.Running:
                    SendStatusUpdate(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.Installed, RunningStatus.Running));
                    return;
                case ServiceControllerStatus.Stopped:
                    SendStatusUpdate(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.Installed, RunningStatus.Stopped));
                    return;
                default:
                    return;
            }
        }

        #endregion

        protected override void OnStart(string[] args)
        {
            if (_isRunning)
            {
                _logger.Information("Invinsense service is already running.");
                return;
            }

            _isRunning = true;

            _logger.Information("Starting Invinsense service...");

            _sysTrayTimer.Start();

            _logger.Information("Scheduling dependency after 5 sec...");

            //TODO: Need to move tool installation logic to separate class.
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(5000);
                VerifyDependencyAndInstall();
            });

            _logger.Information("Starting IPC server");

            _logger.Information("Invinsense service started.");
            SendStatusUpdate(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.Installed, RunningStatus.Running));
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicebase.requestadditionaltime?view=netframework-4.8
        /// https://stackoverflow.com/questions/125964/how-to-stop-a-windows-service-that-is-stuck-on-stopping
        /// Check if installation is in progress and wait for it to complete.
        /// Try to use : RequestAdditionalTime(1000 * 60 * 2);
        /// </summary>
        protected override void OnStop()
        {
            _isRunning = false;

            _logger.Information("Stopping service");

            try
            {
                //Stop the timer.
                _sysTrayTimer.Stop();

                //Update tray for LMP.
                SendStatusUpdate(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.Installed, RunningStatus.Stopped));

                //Clean up pipe variables.
                _logger.Information("Cleaning up pipe server");

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while stopping service");
            }

            base.OnStop();
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

        /// <summary>
        /// Custom command handler.
        /// </summary>
        /// <param name="command"></param>
        protected override void OnCustomCommand(int command)
        {
            _logger.Information($"CustomCommand {command}");
            base.OnCustomCommand(command);

            if (command == 130)
            {
                Stop();
            }
        }

        /// <summary>
        /// Logging unhandled exceptions.
        /// </summary>
        protected override void OnShutdown()
        {
            _logger.Information("System is shutting down");
            base.OnShutdown();
        }

        #region Checking System Tray Periodically

        private readonly Timer _sysTrayTimer = new Timer { AutoReset = true, Interval = 1000 * 60 * 1 }; //1 min

        private bool inTimer = false;

        private void CheckUserSystemTray(object source, ElapsedEventArgs e)
        {
            if (inTimer)
            {
                return;
            }

            inTimer = true;

            try
            {
                //Check the user seesion is active or not
                var processes = Process.GetProcesses();
                bool isSessionActive = processes.Any(p => p.SessionId > 0 && p.ProcessName != "Idel");

                _logger.Verbose($"Is session active: {isSessionActive}");

                if (isSessionActive)
                {
                    Process trayApp = processes.FirstOrDefault(pp => pp.ProcessName.StartsWith("IvsTray"));

                    //TODO: Evaluate below scenario for multiple user sessions.
                    //Process myExplorer = Process.GetProcesses().FirstOrDefault(pp => pp.ProcessName == "explorer" && pp.SessionId == trayApp.SessionId);

                    if (trayApp != null)
                    {
                        _logger.Verbose($"Active Session App: {trayApp.ProcessName} - {trayApp.SessionId}");
                       
                        if(_logger.IsEnabled(Serilog.Events.LogEventLevel.Verbose))
                        {
                            SendToolStatuses();
                        }
                    }
                    else
                    {
                        var ivsTrayFile = CommonUtils.ConstructFromRoot("..\\IvsTray\\IvsTray.exe");
                        _logger.Information($"IvsTray is not running. Starting... {ivsTrayFile}");
                        ProcessExtensions.RunInActiveUserSession(null, ivsTrayFile);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while checking system tray");
            }

            inTimer = false;
        }

        #endregion

        /// <summary>
        /// TODO: Need to move tool installation logic to separate class.
        /// </summary>
        private void VerifyDependencyAndInstall()
        {
            _logger.Information("Starting dependency check and installation");

            _logger.Information("Checking Sysmon");
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

            _logger.Information("Checking OsQuery");
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

            _logger.Information("Checking Wazuh");
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

            _logger.Information("Checking DBytes");
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

        private ToolStatus GetToolStatus(string toolName)
        {
            switch (toolName)
            {
                case ToolName.AdvanceTelemetry:
                    return new ToolStatus(ToolName.AdvanceTelemetry, AdvanceTelemetryServiceChecker.InstallStatus, AdvanceTelemetryServiceChecker.RunningStatus);
                case ToolName.EndpointDeception:
                    return new ToolStatus(ToolName.EndpointDeception, EdrServiceChecker.InstallStatus, EdrServiceChecker.RunningStatus);
                case ToolName.EndpointDetectionAndResponse:
                    return new ToolStatus(ToolName.EndpointDetectionAndResponse, DeceptionServiceChecker.InstallStatus, DeceptionServiceChecker.RunningStatus);
                case ToolName.EndpointProtection:
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
                case ToolName.LateralMovementProtection:
                    return new ToolStatus(ToolName.LateralMovementProtection, LmpServiceChecker.InstallStatus, LmpServiceChecker.RunningStatus);
                case ToolName.UserBehaviorAnalytics:
                    return new ToolStatus(ToolName.UserBehaviorAnalytics, UserBehaviorServiceChecker.InstallStatus, UserBehaviorServiceChecker.RunningStatus);
                default:
                    throw new Exception($"Unknown tool name {toolName}");
            }
        }

        #region IPC Block

        private void SendToolStatuses()
        {
            var skipDeception = ToolRepository.CanSkipMonitoring(ToolName.EndpointDeception);
            var statuses = new List<ToolStatus>();
            if (!skipDeception)
            {
                statuses.Add(GetToolStatus(ToolName.EndpointDeception));
            }

            statuses.AddRange(new List<ToolStatus>
                {
                    GetToolStatus(ToolName.EndpointProtection),
                    GetToolStatus(ToolName.UserBehaviorAnalytics),
                    GetToolStatus(ToolName.EndpointDetectionAndResponse),
                    GetToolStatus(ToolName.AdvanceTelemetry),
                    GetToolStatus(ToolName.LateralMovementProtection)
                });

            var message = Newtonsoft.Json.JsonConvert.SerializeObject(statuses);
            _logger.Debug($"Sending status to tray {string.Join(", ", statuses.Select(x => x))}");
            _serverPipe.WriteString(message);
        }

        private void SendStatusUpdate(ToolStatus status)
        {
            //Capture the event in the event logger
            ToolRepository.CaptureEvent(status);

            //Send the status to the client
            var statuses = new List<ToolStatus> { status };

            var message = Newtonsoft.Json.JsonConvert.SerializeObject(statuses);
            _logger.Information($"Sending status to tray {string.Join(", ", statuses.Select(x => x))}");
            _serverPipe.WriteString(message);
        }

        #endregion
    }
}
