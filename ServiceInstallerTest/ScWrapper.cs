using System;
using System.Collections.Specialized;
using System.Configuration.Install;
using System.ServiceProcess;

namespace ServiceInstallerTest
{
    internal static class ScWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        public static void Start(string serviceName)
        {
            var service = new ServiceController(serviceName);
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(5));
            Console.WriteLine($"Service Status: {service.Status}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        public static void Stop(string serviceName)
        {
            var service = new ServiceController(serviceName);
            if(service.Status == ServiceControllerStatus.Running)
            {
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(5));
            }
            Console.WriteLine($"Service Status: {service.Status}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="command"></param>
        public static void SendCommand(string serviceName, int command)
        {
            var service = new ServiceController(serviceName);
         
            if(service.Status != ServiceControllerStatus.Running)
            {
                Console.WriteLine("Service is not running");
                return;
            }
            
            service.ExecuteCommand(command);
            System.Threading.Thread.Sleep(2000);
            Console.WriteLine($"Can Stop: {service.CanStop}");
            Console.WriteLine($"Service status: {service.Status}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        public static void Pause(string serviceName)
        {
            var service = new ServiceController(serviceName);
            service.Pause();
            service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(5));
            Console.WriteLine($"Service Status: {service.Status}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        public static void Resume(string serviceName)
        {
            var service = new ServiceController(serviceName);
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(5));
            Console.WriteLine($"Service Status: {service.Status}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public static void Install(string path, string name)
        {
            using (ServiceProcessInstaller installer = new ServiceProcessInstaller())
            {
                using (ServiceInstaller svc = new ServiceInstaller())
                {
                    installer.Account = ServiceAccount.LocalSystem;
                    installer.Username = null;
                    installer.Password = null;

                    svc.Context = new InstallContext($"install_{name}.log", new[] { string.Format("/assemblypath={0}", path), "logConsole=false" });
                    svc.DisplayName = name;
                    svc.Description = name;
                    svc.ServiceName = name;
                    svc.StartType = ServiceStartMode.Automatic;
                    svc.Parent = installer;
                    ListDictionary state = new ListDictionary();
                    svc.Install(state);
                    state.Clear();
                }
            }
        }

        /// <summary>
        /// SC STOP SimpleService
        /// sc delete SimpleService
        /// C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe /u "path"
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public static void Uninstall(string path, string name)
        {
            ManagedInstallerClass.InstallHelper(new string[] { "/u", "C:\\Code\\invinsense-agent\\SampleService\\bin\\Debug\\SampleService.exe" });

            using (ServiceProcessInstaller installer = new ServiceProcessInstaller())
            {
                using (ServiceInstaller svc = new ServiceInstaller())
                {
                    svc.Context = new InstallContext($"uninstall_{name}.log", new[] { string.Format("/assemblypath={0}", path), "logConsole=false" });
                    svc.ServiceName = name;
                    svc.Parent = installer;
                    svc.Uninstall(null);
                }
            }
        }
    }
}
