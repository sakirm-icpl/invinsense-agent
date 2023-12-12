using System;
using System.Collections.Generic;

namespace SystemManagementTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DisplayAllHardwareInfo();
            Console.ReadLine();
        }

        private static void DisplayAllHardwareInfo()
        {
            var infos = new Dictionary<string, string>
            {
                { "Proccesor Id", HardwareInfo.GetProcessorId() },
                { "HDD SerialNo", HardwareInfo.GetHDDSerialNo() },
                { "Mac Address", HardwareInfo.GetMACAddress() },
                { "Board Maker", HardwareInfo.GetBoardMaker() },
                { "Board Product Id", HardwareInfo.GetBoardProductId() },
                { "CdRomDrive", HardwareInfo.GetCdRomDrive() },
                { "Bios Maker", HardwareInfo.GetBIOSmaker() },
                { "Bios Serial Number", HardwareInfo.GetBIOSserNo() },
                { "Bios Caption", HardwareInfo.GetBIOScaption() },
                { "Account Name", HardwareInfo.GetAccountName() },
                { "Physcial Memory", HardwareInfo.GetAccountName() },
                { "Ram Slots", HardwareInfo.GetNoRamSlots() },
                { "Cpu Manufacturer", HardwareInfo.GetCPUManufacturer() },
                { "IP Gateway", HardwareInfo.GetDefaultIPGateway() },
                { "CPU Speed", HardwareInfo.GetCpuSpeedInGHz().ToString() },
                { "Username", HardwareInfo.GetCurrentUserName() },
                { "Hostname", HardwareInfo.GetHostName() }
            };

            Console.WriteLine("---------- ----  ---- Hardware information of the device is as follows : ----  ----  ----------\n");
            foreach (KeyValuePair<string, string> item in infos)
            {
                Console.WriteLine($"   {item.Key}: {item.Value}");
            }
        }
    }
}