using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using System.DirectoryServices;
using System.Diagnostics;

namespace Invinsense30
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        private static readonly string Name = "Single Agent";

        public ProjectInstaller()
        {
            InitializeComponent(); //generated code including property settings from previous steps
            this.serviceInstaller1.AfterInstall += Autorun_AfterServiceInstall; //use your ServiceInstaller name if changed from serviceInstaller1

        }
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            CreateFakeUser(Name);
            CreateFakeFiles();

        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            RemoveFakeUser(Name);
            RemoveFakeFiles();
            
            foreach (var process in Process.GetProcessesByName("SingleAgent"))
            {
                process.Kill();
                Environment.Exit(0);
            }
        }

        void Autorun_AfterServiceInstall(object sender, InstallEventArgs e)
        {
            ServiceInstaller serviceInstaller = (ServiceInstaller)sender;
            using (ServiceController sc = new ServiceController(serviceInstaller.ServiceName))
            {
                sc.Start();
            }
        }

        void CreateFakeFiles()
        {
            var dir = @"C:\Single Agent\WindowUser";  // folder location
            if (!Directory.Exists(dir))  // if it doesn't exist, create
                Directory.CreateDirectory(dir);

            // use Path.Combine to combine 2 strings to a path
            File.WriteAllText(Path.Combine(dir, "WindowUser.txt"), "Username-Single Agent");
        }

        public static void RemoveFakeFiles()
        {
            Directory.Delete("c:/Single Agent", true);
        }

        public static void CreateFakeUser(string Name)
        {
            try
            {
                DirectoryEntry AD = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");
                DirectoryEntry NewUser = AD.Children.Add(Name, "user");
                NewUser.Invoke("Put", new object[] { "Description", "Test User from SingleAgent" });
                NewUser.CommitChanges();
                DirectoryEntry grp;

                grp = AD.Children.Find("Administrators", "group");
                if (grp != null) { grp.Invoke("Add", new object[] { NewUser.Path.ToString() }); }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void RemoveFakeUser(string Name)
        {
            DirectoryEntry localDirectory = new DirectoryEntry("WinNT://" + Environment.MachineName.ToString());
            DirectoryEntries users = localDirectory.Children;
            DirectoryEntry user = users.Find(Name);
            users.Remove(user);
        }
    }
}
