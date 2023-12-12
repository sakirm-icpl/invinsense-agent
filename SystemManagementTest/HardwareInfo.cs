using System;
using System.Linq;
using System.Management;
using System.Net;

namespace SystemManagementTest
{
    public static class HardwareInfo
    {
        /// <summary>
        ///     Retrieving Processor Id.
        /// </summary>
        /// <returns></returns>
        public static string GetProcessorId()
        {
            var mc = new ManagementClass("win32_processor");
            var moc = mc.GetInstances();
            var Id = string.Empty;
            foreach (var mo in moc.Cast<ManagementObject>())
            {
                Id = mo.Properties["processorID"].Value.ToString();
                break;
            }

            return Id;
        }

        /// <summary>
        ///     Retrieving HDD Serial No.
        /// </summary>
        /// <returns></returns>
        public static string GetHDDSerialNo()
        {
            var mangnmt = new ManagementClass("Win32_LogicalDisk");
            var mcol = mangnmt.GetInstances();
            var result = "";
            foreach (var strt in mcol.Cast<ManagementObject>()) result += Convert.ToString(strt["VolumeSerialNumber"]);
            return result;
        }

        /// <summary>
        ///     Retrieving System MAC Address.
        /// </summary>
        /// <returns></returns>
        public static string GetMACAddress()
        {
            var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var moc = mc.GetInstances();
            var MACAddress = string.Empty;
            foreach (var mo in moc.Cast<ManagementObject>())
            {
                if (MACAddress == string.Empty)
                    if ((bool)mo["IPEnabled"])
                        MACAddress = mo["MacAddress"].ToString();
                mo.Dispose();
            }

            MACAddress = MACAddress.Replace(":", "");
            return MACAddress;
        }

        /// <summary>
        ///     Retrieving Motherboard Manufacturer.
        /// </summary>
        /// <returns></returns>
        public static string GetBoardMaker()
        {
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");

            foreach (var wmi in searcher.Get().Cast<ManagementObject>())
                try
                {
                    return wmi.GetPropertyValue("Manufacturer").ToString();
                }

                catch
                {
                }

            return "Board Maker: Unknown";
        }

        /// <summary>
        ///     Retrieving Motherboard Product Id.
        /// </summary>
        /// <returns></returns>
        public static string GetBoardProductId()
        {
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");

            foreach (var wmi in searcher.Get().Cast<ManagementObject>())
                try
                {
                    return wmi.GetPropertyValue("Product").ToString();
                }

                catch
                {
                }

            return "Product: Unknown";
        }

        /// <summary>
        ///     Retrieving CD-DVD Drive Path.
        /// </summary>
        /// <returns></returns>
        public static string GetCdRomDrive()
        {
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_CDROMDrive");

            foreach (var wmi in searcher.Get().Cast<ManagementObject>())
                try
                {
                    return wmi.GetPropertyValue("Drive").ToString();
                }

                catch
                {
                }

            return "CD ROM Drive Letter: Unknown";
        }

        /// <summary>
        ///     Retrieving BIOS Maker.
        /// </summary>
        /// <returns></returns>
        public static string GetBIOSmaker()
        {
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

            foreach (var wmi in searcher.Get().Cast<ManagementObject>())
                try
                {
                    return wmi.GetPropertyValue("Manufacturer").ToString();
                }

                catch
                {
                }

            return "BIOS Maker: Unknown";
        }

        /// <summary>
        ///     Retrieving BIOS Serial No.
        /// </summary>
        /// <returns></returns>
        public static string GetBIOSserNo()
        {
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

            foreach (var wmi in searcher.Get().Cast<ManagementObject>())
                try
                {
                    return wmi.GetPropertyValue("SerialNumber").ToString();
                }

                catch
                {
                }

            return "BIOS Serial Number: Unknown";
        }

        /// <summary>
        ///     Retrieving BIOS Caption.
        /// </summary>
        /// <returns></returns>
        public static string GetBIOScaption()
        {
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

            foreach (var wmi in searcher.Get().Cast<ManagementObject>())
                try
                {
                    return wmi.GetPropertyValue("Caption").ToString();
                }
                catch
                {
                }

            return "BIOS Caption: Unknown";
        }

        /// <summary>
        ///     Retrieving System Account Name.
        /// </summary>
        /// <returns></returns>
        public static string GetAccountName()
        {
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_UserAccount");

            foreach (var wmi in searcher.Get().Cast<ManagementObject>())
                try
                {
                    return wmi.GetPropertyValue("Name").ToString();
                }
                catch
                {
                }

            return "User Account Name: Unknown";
        }

        /// <summary>
        ///     Retrieving Physical Ram Memory.
        /// </summary>
        /// <returns></returns>
        public static string GetPhysicalMemory()
        {
            var oMs = new ManagementScope();
            var oQuery = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
            var oSearcher = new ManagementObjectSearcher(oMs, oQuery);
            var oCollection = oSearcher.Get();

            long MemSize = 0;

            // In case more than one Memory sticks are installed
            foreach (var obj in oCollection.Cast<ManagementObject>())
            {
                var mCap = Convert.ToInt64(obj["Capacity"]);
                MemSize += mCap;
            }

            MemSize = MemSize / 1024 / 1024;
            return MemSize + "MB";
        }

        /// <summary>
        ///     Retrieving No of Ram Slot on Motherboard.
        /// </summary>
        /// <returns></returns>
        public static string GetNoRamSlots()
        {
            var MemSlots = 0;
            var oMs = new ManagementScope();
            var oQuery2 = new ObjectQuery("SELECT MemoryDevices FROM Win32_PhysicalMemoryArray");
            var oSearcher2 = new ManagementObjectSearcher(oMs, oQuery2);
            var oCollection2 = oSearcher2.Get();
            foreach (var obj in oCollection2.Cast<ManagementObject>()) MemSlots = Convert.ToInt32(obj["MemoryDevices"]);
            return MemSlots.ToString();
        }

        //Get CPU Temprature.
        /// <summary>
        ///     method for retrieving the CPU Manufacturer
        ///     using the WMI class
        /// </summary>
        /// <returns>CPU Manufacturer</returns>
        public static string GetCPUManufacturer()
        {
            var cpuMan = string.Empty;
            //create an instance of the Managemnet class with the
            //Win32_Processor class
            var mgmt = new ManagementClass("Win32_Processor");
            //create a ManagementObjectCollection to loop through
            var objCol = mgmt.GetInstances();
            //start our loop for all processors found
            foreach (var obj in objCol.Cast<ManagementObject>())
                if (cpuMan == string.Empty)
                    // only return manufacturer from first CPU
                    cpuMan = obj.Properties["Manufacturer"].Value.ToString();
            return cpuMan;
        }

        /// <summary>
        ///     method to retrieve the CPU's current
        ///     clock speed using the WMI class
        /// </summary>
        /// <returns>Clock speed</returns>
        public static int GetCPUCurrentClockSpeed()
        {
            var cpuClockSpeed = 0;
            //create an instance of the Managemnet class with the
            //Win32_Processor class
            var mgmt = new ManagementClass("Win32_Processor");
            //create a ManagementObjectCollection to loop through
            var objCol = mgmt.GetInstances();
            //start our loop for all processors found
            foreach (var obj in objCol.Cast<ManagementObject>())
                if (cpuClockSpeed == 0)
                    // only return cpuStatus from first CPU
                    cpuClockSpeed = Convert.ToInt32(obj.Properties["CurrentClockSpeed"].Value.ToString());
            //return the status
            return cpuClockSpeed;
        }

        /// <summary>
        ///     method to retrieve the network adapters
        ///     default IP gateway using WMI
        /// </summary>
        /// <returns>adapters default IP gateway</returns>
        public static string GetDefaultIPGateway()
        {
            //create out management class object using the
            //Win32_NetworkAdapterConfiguration class to get the attributes
            //of the network adapter
            var mgmt = new ManagementClass("Win32_NetworkAdapterConfiguration");
            //create our ManagementObjectCollection to get the attributes with
            var objCol = mgmt.GetInstances();
            var gateway = string.Empty;
            //loop through all the objects we find
            foreach (var obj in objCol.Cast<ManagementObject>())
            {
                if (gateway == string.Empty) // only return MAC Address from first card
                    //grab the value from the first network adapter we find
                    //you can change the string to an array and get all
                    //network adapters found as well
                    //check to see if the adapter's IPEnabled
                    //equals true
                    if ((bool)obj["IPEnabled"])
                        gateway = obj["DefaultIPGateway"].ToString();
                //dispose of our object
                obj.Dispose();
            }

            //replace the ":" with an empty space, this could also
            //be removed if you wish
            gateway = gateway.Replace(":", "");
            //return the mac address
            return gateway;
        }

        /// <summary>
        ///     Retrieve CPU Speed.
        /// </summary>
        /// <returns></returns>
        public static double? GetCpuSpeedInGHz()
        {
            double? GHz = null;
            using (var mc = new ManagementClass("Win32_Processor"))
            {
                foreach (var mo in mc.GetInstances().Cast<ManagementObject>())
                {
                    GHz = 0.001 * (uint)mo.Properties["CurrentClockSpeed"].Value;
                    break;
                }
            }

            return GHz;
        }

        public static string GetCurrentUserName()
        {
            return Environment.UserName;
        }

        public static string GetHostName()
        {
            return Dns.GetHostName();
        }
    }
}