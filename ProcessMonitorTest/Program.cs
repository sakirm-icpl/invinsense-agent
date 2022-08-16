﻿using System;
using System.Management;

namespace ProcessMonitorTest
{
    /// <summary>
    /// 
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected Program() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ManagementEventWatcher startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            startWatch.EventArrived += new EventArrivedEventHandler(StartWatchEventArrived);
            startWatch.Start();
            ManagementEventWatcher stopWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            stopWatch.EventArrived += new EventArrivedEventHandler(StopWatchEventArrived);
            stopWatch.Start();
            Console.WriteLine("Press any key to exit");
            while (!Console.KeyAvailable) System.Threading.Thread.Sleep(50);
            startWatch.Stop();
            stopWatch.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void StopWatchEventArrived(object sender, EventArrivedEventArgs e)
        {
            var processName = e.NewEvent.Properties["ProcessName"].Value.ToString();

            if (processName.ToLower().Contains("notepad"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.WriteLine("Process stopped: {0}", processName);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void StartWatchEventArrived(object sender, EventArrivedEventArgs e)
        {
            var processName = e.NewEvent.Properties["ProcessName"].Value.ToString();

            if (processName.ToLower().Contains("notepad"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.WriteLine("Process started: {0}", e.NewEvent.Properties["ProcessName"].Value);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}