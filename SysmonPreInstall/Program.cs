using System.Diagnostics;
using System.ServiceProcess;

namespace SysmonPreInstall
{
    class Program
    {
        static void Main(string[] args) 
        {
            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "Sysmon64");
            if (ctl != null) 
            {
                // Get the path to the osquery executable
                string sysmonPath = @"C:\Windows\Sysmon64.exe";

                // Use FileVersionInfo to get the name and version of the executable
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(sysmonPath);

                // Print the name and version
                var productName = fileVersionInfo.ProductName;

                var productVersion = fileVersionInfo.ProductVersion;
                
                Console.WriteLine(fileVersionInfo.FileVersion);
            }
        }
    }
}