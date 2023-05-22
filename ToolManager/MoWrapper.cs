using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace ToolManager
{
    public class MoWrapper
    {
        public static List<string> ListPrograms()
        {
            List<string> programs = new List<string>();

            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
                IEnumerable<ManagementObject> collection = mos.Get().Cast<ManagementObject>();
                foreach (ManagementObject mo in collection)
                {
                    try
                    {
                        //more properties:
                        //http://msdn.microsoft.com/en-us/library/windows/desktop/aa394378(v=vs.85).aspx
                        programs.Add(mo["Name"].ToString());

                    }
                    catch
                    {
                        //this program may not have a name property
                    }
                }

                return programs;

            }
            catch
            {
                return programs;
            }
        }

        public static string GetPackageIdentifier(string packageName)
        {
            string searchString = $"SELECT * FROM Win32_Product WHERE Name LIKE '{packageName}'";

            ManagementObjectSearcher mos = new ManagementObjectSearcher(searchString);

            foreach (ManagementObject mo in mos.Get().Cast<ManagementObject>())
            {
                // Will return Name, IdentifyingNumber and Version
                try
                {
                    if (mo["Name"].ToString().Contains(packageName))
                    {
                        return mo["IdentifyingNumber"].ToString();
                    }
                }
                catch
                {
                    //this program may not have a name property, so an exception will be thrown
                    //handle your exception here
                }
            }

            return null;
        }

        public static bool UninstallProgram(string programName)
        {
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product WHERE Name = '" + programName + "'");

                foreach (ManagementObject mo in mos.Get().Cast<ManagementObject>())
                {
                    try
                    {
                        if (mo["Name"].ToString() == programName)
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
    }
}
