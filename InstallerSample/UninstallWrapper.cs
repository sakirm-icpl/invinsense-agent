using System;
using System.Diagnostics;
using System.Management;

namespace InstallerSample
{
    internal class UninstallWrapper
    {
        public static bool UninstallProgram(string ProgramName)
        {
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product WHERE Name = '" + ProgramName + "'");

                foreach (ManagementObject mo in mos.Get())
                {
                    try
                    {
                        if (mo["Name"].ToString() == ProgramName)
                        {
                            object hr = mo.InvokeMethod("Uninstall", null);

                            Console.WriteLine($"Uninstall invoke return code: {hr}");

                            return (bool)hr;
                        }
                    }
                    catch
                    {
                        //this program may not have a name property, so an exception will be thrown
                    }
                }

                //was not found...
                return false;

            }
            catch
            {
                return false;
            }
        }

        public static void UnInstallPackage(string packageName)
        {
            string searchString = $"SELECT * FROM Win32_Product WHERE Name LIKE '{packageName}'";

            ManagementObjectSearcher mos = new ManagementObjectSearcher(searchString);

            foreach (ManagementObject mo in mos.Get())
            {
                // Will return Name, IdentifyingNumber and Version
                try
                {
                    Console.WriteLine("Properties");
                    foreach (var item in mo.Properties)
                    {
                        try
                        {
                            Console.WriteLine(item.Name + "-" + item.Value.ToString());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error in property: {ex.Message}");
                        }
                    }

                    Console.WriteLine("System Properties");
                    foreach (var item in mo.SystemProperties)
                    {
                        try
                        {
                            Console.WriteLine(item.Name + "-" + item.Value.ToString());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error in property: {ex.Message}");
                        }
                    }

                    if (mo["Name"].ToString().Contains(packageName))
                    {
                        //object arp = mo.InvokeMethod("Uninstall", null);
                        Process process = new Process();
                        process.StartInfo.FileName = "msiexec.exe";
                        process.StartInfo.Arguments = "/X" + mo["IdentifyingNumber"].ToString() + " /q";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardInput = true;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.CreateNoWindow = false;
                        process.Start();
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    //this program may not have a name property, so an exception will be thrown
                    //handle your exception here
                }
            }
        }
    }
}
