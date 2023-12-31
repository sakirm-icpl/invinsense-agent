using Common.Extensions;
using Serilog;
using System;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Threading;

namespace MsiWrapper
{
    /// <summary>
    ///     Encapsulates MSI package installation/uninstallation operations.
    ///     https://jonathancrozier.com/blog/how-to-install-msi-packages-using-msiexec-and-c-sharp
    /// </summary>
    public static class MsiPackageWrapper
    {
        #region Constants

        private const string WindowsInstallerProgramName = "msiexec";

        #endregion

        /// <summary>
        ///     Wait (up to a timeout) for the MSI installer service to become free.
        /// </summary>
        /// <returns>
        ///     Returns true for a successful wait, when the installer service has become free.
        ///     Returns false when waiting for the installer service has exceeded the timeout.
        /// </returns>
        public static bool IsMsiExecFree(TimeSpan maxWaitTime)
        {
            // The _MSIExecute mutex is used by the MSI installer service to serialize installations
            // and prevent multiple MSI based installations happening at the same time.
            // For more info: http://msdn.microsoft.com/en-us/library/aa372909(VS.85).aspx
            const string installerServiceMutexName = "Global\\_MSIExecute";
            Mutex MSIExecuteMutex = null;
            var isMsiExecFree = false;
            try
            {
                MSIExecuteMutex = Mutex.OpenExisting(installerServiceMutexName, MutexRights.Synchronize);
                isMsiExecFree = MSIExecuteMutex.WaitOne(maxWaitTime, false);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                // Mutex doesn't exist, do nothing
                isMsiExecFree = true;
            }
            catch (ObjectDisposedException)
            {
                // Mutex was disposed between opening it and attempting to wait on it, do nothing
                isMsiExecFree = true;
            }
            finally
            {
                if (MSIExecuteMutex != null && isMsiExecFree) MSIExecuteMutex.ReleaseMutex();
            }

            return isMsiExecFree;
        }

        #region Methods

        public static bool GetMsiVersion(string msiPath, out Version version)
        {
            Type type = Type.GetTypeFromProgID("WindowsInstaller.Installer");

            WindowsInstaller.Installer installer = (WindowsInstaller.Installer)

            Activator.CreateInstance(type);

            try
            {
                WindowsInstaller.Database db = installer.OpenDatabase(msiPath, 0);
                WindowsInstaller.View dv = db.OpenView("SELECT `Value` FROM `Property` WHERE `Property`='ProductVersion'");
                WindowsInstaller.Record record = null;
                dv.Execute(record);
                record = dv.Fetch();
                string str = record.get_StringData(1).ToString();
                version = new Version(str);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to fetch version from {msiPath}: {ex.Message}");
            }

            version = null;
            return false;
        }

        /// <summary>
        ///     Installs the program.
        /// </summary>
        /// <returns>True if the installation succeeded, otherwise false</returns>
        public static bool Install(string installerFile, string logFile, params string[] args)
        {
            try
            {
                Log.Information("Beginning MSI package installation");

                var arguments = $"/i \"{installerFile}\" /quiet";

                // /l = Logging enabled.
                //  * = Log everything.
                //  v = Verbose output.
                //  x = Extra debugging information.
                arguments += $" /l*vx \"{logFile}\"";

                foreach (var arg in args) arguments += $" {arg}";

                using (Process p = ProcessHelper.CreateHiddenProcess(WindowsInstallerProgramName, arguments))
                {
                    Log.Information("Starting process: {0}", p.StartInfo.FileName);
                    p.Start();
                    p.WaitForExit();

                    var installResultDescription = ((MsiExitCode)p.ExitCode).GetEnumDescription();
                    Log.Information("MSI package install result: ({0}) {1}", p.ExitCode, installResultDescription);

                    if (p.ExitCode != 0) throw new Exception(installResultDescription);
                }

                Log.Information("Installation completed");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred.");
                throw;
            }

            return true;
        }

        /// <summary>
        ///     Uninstalls the program.
        /// </summary>
        /// <returns>True if the uninstallation succeeded, otherwise false</returns>
        public static bool Uninstall(string packageName, params string[] args)
        {
            var uninstallResult = false;

            try
            {
                Log.Information("Beginning MSI package uninstallation");
                var identifier = MoWrapper.GetPackageIdentifier(packageName);

                if (string.IsNullOrEmpty(identifier)) return false;

                var arguments = $"/x \"{identifier}\" /quiet";

                // /l = Logging enabled
                //  * = Log everything
                //  v = Verbose output
                //  x = Extra debugging information
                //arguments += $" /l*vx \"{logFile}\"";

                foreach (var arg in args) arguments += $" {arg}";

                using (Process p = ProcessHelper.CreateHiddenProcess(WindowsInstallerProgramName, arguments))
                {
                    Log.Information("Starting process '{0}'", p.StartInfo.FileName);
                    p.Start();
                    p.WaitForExit();

                    var uninstallResultDescription = ((MsiExitCode)p.ExitCode).GetEnumDescription();
                    Log.Information("MSI package uninstall result: ({0}) {1}", p.ExitCode, uninstallResultDescription);

                    if (p.ExitCode != 0) throw new Exception(uninstallResultDescription);
                }

                Log.Information("Uninstallation completed");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred.");
                throw;
            }

            return uninstallResult;
        }

        #endregion
    }
}