using System;
using System.Diagnostics;
using System.Linq;

namespace EventLogPublisher
{
    /// <summary>
    /// Powershell Commands:
    ///     Remove-EventLog -LogName "Invinsense"
    ///     
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// default constructor
        /// </summary>
        protected Program() { }

        static void Main()
        {
            Logger.EnsureEventSource();

            Console.WriteLine("Press \"q\" to stop");
            string str;
            while ((str = Console.ReadLine()) != "q")
            {
                Logger.LogInformation(str, "SingleAgent");
            }
        }
    }
}
