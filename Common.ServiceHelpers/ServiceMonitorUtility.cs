using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Serilog;

namespace Common.ServiceHelpers
{
    public static class ServiceMonitorUtility
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(ServiceMonitorUtility));

        private static Dictionary<string, ServiceController> _monitoredServices = new Dictionary<string, ServiceController>();
        private static Dictionary<string, Task> _monitoringTasks = new Dictionary<string, Task>();
        private static Dictionary<string, ServiceControllerStatus> _serviceStatuses = new Dictionary<string, ServiceControllerStatus>();

        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public static event Action<string, ServiceControllerStatus> ServiceStatusChanged;

        public static void AddService(string serviceName)
        {
            if (!ServiceHelper.GetServiceInfo(serviceName, out var detail))
            {
                _logger.Error(serviceName + " not found");    
            }

            _logger.Information($"Service {serviceName} found with {detail.Version}.");

            if (!_monitoredServices.ContainsKey(serviceName))
            {
                var service = new ServiceController(serviceName);
                _monitoredServices.Add(serviceName, service);
                _serviceStatuses.Add(serviceName, service.Status);
                _monitoringTasks.Add(serviceName, MonitorServiceAsync(service, _cancellationTokenSource.Token));
            }
            else
            {
                _logger.Error($"Service {serviceName} already added.");
            }
        }

        private static Task MonitorServiceAsync(ServiceController service, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var lastStatus = _serviceStatuses[service.ServiceName]; 
                            
                            ServiceControllerStatus currentStatus = service.Status;

                            if (lastStatus != currentStatus)
                            {
                                _serviceStatuses[service.ServiceName] = currentStatus;
                                ServiceStatusChanged?.Invoke(service.ServiceName, currentStatus);
                            }
                            
                            ServiceControllerStatus targetStatus = currentStatus == ServiceControllerStatus.Running ? ServiceControllerStatus.Stopped : ServiceControllerStatus.Running;
                            service.WaitForStatus(targetStatus, TimeSpan.FromSeconds(30)); // Set an appropriate timeout
                            _serviceStatuses[service.ServiceName] = service.Status;

                            if (!cancellationToken.IsCancellationRequested)
                            {
                                ServiceStatusChanged?.Invoke(service.ServiceName, service.Status);
                            }
                        }
                        catch (System.ServiceProcess.TimeoutException)
                        {
                            _logger.Verbose($"Monitoring the service: {service.ServiceName} operation timed out.");
                        }
                        catch (InvalidOperationException)
                        {
                            _logger.Error("Service not found");
                        }

                        service.Refresh();
                    }
                }
                catch
                {
                    _logger.Error($"Error while monitoring service {service.ServiceName}");
                }
            });
        }

        public static void StopMonitoring()
        {
            _cancellationTokenSource.Cancel();
            foreach (var task in _monitoringTasks.Values)
            {
                task.Wait(); // Ensuring all tasks are completed before exiting
            }
        }

        public static void ResumeMonitoring()
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();

                foreach (var service in _monitoredServices.Values)
                {
                    var serviceName = service.ServiceName;
                    if (!_monitoringTasks.ContainsKey(serviceName))
                    {
                        _monitoringTasks[serviceName] = MonitorServiceAsync(service, _cancellationTokenSource.Token);
                    }
                }
            }
        }
    }
}
