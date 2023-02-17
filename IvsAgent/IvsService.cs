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

namespace IvsAgent
{
    public partial class IvsService : ServiceBase
    {
        private readonly Timer avTimer = new Timer(15000);

        private readonly ExtendedServiceController EdrServiceChecker;
        private readonly ExtendedServiceController DeceptionServiceChecker;
        private readonly ExtendedServiceController OsQuery;
        private readonly ExtendedServiceController Sysmon;
        private readonly ExtendedServiceController LmpService;

        private readonly ILogger _logger = Log.ForContext<IvsService>();
      
        AgentServiceJasonData agentServiceJasonData=new AgentServiceJasonData();

        private readonly ToolRepository toolRepository;

        private bool _isRunning = false;

        public IvsService()
        {
            InitializeComponent();

            AutoLog = false;
            
            //Allow service to handle shutdown
            CanShutdown = true;

            //Stop service to stop
            CanStop = false;

            //Stop service to pause and continue
            CanPauseAndContinue = false;
            
            CanHandleSessionChangeEvent = true;

            toolRepository = new ToolRepository();

            EdrServiceChecker = new ExtendedServiceController("WazuhSvc");
            EdrServiceChecker.StatusChanged += (object sender, ServiceStatusEventArgs e) => EdrUpdateStatus(e.Status);

            DeceptionServiceChecker = new ExtendedServiceController("DBytesService");
            DeceptionServiceChecker.StatusChanged += (object sender, ServiceStatusEventArgs e) => DeceptionUpdateStatus(e.Status);

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

        private void EdrUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                toolRepository.CaptureEvent(new ToolStatus(ToolName.EndpointDecetionAndResponse, InstallStatus.NotFound, RunningStatus.NotFound));
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running:
                    toolRepository.CaptureEvent(new ToolStatus(ToolName.EndpointDecetionAndResponse, InstallStatus.Installed, RunningStatus.Running));
                    return;
                case ServiceControllerStatus.Stopped:
                    toolRepository.CaptureEvent(new ToolStatus(ToolName.EndpointDecetionAndResponse, InstallStatus.Installed, RunningStatus.Stopped));
                    return;
                default:
                    toolRepository.CaptureEvent(new ToolStatus(ToolName.EndpointDecetionAndResponse, InstallStatus.Installed, RunningStatus.Warning));
                    return;
            }
        }

        private void DeceptionUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                toolRepository.CaptureEvent(new ToolStatus(ToolName.EndpointDeception, InstallStatus.NotFound, RunningStatus.NotFound));
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running:
                    toolRepository.CaptureEvent(new ToolStatus(ToolName.EndpointDeception, InstallStatus.Installed, RunningStatus.Running));
                    return;
                case ServiceControllerStatus.Stopped:
                    toolRepository.CaptureEvent(new ToolStatus(ToolName.EndpointDeception, InstallStatus.Installed, RunningStatus.Stopped));
                    return;
                default:
                    toolRepository.CaptureEvent(new ToolStatus(ToolName.EndpointDeception, InstallStatus.Installed, RunningStatus.Warning));
                    return;
            }
        }

        private void OsQueryUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                toolRepository.CaptureEvent(new ToolStatus(ToolName.UserBehaviorAnalytics, InstallStatus.NotFound, RunningStatus.NotFound));
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running:
                    toolRepository.CaptureEvent(new ToolStatus(ToolName.UserBehaviorAnalytics, InstallStatus.Installed, RunningStatus.Running));
                    return;
                case ServiceControllerStatus.Stopped:
                    toolRepository.CaptureEvent(new ToolStatus(ToolName.UserBehaviorAnalytics, InstallStatus.Installed, RunningStatus.Stopped));
                    return;
                default:
                    toolRepository.CaptureEvent(new ToolStatus(ToolName.UserBehaviorAnalytics, InstallStatus.Installed, RunningStatus.Warning));
                    return;
            }
        }

        private void SysmonUpdateStatus(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                toolRepository.CaptureEvent(new ToolStatus(ToolName.AdvanceTelemetry, InstallStatus.NotFound, RunningStatus.NotFound));
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running:
                    toolRepository.CaptureEvent(new ToolStatus(ToolName.AdvanceTelemetry, InstallStatus.Installed, RunningStatus.Running));
                    return;
                case ServiceControllerStatus.Stopped:
                    toolRepository.CaptureEvent(new ToolStatus(ToolName.AdvanceTelemetry, InstallStatus.Installed, RunningStatus.Stopped));
                    return;
                default:
                    toolRepository.CaptureEvent(new ToolStatus(ToolName.AdvanceTelemetry, InstallStatus.Installed, RunningStatus.Warning));
                    return;
            }
        }

        private void LmpStatusUpdate(ServiceControllerStatus? status)
        {
            if (status == null)
            {
                toolRepository.CaptureEvent(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.NotFound, RunningStatus.NotFound));
                return;
            }

            switch (status.Value)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running:
                    toolRepository.CaptureEvent(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.Installed, RunningStatus.Running));
                    return;
                case ServiceControllerStatus.Stopped:
                    toolRepository.CaptureEvent(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.Installed, RunningStatus.Stopped));
                    return;
                default:
                    toolRepository.CaptureEvent(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.Installed, RunningStatus.Warning));
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

            _logger.Information("Starting Invinsense service..." );

            avTimer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            avTimer.Start();

            _logger.Information("Scheduling dependency after 5 sec...");

            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(5000);
                VerifyDependencyAndInstall();
            });
            
            _logger.Information("Task added...");
            toolRepository.CaptureEvent(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.Installed, RunningStatus.Running));
        }

        protected override void OnStop()
        {
            _logger.Information("Stopping service");
            avTimer.Stop();

            toolRepository.CaptureEvent(new ToolStatus(ToolName.LateralMovementProtection, InstallStatus.Installed, RunningStatus.Stopped));

            _isRunning = false;

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
            _logger.Information($"CustomCommand {command }");
            base.OnCustomCommand(command);

            if(command == 130)
            {
                Stop();
            }
            else if(command==140)
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
            else if(command==141)
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
            ServiceController service = new ServiceController("Invinsense 3.0");
            service.Stop();
            System.Environment.Exit(0);
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
            bool isSessionActive = Process.GetProcesses().Any(p => p.SessionId> 0 && p.ProcessName !="Idel");
            if (isSessionActive) 
            {
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
            }
            var avStatuses = AvMonitor.ListAvStatuses();

            var activeAvStatus = avStatuses.FirstOrDefault(x => x.IsAvEnabled);

            InstallStatus currentAvStatus;
            
            _logger.Information($"AvName:{activeAvStatus.AvName}");
            if (activeAvStatus == null)
            {
                currentAvStatus = InstallStatus.Error;
            }
            else
            {
                currentAvStatus = activeAvStatus.IsAvUptoDate ? InstallStatus.Installed : InstallStatus.Outdated;
            }

            if (avLastStatus != currentAvStatus)
            {
                _logger.Information($"AvDescription{activeAvStatus.AvName} service status : Last: {avLastStatus}, New: {currentAvStatus}");
                avLastStatus = currentAvStatus;

                toolRepository.CaptureEvent(new ToolStatus(ToolName.EndpointProtection, currentAvStatus, currentAvStatus == InstallStatus.Installed ? RunningStatus.Running : RunningStatus.Warning));
            }

            inTimer = false;
        }
        
        private void VerifyDependencyAndInstall()
        {
            if (SysmonWrapper.Verify(true) == 0)
            {
                _logger.Information($"SysmonServiceVerified with status {Sysmon.Status}");
                Sysmon.StartListening();
                SysmonUpdateStatus(Sysmon.Status);
                _logger.Information($"SysmonServiceListening with status {Sysmon.Status}");
            }
            else
            {
                _logger.Error("Error in Sysmon installer");
            }

            if (OsQueryWrapper.Verify(true) == 0)
            {
                _logger.Information("OsQueryVerified with status {OsQuery.Status}");
                OsQuery.StartListening();
                OsQueryUpdateStatus(OsQuery.Status);
                _logger.Information($"OsQueryListening with status {OsQuery.Status}");
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
                _logger.Information("Error in dBytes installer" );
            }            
            _logger.Information("Adding fake user");
            UserExtensions.EnsureFakeUser("maintenance", "P@$$w0rd");
        }
    }
}
