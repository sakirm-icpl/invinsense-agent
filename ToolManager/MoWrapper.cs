using System;
using System.Collections.Generic;
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
                foreach (ManagementObject mo in mos.Get())
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

            foreach (ManagementObject mo in mos.Get())
            {
                // Will return Name, IdentifyingNumber and Version
                try
                {
                    if (mo["Name"].ToString().Contains(packageName))
                    {
                        return mo["Name"].ToString();
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
    }
}
