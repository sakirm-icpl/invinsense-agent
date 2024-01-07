using ConsoleMenu;
using System;
using System.Collections.Generic;

namespace SystemManagementTest
{
    internal class Program
    {
        private static void Main()
        {
            var consoleMenu = new ConsoleMenuUtility();
            consoleMenu.DisplayMenuAndHandleInput();
        }

        [ConsoleOption(1, "Show full hardware information")]
        public static void DisplayAllHardwareInfo()
        {
            var infoList = new Dictionary<string, string>
            {
                { "Processor Id", HardwareInfo.GetProcessorId() },
                { "HDD SerialNo", HardwareInfo.GetHDDSerialNo() },
                { "Mac Address", HardwareInfo.GetMACAddress() },
                { "Board Maker", HardwareInfo.GetBoardMaker() },
                { "Board Product Id", HardwareInfo.GetBoardProductId() },
                { "CdRomDrive", HardwareInfo.GetCdRomDrive() },
                { "Bios Maker", HardwareInfo.GetBIOSmaker() },
                { "Bios Serial Number", HardwareInfo.GetBIOSserNo() },
                { "Bios Caption", HardwareInfo.GetBIOScaption() },
                { "Account Name", HardwareInfo.GetAccountName() },
                { "Physical Memory", HardwareInfo.GetAccountName() },
                { "Ram Slots", HardwareInfo.GetNoRamSlots() },
                { "Cpu Manufacturer", HardwareInfo.GetCPUManufacturer() },
                { "IP Gateway", HardwareInfo.GetDefaultIPGateway() },
                { "CPU Speed", HardwareInfo.GetCpuSpeedInGHz().ToString() },
                { "Username", HardwareInfo.GetCurrentUserName() },
                { "Hostname", HardwareInfo.GetHostName() }
            };

            Console.WriteLine("---------- ----  ---- Hardware information of the device is as follows : ----  ----  ----------\n");
            foreach (KeyValuePair<string, string> item in infoList)
            {
                Console.WriteLine($"   {item.Key}: {item.Value}");
            }
        }

        [ConsoleOption(2, "Show processor id")]
        public static void DisplayProcessorId()
        {
            Console.WriteLine($"Processor Id: {HardwareInfo.GetProcessorId()}");
        }

        [ConsoleOption(3, "Show HDD serial number")]
        public static void DisplayHDDSerialNo()
        {
            Console.WriteLine($"HDD SerialNo: {HardwareInfo.GetHDDSerialNo()}");
        }

        [ConsoleOption(4, "Show MAC address")]
        public static void DisplayMACAddress()
        {
            Console.WriteLine($"Mac Address: {HardwareInfo.GetMACAddress()}");
        }

        [ConsoleOption(5, "Show board maker")]
        public static void DisplayBoardMaker()
        {
            Console.WriteLine($"Board Maker: {HardwareInfo.GetBoardMaker()}");
        }

        [ConsoleOption(6, "Show board product id")]
        public static void DisplayBoardProductId()
        {
            Console.WriteLine($"Board Product Id: {HardwareInfo.GetBoardProductId()}");
        }

        [ConsoleOption(7, "Show CD ROM drive")]
        public static void DisplayCdRomDrive()
        {
            Console.WriteLine($"CdRomDrive: {HardwareInfo.GetCdRomDrive()}");
        }

        [ConsoleOption(8, "Show BIOS maker")]
        public static void DisplayBIOSmaker()
        {
            Console.WriteLine($"Bios Maker: {HardwareInfo.GetBIOSmaker()}");
        }

        [ConsoleOption(9, "Show BIOS serial number")]
        public static void DisplayBIOSserNo()
        {
            Console.WriteLine($"Bios Serial Number: {HardwareInfo.GetBIOSserNo()}");
        }

        [ConsoleOption(10, "Show BIOS caption")]
        public static void DisplayBIOScaption()
        {
            Console.WriteLine($"Bios Caption: {HardwareInfo.GetBIOScaption()}");
        }

        [ConsoleOption(11, "Show account name")]
        public static void DisplayAccountName()
        {
            Console.WriteLine($"Account Name: {HardwareInfo.GetAccountName()}");
        }

        [ConsoleOption(12, "Show physical memory")]
        public static void DisplayPhysicalMemory()
        {
            Console.WriteLine($"Physical Memory: {HardwareInfo.GetPhysicalMemory()}");
        }

        [ConsoleOption(13, "Show RAM slots")]
        public static void DisplayNoRamSlots()
        {
            Console.WriteLine($"Ram Slots: {HardwareInfo.GetNoRamSlots()}");
        }

        [ConsoleOption(14, "Show CPU manufacturer")]
        public static void DisplayCPUManufacturer()
        {
            Console.WriteLine($"Cpu Manufacturer: {HardwareInfo.GetCPUManufacturer()}");
        }

        [ConsoleOption(15, "Show IP gateway")]
        public static void DisplayDefaultIPGateway()
        {
            Console.WriteLine($"IP Gateway: {HardwareInfo.GetDefaultIPGateway()}");
        }

        [ConsoleOption(16, "Show CPU speed")]
        public static void DisplayCpuSpeedInGHz()
        {
            Console.WriteLine($"CPU Speed: {HardwareInfo.GetCpuSpeedInGHz()}");
        }

        [ConsoleOption(17, "Show username")]
        public static void DisplayCurrentUserName()
        {
            Console.WriteLine($"Username: {HardwareInfo.GetCurrentUserName()}");
        }

        [ConsoleOption(18, "Show hostname")]
        public static void DisplayHostName()
        {
            Console.WriteLine($"Hostname: {HardwareInfo.GetHostName()}");
        }
    }
}