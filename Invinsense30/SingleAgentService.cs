using Invinsense30.Monitor;
using Serilog;
using SingleAgent.Monitor;
using System;
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

            Dbytes = new ExtendedServiceController("DBytesService");
            Dbytes.StatusChanged += (object sender, ServiceStatusEventArgs e) => UpdateStatus("Deceptive Bytes", e.Status);

            wazuh = new ExtendedServiceController("WazuhSvc");
            wazuh.StatusChanged += (object sender, ServiceStatusEventArgs e) => UpdateStatus("Wazuh", e.Status);

            Sysmon = new ExtendedServiceController("Sysmon64");
            Sysmon.StatusChanged += (object sender, ServiceStatusEventArgs e) => UpdateStatus("Microsoft Sysmon", e.Status);

            Dejavu = new ExtendedServiceController("Spooler");
            Dejavu.StatusChanged += (object sender, ServiceStatusEventArgs e) => UpdateStatus("Lateral Movement Protection", e.Status);

            UpdateStatus("Wazuh", wazuh.Status);
            UpdateStatus("Deceptive Bytes", Dbytes.Status);
            UpdateStatus("Microsoft Sysmon", Sysmon.Status);
            UpdateStatus("Dejavu", Dejavu.Status);
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds  
            timer.Enabled = true;

            UpdateStatus("Invinsense3.0", ServiceControllerStatus.Running);
        }

        protected override void OnStop()
        {
            UpdateStatus("Invinsense3.0", ServiceControllerStatus.Stopped);
            Log.Information("Service is stopped");
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            Log.Information("Service timer");
        }

        private void UpdateStatus(string name, ServiceControllerStatus? status)
        {
            string message;

            if (status == null)
            {
                _logger.Error($"{name} Status Changed: {status}");
            }
            else if (status == ServiceControllerStatus.Running)
            {
                _logger.Error($"{name} Status Changed: {status}");
            }
            else if (status == ServiceControllerStatus.Stopped)
            {
                _logger.Error($"{name} Status Changed: {status}");
            }
            else
            {
                _logger.Error($"{name} Status Changed: {status}");
            }
        }

        private string avLastStatus = "";
        private void AvCheckTick(object sender, EventArgs e)
        {
            var status = ServiceHelper.AVStatus("Windows Defender");

            if (avLastStatus == status)
            {
                return;
            }

            avLastStatus = status;

            switch (status)
            {
                case "Enabled":
                    
                    break;
                case "Need Update":
                    
                    break;
                case "Disabled":
                    
                    break;
                default:
                    //Not found        
                    break;
            }
        }
    }
}
