using System;
using System.Diagnostics;
using System.Management;

namespace InstallerSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UnInstallPackage("Setup1");
        }

        static void UnInstallPackage(string packageName)
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
