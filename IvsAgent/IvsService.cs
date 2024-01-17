using Serilog;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Common.Ipc.Np.Server;
using Common.Models;
using Common;
using Common.ServiceHelpers;
using Common.AvHelper;
using ToolManager;
using Common.RegistryHelpers;

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
        private ServerPipe _serverPipe;

        private readonly ILogger _logger = Log.ForContext<IvsService>();

        private readonly Dictionary<string, ToolGroup> servicesToMonitor = new Dictionary<string, ToolGroup>
        {
            { "WazuhSvc", ToolGroup.EndpointDetectionAndResponse },
            { "osqueryd", ToolGroup.UserBehaviorAnalytics },
            { "Sysmon64", ToolGroup.AdvanceTelemetry },
            { "IvsAgent", ToolGroup.LateralMovementProtection }
        };

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

            ServiceStatusWatcher.ServiceStatusChanged += SendStatusUpdate;

            AvStatusWatcher.Instance.AvStatusChanged += AvStatusChanged;
        }

        internal void TestStartupAndStop(string[] args)
        {
            OnStart(args);
 			Console.ReadLine();
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            _logger.Verbose("IvsService.OnStart");

            _logger.Information("Starting IPC server");
            CreateServerPipe();

            //TODO: Need to move tool installation logic to separate class.
            Task.Factory.StartNew(async () =>
            {
                _logger.Information("Start waiting for tool verification");
                await Task.Delay(5000);

                var apiUrl = WinRegistryHelper.GetPropertyByName(Constants.CompanyName, "ApiUrl");

                if(string.IsNullOrEmpty(apiUrl)) 
                {
                    _logger.Error("ApiUrl is not set in registry");
                    return;
                }

                await CheckRequiredTools.Install(apiUrl);

                foreach (var service in servicesToMonitor)
                {
                    ServiceStatusWatcher.AddService(service.Key);
                }

                _logger.Information("Start watching IvsTray App");
                ServiceStatusWatcher.ResumeMonitoring();

                _logger.Information("Start watching windows defender events");
                AvStatusWatcher.Instance.StartMonitoring();

                _logger.Information("Start watching IvsTray App");
                UserSessionAppMonitor.Instance.StartMonitoring();
            });
        }

        protected override void OnStop()
        {
            _logger.Information("Stopping service");

            try
            {
                _logger.Information("Stopping IvsTray watcher");
                ServiceStatusWatcher.StopMonitoring();

                _logger.Information("Stopping windows defender watcher");
                AvStatusWatcher.Instance.StopMonitoring();

                _logger.Information("Stopping IvsTray App");
                UserSessionAppMonitor.Instance.StopMonitoring();

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
            _serverPipe = new ServerPipe(Constants.BrandName, p => p.StartStringReaderAsync());

            // Data received from client
            _serverPipe.DataReceived += (sender, args) =>
            {
                _logger.Verbose($"Message received: {args.String}");
            };

            // Client connected
            _serverPipe.Connected += (sender, args) =>
            {
                _logger.Debug("Client is connected.");
                SendToolStatuses();
            };

            _serverPipe.PipeClosed += (sender, args) =>
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
            var trayStatus = new TrayStatus();

            //Get service status and convert into ToolStatus and prepare list of ToolStatuses.
            foreach (var service in servicesToMonitor)
            {
                RunningStatus status = RunningStatus.Unknown;

                if(!ServiceHelper.GetServiceInfo(service.Key, out var detail))
                {
                    _logger.Error($"Error in getting details for service: {service.Key}");
                }
                else if(detail.InstallStatus != InstallStatus.Installed)
                {
                    _logger.Error($"Service {service.Key} is not installed");
                }
                else
                {
                    status = ServiceStatusMapper.Map(ServiceHelper.GetServiceStatus(service.Key));
                }

                trayStatus.ToolStatuses.Add(new ToolStatus(service.Value.Id, detail.InstallStatus, status));
            }

            var message = Newtonsoft.Json.JsonConvert.SerializeObject(trayStatus);
            _logger.Verbose($"Sending status to tray. ErrorCode:{trayStatus.ErrorCode}, Message:{trayStatus.ErrorMessage}, {string.Join(", ", trayStatus.ToolStatuses.Select(x => x))}");
            _serverPipe.WriteString(message);
        }

        private void SendStatusUpdate(string serviceName, ServiceControllerStatus status)
        {
            //Capture the event in the event logger
            var eventInstance = new EventInstance(status.GetHashCode(), 0, EventLogEntryType.Information);
            //var log = new EventLog(Constants.LogGroupName) { Source = Constants.IvsAgentName };
            EventLog.WriteEvent(eventInstance, status.ToString());

            var trayStatus = new TrayStatus();

            //Send the status to the client
            trayStatus.ToolStatuses.Add(ServiceStatusMapper.Map(servicesToMonitor[serviceName], status));

            var message = Newtonsoft.Json.JsonConvert.SerializeObject(trayStatus);
            _logger.Verbose($"Sending status to tray {string.Join(", ", trayStatus.ToolStatuses.Select(x => x))}");
            _serverPipe.WriteString(message);
        }

        private void AvStatusChanged(object sender, ToolStatus e)
        {
            //Send the status to the client
            var statuses = new List<ToolStatus> { e };

            var message = Newtonsoft.Json.JsonConvert.SerializeObject(statuses);
            _logger.Verbose($"Sending status to tray {string.Join(", ", statuses.Select(x => x))}");
            _serverPipe.WriteString(message);
        }

        #endregion
    }
}
