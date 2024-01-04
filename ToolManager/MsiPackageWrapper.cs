using Common.Extensions;
using Common.FileHelpers;
using Microsoft.Win32;
using Serilog;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using ToolManager.Models;

namespace ToolManager
{
    /// <summary>
    ///     Encapsulates MSI package installation/uninstallation operations.
    ///     https://jonathancrozier.com/blog/how-to-install-msi-packages-using-msiexec-and-c-sharp
    /// </summary>
    public static class MsiPackageWrapper
    {
        #region Constants

        private static readonly ILogger logger = Log.ForContext(typeof(MsiPackageWrapper));

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

            WindowsInstaller.Installer installer = (WindowsInstaller.Installer) Activator.CreateInstance(type);

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
                logger.Error($"Failed to fetch version from {msiPath}: {ex.Message}");
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
                if (!IsMsiExecFree(TimeSpan.FromMinutes(5)))
                {
                    logger.Error("MSI installer is not ready. 1618");
                    return false;
                }

                logger.Information("Beginning MSI package installation");

                var arguments = $"/i \"{installerFile}\" /quiet";

                // /l = Logging enabled.
                //  * = Log everything.
                //  v = Verbose output.
                //  x = Extra debugging information.
                arguments += $" /l*vx \"{logFile}\"";

                foreach (var arg in args) arguments += $" {arg}";

                using (Process p = ProcessHelper.CreateHiddenProcess(WindowsInstallerProgramName, arguments))
                {
                    logger.Information("Starting process: {0}", p.StartInfo.FileName);
                    
                    p.Start();

                    p.WaitForExit();

                    var installResultDescription = ((MsiExitCode)p.ExitCode).GetEnumDescription();
                    
                    logger.Information("MSI package install result: ({0}) {1}", p.ExitCode, installResultDescription);

                    if (p.ExitCode != 0) throw new Exception(installResultDescription);
                }

                logger.Information("Installation completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An exception occurred.");
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
                if (!IsMsiExecFree(TimeSpan.FromMinutes(5)))
                {
                    logger.Error("MSI installer is not ready.");
                    return false;
                }

                logger.Information("Beginning MSI package uninstallation");
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
                    logger.Information("Starting process '{0}'", p.StartInfo.FileName);
                    p.Start();
                    p.WaitForExit();

                    var uninstallResultDescription = ((MsiExitCode)p.ExitCode).GetEnumDescription();
                    logger.Information("MSI package uninstall result: ({0}) {1}", p.ExitCode, uninstallResultDescription);

                    if (p.ExitCode != 0) throw new Exception(uninstallResultDescription);
                }

                logger.Information("Uninstallation completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An exception occurred.");
                throw;
            }

            return uninstallResult;
        }

        #endregion

        public static bool GetProductInfoReg(string productName, out InstallStatusWithDetail productInfo)
        {
            if (GetProductInfoRegByKey(productName, Architecture.X64, out productInfo)) return true;
            if (GetProductInfoRegByKey(productName, Architecture.X86, out productInfo)) return true;
            
            productInfo = new InstallStatusWithDetail
            {
                InstallStatus = InstallStatus.NotFound
            };
            
            return true;
        }

        private static bool GetProductInfoRegByKey(string productName, Architecture architecture, out InstallStatusWithDetail productInfo)
        {
            productInfo = new InstallStatusWithDetail
            {
                InstallStatus = InstallStatus.NotFound
            };

            var found = false;

            var regViewType = architecture == Architecture.X64 ? RegistryView.Registry64 : RegistryView.Registry32;

            using (var rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, regViewType))
            {
                var uninstallKey = rk.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");

                foreach (var skName in uninstallKey.GetSubKeyNames())
                    using (var subkey = uninstallKey.OpenSubKey(skName))
                    {
                        try
                        {
                            var displayName = (string)subkey.GetValue("DisplayName");

                            if (displayName == null || !displayName.Contains(productName)) continue;

                            found = true;
                            productInfo.Name = displayName;
                            productInfo.InstallStatus = InstallStatus.Installed;
                            productInfo.Version = new Version(subkey.GetValue("DisplayVersion").ToString());
                            productInfo.InstalledDate = ConvertToDateTime((string)subkey.GetValue("InstallDate"));
                            productInfo.InstallPath = (string)subkey.GetValue("InstallLocation");
                            productInfo.FileDate = CommonFileHelpers.GetFileDate(productInfo.InstallPath);
                            productInfo.Architecture = architecture;
                            break;
                        }
                        catch (Exception ex)
                        {
                            // If there is an error, continue with the next subkey
                            logger.Error(ex.Message);
                        }
                    }
            }

            logger.Information($"{productName} Found: {found}, {productInfo}");

            return found;
        }

        // The install date string format is YYYYMMDD
        private static DateTime? ConvertToDateTime(string installDateStr)
        {
            if (DateTime.TryParseExact(installDateStr, "yyyyMMdd", null, DateTimeStyles.None, out var date)) return date;
            return null;
        }
    }
}