using Serilog;
using System;
using System.Diagnostics;
using Common.Models;

namespace ToolManager
{
    public static class ExePackageWrapper
    {
        private static readonly ILogger logger = Log.ForContext(typeof(ExePackageWrapper));

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

                using (Process p = ProcessExtensions.CreateHiddenProcess(installerFile, arguments))
                {
                    logger.Information("Starting process: {0}", p.StartInfo.FileName);
                    p.Start();
                    p.WaitForExit();
                    logger.Information("Package install result: {0}", p.ExitCode);
                    return p.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An exception occurred.");
                return false;
            }
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

                using (Process p = ProcessExtensions.CreateHiddenProcess(installerFile, arguments))
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