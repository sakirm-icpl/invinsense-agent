using System;

namespace AvMonitorTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var status = AvMonitor.AVStatus("Windows Defender");

            Console.WriteLine($"Antivirus status: {status}");
        }
    }
}
