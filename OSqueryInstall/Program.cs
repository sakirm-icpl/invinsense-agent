using Common.Utils;
using Serilog;
using System.Diagnostics;
using System.IO.Compression;
using System.ServiceProcess;
namespace Osquery
{
    public class OSquery
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(OSquery));
        public static void Main(string[] args) 
        { 
            
            try
            { 
                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "osqueryd");
                if(ctl != null) 
                {
                    _logger.Information($"OsQueryStatus:{ctl.Status}");
                }
                var msiPath = "D:\\invinsense-agent\\artifacts\\osquery\\osquery-5.5.1.msi";

                var logPath = CommonUtils.DataFolder + "\\osqueryInstall.log";

                Process installerProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "msiexec",
                        Arguments = $"/I \"{msiPath}\" /QN /l*vx \"{logPath}\" ACCEPTEULA=1 ALLUSERS=1",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        WorkingDirectory = CommonUtils.RootFolder
                    }
                };
                installerProcess.OutputDataReceived += InstallerProcess_OutputDataReceived;
                installerProcess.ErrorDataReceived += InstallerProcess_ErrorDataReceived;
                installerProcess.Exited += InstallerProcess_Exited;
                

                installerProcess.Start();

                _logger.Information("OSQUERY Installation started...");

                installerProcess.WaitForExit(100000);

                if (installerProcess.ExitCode == 0)
                {
                    _logger.Information("OSQUERY installation completed");

                    _logger.Information("Copying osquery.conf file to osquery installed directory");

                    File.Copy(CommonUtils.GetAbsoletePath("..\\artifacts\\osquery\\osquery.conf"), "C:\\Program Files\\osquery\\osquery.conf", true);

                    Console.WriteLine("OsQueryStatus", new { OsQueryStatus = "Extract packs to osquery" });

                    ZipFile.ExtractToDirectory(CommonUtils.GetAbsoletePath("..\\artifacts\\osquery\\osquery-packs.zip"), "C:\\Program Files\\osquery\\packs");
                    
                    Thread.Sleep(50000);

                }
                else
                {
                    _logger.Information($"OSQUERY installation fault: {installerProcess.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");
            }
        }
        private static void InstallerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Information($"OSQUERY installation error data: {e.Data}");
        }

        private static void InstallerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.Information($"OSQUERY error data: {e.Data}");
        }

        private static void InstallerProcess_Exited(object sender, EventArgs e)
        {
            _logger.Information("OSQUERY process exited.");
        }
    }
}
