using System;

namespace AvMonitorTest
{
    internal class Program
    {
        static void Main()
        {
            var avStatuses = AvMonitor.ListAvStatuses();

            foreach (var avStatus in avStatuses)
            {
                Console.WriteLine($"Antivirus status: {avStatus}");
            }

            Console.ReadLine();
        }
    }
}
