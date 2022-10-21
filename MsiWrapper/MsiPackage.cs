﻿using Serilog;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MsiWrapper
{
    /// <summary>
    /// Encapsulates MSI package installation/uninstallation operations.
    /// https://jonathancrozier.com/blog/how-to-install-msi-packages-using-msiexec-and-c-sharp
    /// </summary>
    public class MsiPackage
    {
        #region Constants

        private const string WindowsInstallerProgramName = "msiexec";

        #endregion

        #region Readonlys

        private readonly string _pathToInstallerFile;
        private readonly string _pathToInstallLogFile;
        private readonly string _pathToUninstallLogFile;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pathToInstallerFile">Path to the installer file</param>
        /// <param name="pathToInstallLogFile">Path to the install log file (optional)</param>
        /// <param name="pathToUninstallLogFile">Path to the uninstall log file (optional)</param>
        public MsiPackage(
            string pathToInstallerFile,
            string pathToInstallLogFile   = null,
            string pathToUninstallLogFile = null)
        {
            _pathToInstallerFile    = pathToInstallerFile;
            _pathToInstallLogFile   = pathToInstallLogFile;
            _pathToUninstallLogFile = pathToUninstallLogFile;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the product code of the MSI package.
        /// </summary>
        /// <returns>The product code as a string, or an empty string if the installer file does not exist</returns>
        private string GetProductCode()
        {
            Log.Information("Getting product code from MSI package: {0}", _pathToInstallerFile);
            
            try
            {
                if (File.Exists(_pathToInstallerFile))
                {
                    using (var db = new Database(_pathToInstallerFile, DatabaseOpenMode.ReadOnly))
                    {
                        // Note: The 'grave accents' instead of normal single quotes in the query string are essential.
                        string productCode = (string)db.ExecuteScalar("SELECT `Value` FROM `Property` WHERE `Property` = 'ProductCode'");

                        Log.Information("Retrieved product code '{0}' from MSI package: {1}", productCode, _pathToInstallerFile);

                        return productCode;
                    }
                }
                else
                {
                    Log.Warning("An MSI package does not exist at the specified location, returning an empty string");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred.");
                throw;
            }
        }

        /// <summary>
        /// Installs the program.
        /// </summary>
        /// <returns>True if the installation succeeded, otherwise false</returns>
        public bool Install()
        {
            try
            {
                Log.Information("Beginning MSI package installation");

                string arguments = $"/i \"{_pathToInstallerFile}\" /quiet";

                if (!string.IsNullOrEmpty(_pathToInstallLogFile))
                {
                    // Create the install log directory if it doesn't already exist.
                    string pathToInstallLogDirectory = Path.GetDirectoryName(_pathToInstallLogFile);

                    if (!Directory.Exists(pathToInstallLogDirectory))
                    {
                        Directory.CreateDirectory(pathToInstallLogDirectory);
                    }

                    // /l = Logging enabled.
                    //  * = Log everything.
                    //  v = Verbose output.
                    //  x = Extra debugging information.
                    arguments += $" /l*vx \"{_pathToInstallLogFile}\"";
                }

                using (Process p = ProcessHelper.CreateHiddenProcess(WindowsInstallerProgramName, arguments))
                {
                    Log.Information("Starting process: {0}", p.StartInfo.FileName);
                    p.Start();
                    p.WaitForExit();

                    string installResultDescription = ((MsiExitCode)p.ExitCode).GetEnumDescription();
                    Log.Information("MSI package install result: ({0}) {1}", p.ExitCode, installResultDescription);

                    if (p.ExitCode != 0) throw new InstallerException(installResultDescription);
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
        /// Uninstalls the program.
        /// </summary>
        /// <returns>True if the uninstallation succeeded, otherwise false</returns>
        public bool Uninstall()
        {
            bool uninstallResult = false;

            try
            {
                Log.Information("Beginning MSI package uninstallation");

                // We need to use the product code to uninstall the program.
                // Using the MSI package doesn't work, even though the documentation says that it should.
                string productCode = GetProductCode();
                string arguments   = $"/x {productCode} /quiet";

                if (!string.IsNullOrEmpty(_pathToUninstallLogFile))
                {
                    // Create the uninstall log directory if it doesn't already exist.
                    string pathToUninstallLogDirectory = Path.GetDirectoryName(_pathToUninstallLogFile);

                    if (!Directory.Exists(pathToUninstallLogDirectory))
                    {
                        Directory.CreateDirectory(pathToUninstallLogDirectory);
                    }

                    // /l = Logging enabled
                    //  * = Log everything
                    //  v = Verbose output
                    //  x = Extra debugging information
                    arguments += $" /l*vx \"{_pathToUninstallLogFile}\"";
                }

                using (Process p = ProcessHelper.CreateHiddenProcess(WindowsInstallerProgramName, arguments))
                {
                    Log.Information("Starting process '{0}'", p.StartInfo.FileName);
                    p.Start();
                    p.WaitForExit();

                    string uninstallResultDescription = ((MsiExitCode)p.ExitCode).GetEnumDescription();
                    Log.Information("MSI package uninstall result: ({0}) {1}", p.ExitCode, uninstallResultDescription);

                    if (p.ExitCode != 0) throw new InstallerException(uninstallResultDescription);
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

        /// <summary>
        /// Wait (up to a timeout) for the MSI installer service to become free.
        /// </summary>
        /// <returns>
        /// Returns true for a successful wait, when the installer service has become free.
        /// Returns false when waiting for the installer service has exceeded the timeout.
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
                MSIExecuteMutex = Mutex.OpenExisting(installerServiceMutexName, System.Security.AccessControl.MutexRights.Synchronize);
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
                if (MSIExecuteMutex != null && isMsiExecFree)
                {
                    MSIExecuteMutex.ReleaseMutex();
                }
            }

            return isMsiExecFree;
        }
    }
}