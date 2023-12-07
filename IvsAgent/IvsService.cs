using IvsAgent.Monitor;
using Serilog;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using Common.Persistence;
using ToolManager;
using System.Threading.Tasks;
using IvsAgent.Extensions;
using System.Linq;
using System.Collections.Generic;
using Common;
using Common.NamedPipes;

namespace IvsAgent
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicebase.requestadditionaltime?view=netframework-4.8
    /// https://stackoverflow.com/questions/125964/how-to-stop-a-windows-service-that-is-stuck-on-stopping
    /// Check if installation is in progress and wait for it to complete.
    /// Try to use : RequestAdditionalTime(1000 * 60 * 2);
    /// </summary>
    public partial class IvsService : ServiceBase
    {
        private readonly ExtendedServiceController EdrServiceChecker;
        private readonly ExtendedServiceController EcdServiceChecker;
        private readonly ExtendedServiceController UbaServiceChecker;
        private readonly ExtendedServiceController AteleServiceChecker;
        private readonly ExtendedServiceController LmpServiceChecker;

        private ServerPipe _serverPipe;

        private readonly ILogger _logger = Log.ForContext<IvsService>();

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

            //Allow service to handle session change
            CanHandleSessionChangeEvent = true;

            EdrServiceChecker = new ExtendedServiceController("WazuhSvc");
            EdrServiceChecker.StatusChanged += (object sender, ServiceStatusEventArgs e) => EdrUpdateStatus(e.Status);

            EcdServiceChecker = new ExtendedServiceController("DBytesService");
            EcdServiceChecker.StatusChanged += (object sender, ServiceStatusEventArgs e) => EcdUpdateStatus(e.Status);

            UbaServiceChecker = new ExtendedServiceController("osqueryd");
            UbaServiceChecker.StatusChanged += (object sender, ServiceStatusEventArgs e) => OsQueryUpdateStatus(e.Status);

            AteleServiceChecker = new ExtendedServiceController("Sysmon64");
            AteleServiceChecker.StatusChanged += (object sender, ServiceStatusEventArgs e) => SysmonUpdateStatus(e.Status);

            LmpServiceChecker = new ExtendedServiceController("IvsAgent");
            LmpServiceChecker.StatusChanged += (object sender, ServiceStatusEventArgs e) => LmpStatusUpdate(e.Status);

            AvStatusWatcher.Instance.AvStatusChaned += (object sender, ToolStatus e) => SendStatusUpdate(e);
        }

        protected override void OnStart(string[] args)
        {
            _logger.Verbose("IvsService.OnStart");

            _logger.Information("Starting IPC server");
            CreateServerPipe();

            _logger.Information("Start watching IvsTray App");
            IvsTrayMonitor.Instance.StartMonitoring();

            _logger.Information("Start watching windows defender events");
            AvStatusWatcher.Instance.StartMonitoring();

            //TODO: Need to move tool installation logic to separate class.
            Task.Factory.StartNew(async () =>
            {
                _logger.Information("Start waiting for tool verification");
                await Task.Delay(5000);
                VerifyDependencyAndInstall();
            });

            _logger.Information("Invinsense service started.");
            SendStatusUpdate(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.Installed, RunningStatus.Running));
        }

        protected override void OnStop()
        {
            _logger.Information("Stopping service");

            try
            {
                _logger.Information("Stopping IvsTray watcher");
                IvsTrayMonitor.Instance.StopMonitoring();

                _logger.Information("Stopping windows defender watcher");
                AvStatusWatcher.Instance.StopMonitoring();

                _logger.Information("Cleaning up pipe server");
                DestroyServerPipe();

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while stopping service");
            }
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

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            var message = $"IvsService.OnSessionChange {DateTime.Now.ToLongTimeString()} - UserSession Changed : {changeDescription.Reason}  Session ID: {changeDescription.SessionId}";
            _logger.Information(message);
            EventLog.WriteEntry(message);
        }

        #region IPC Block

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

        private void DestroyServerPipe()
        {
            _serverPipe?.Close();
            _serverPipe = null;
        }

        private void SendToolStatuses()
        {
            var skipEndpointDeception = ToolRegistry.CanSkipMonitoring(ToolName.EndpointDeception);
            
            var trayStatus = new TrayStatus();
            
            //Reading system variable for isolation mode
            
            if (!skipEndpointDeception)
            {
                trayStatus.ToolStatuses.Add(GetToolStatus(ToolName.EndpointDeception));
            }

            trayStatus.ToolStatuses.AddRange(new List<ToolStatus>
                {
                    GetToolStatus(ToolName.EndpointProtection),
                    GetToolStatus(ToolName.UserBehaviorAnalytics),
                    GetToolStatus(ToolName.EndpointDetectionAndResponse),
                    GetToolStatus(ToolName.AdvanceTelemetry),
                    GetToolStatus(ToolName.LateralMovementProtection)
                });

            var message = Newtonsoft.Json.JsonConvert.SerializeObject(trayStatus);
            _logger.Verbose($"Sending status to tray. ErrorCode:{trayStatus.ErrorCode}, Message:{trayStatus.ErrorMessage}, {string.Join(", ", trayStatus.ToolStatuses.Select(x => x))}");
            _serverPipe.WriteString(message);
        }

        private void SendStatusUpdate(ToolStatus status)
        {
            //Capture the event in the event logger
            var eventInstance = new EventInstance(status.GetHashCode(), 0, EventLogEntryType.Information);
            //var log = new EventLog(Constants.LogGroupName) { Source = Constants.IvsAgentName };
            EventLog.WriteEvent(eventInstance, status.ToString());

            //Send the status to the client
            var statuses = new List<ToolStatus> { status };

            var message = Newtonsoft.Json.JsonConvert.SerializeObject(statuses);
            _logger.Verbose($"Sending status to tray {string.Join(", ", statuses.Select(x => x))}");
            _serverPipe.WriteString(message);
        }

        #endregion

        #region ToolStatusCheck

        /// <summary>
        /// Check the status of the tool
        /// </summary>
        /// <param name="toolName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private ToolStatus GetToolStatus(string toolName)
        {
            switch (toolName)
            {
                case ToolName.AdvanceTelemetry:
                    return new ToolStatus(ToolName.AdvanceTelemetry, AteleServiceChecker.InstallStatus, AteleServiceChecker.RunningStatus);
                case ToolName.EndpointDeception:
                    return new ToolStatus(ToolName.EndpointDeception, EcdServiceChecker.InstallStatus, EcdServiceChecker.RunningStatus);
                case ToolName.EndpointDetectionAndResponse:
                    return new ToolStatus(ToolName.EndpointDetectionAndResponse, EdrServiceChecker.InstallStatus, EdrServiceChecker.RunningStatus);
                case ToolName.EndpointProtection:
                    return AvStatusWatcher.Instance.GetStatus();
                case ToolName.LateralMovementProtection:
                    return new ToolStatus(ToolName.LateralMovementProtection, LmpServiceChecker.InstallStatus, LmpServiceChecker.RunningStatus);
                case ToolName.UserBehaviorAnalytics:
                    return new ToolStatus(ToolName.UserBehaviorAnalytics, UbaServiceChecker.InstallStatus, UbaServiceChecker.RunningStatus);
                default:
                    throw new Exception($"Unknown tool name {toolName}");
            }
        }

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

        private void EcdUpdateStatus(ServiceControllerStatus? status)
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

        #region Tool Instalation For first time
        /// <summary>
        /// TODO: Need to move tool installation logic to separate class.
        /// </summary>
        private void VerifyDependencyAndInstall()
        {
            _logger.Information("Starting dependency check and installation");

            _logger.Information($"Checking {ToolName.AdvanceTelemetry}");
            if (SysmonWrapper.Verify(true) == 0)
            {
                _logger.Information($"{ToolName.AdvanceTelemetry} ServiceVerified with status {AteleServiceChecker.Status}");
                AteleServiceChecker.StartListening();
                SysmonUpdateStatus(AteleServiceChecker.Status);
                _logger.Information($"{ToolName.AdvanceTelemetry} ServiceListening with status {AteleServiceChecker.Status}");
            }
            else
            {
                _logger.Error($"Error in {ToolName.AdvanceTelemetry} installer");
            }

            _logger.Information($"Checking {ToolName.UserBehaviorAnalytics}");
            if (OsQueryWrapper.Verify(true) == 0)
            {
                _logger.Information($"{ToolName.UserBehaviorAnalytics} Verified with status {UbaServiceChecker.Status}");
                UbaServiceChecker.StartListening();
                OsQueryUpdateStatus(UbaServiceChecker.Status);
                _logger.Information($"{ToolName.UserBehaviorAnalytics} Listening with status {UbaServiceChecker.Status}");
            }
            else
            {
                _logger.Error($"Error in {ToolName.UserBehaviorAnalytics} installer");
            }

            _logger.Information($"Checking {ToolName.EndpointDetectionAndResponse}");

            var edrAgentInstallRequired = false;
            if (WazuhWrapper.GetInstalledVersion(out Version edrVersion))
            {
                _logger.Information($"{ToolName.EndpointDeception} Service {edrVersion} with status {EdrServiceChecker.Status}");

                if (edrVersion < new Version("4.4.1"))
                {
                    edrAgentInstallRequired = true;
                }
            }
            else
            {
                _logger.Information($"{ToolName.EndpointDetectionAndResponse} does not verified.");
                edrAgentInstallRequired = true;
            }

            if (edrAgentInstallRequired)
            {
                var installCode = WazuhWrapper.Install();
                _logger.Information($"{ToolName.EndpointDetectionAndResponse} install code {installCode}.");
            }

            WazuhWrapper.EdrServiceCheck();

            EdrServiceChecker.StartListening();
            EdrUpdateStatus(EdrServiceChecker.Status);
            _logger.Information($"{ToolName.EndpointDetectionAndResponse} with status {EdrServiceChecker.Status}");

            _logger.Information($"Checking {ToolName.EndpointDeception}");
            if (DBytesWrapper.Verify(true) == 0)
            {
                _logger.Information($"{ToolName.EndpointDeception} with status {EcdServiceChecker.Status}");
                EcdServiceChecker.StartListening();
                EcdUpdateStatus(EcdServiceChecker.Status);
                _logger.Information($"{ToolName.EndpointDeception} with status {EcdServiceChecker.Status}");
            }
            else
            {
                _logger.Information("Error in dBytes installer");
            }

            _logger.Information("Adding fake user");
            UserExtensions.EnsureFakeUser("maintenance", "P@$$w0rd");
        }

        #endregion
    }
}
