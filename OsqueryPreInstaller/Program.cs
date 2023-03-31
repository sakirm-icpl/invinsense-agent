using System.Diagnostics;
using System.ServiceProcess;
using Common.Utils;

namespace OsQueryPreInstaller
{
    class Program
    {
        public static void Main(string[] args) 
        {
            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "osqueryd");
            if (ctl != null) 
            {
                // Get the path to the osquery executable
                string osqueryPath = @"C:\Program Files\osquery\osqueryi.exe";

                // Use FileVersionInfo to get the name and version of the executable
                FileVersionInfo osqueryInfo = FileVersionInfo.GetVersionInfo(osqueryPath);

                CommonUtils.GetAbsoletePath("..\\artifacts\\osquery.conf");
                // Print the name and version

                var productName =osqueryInfo.ProductName;

                var productVersion = osqueryInfo.ProductVersion;
                Console.WriteLine("Version: " + osqueryInfo.ProductVersion);
            }

        }
    }
}
