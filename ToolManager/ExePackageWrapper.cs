using Common.Extensions;
using Serilog;
using System;
using System.Diagnostics;

namespace ToolManager
{
    public static class ExePackageWrapper
    {
        private static readonly ILogger logger = Log.ForContext(typeof(ExePackageWrapper));

        public static bool GetProductVersion(string msiPath, out Version version)
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
                logger.Error($"Failed to fetch version from {msiPath}: {ex.Message}");
            }

            version = null;
            return false;
        }

        /// <summary>
        ///     Installs the program.
        /// </summary>
        /// <returns>True if the installation succeeded, otherwise false</returns>
        public static bool Install(string installerFile, params string[] args)
        {
            try
            {
                logger.Information("Beginning package installation");

                var arguments = string.Empty;

                foreach (var arg in args) arguments += $" {arg}";

                using (Process p = ProcessHelper.CreateHiddenProcess(installerFile, arguments))
                {
                    logger.Information("Starting process: {0}", p.StartInfo.FileName);

                    p.Start();

                    p.WaitForExit();

                    var installResultDescription = ((MsiExitCode)p.ExitCode).GetEnumDescription();

                    logger.Information("Package install result: ({0}) {1}", p.ExitCode, installResultDescription);

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
        public static bool Uninstall(string installerFile, params string[] args)
        {
            var uninstallResult = false;

            try
            {
                logger.Information("Beginning package uninstallation");

                var arguments = string.Empty;

                foreach (var arg in args) arguments += $" {arg}";

                using (Process p = ProcessHelper.CreateHiddenProcess(installerFile, arguments))
                {
                    logger.Information("Starting process '{0}'", p.StartInfo.FileName);
                    p.Start();
                    p.WaitForExit();

                    var uninstallResultDescription = ((MsiExitCode)p.ExitCode).GetEnumDescription();
                    logger.Information("Package uninstall result: ({0}) {1}", p.ExitCode, uninstallResultDescription);

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
    }
}