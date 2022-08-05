using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using System.DirectoryServices;
using System.Diagnostics;

namespace Invinsense3._0
{
    [RunInstaller(true)]
    public partial class ProjectInstaller :Installer
    {
        public static string Name="Single Agent";

       // public ServiceControllerStatus ServiceControllerStatus { get; private set; }

        public ProjectInstaller()
        {
            InitializeComponent(); //generated code including property settings from previous steps
            this.serviceInstaller1.AfterInstall += Autorun_AfterServiceInstall; //use your ServiceInstaller name if changed from serviceInstaller1
            
        }
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            createUser(Name);
            createFile();
            
        }
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            removeUser(Name);
            removefile();
            //EventWaitHandle ev = new EventWaitHandle(false, EventResetMode.AutoReset, "Invinsense3.0");
            // ev.WaitOne();
            foreach (var process in Process.GetProcessesByName("SingleAgent"))
            {
                process.Kill();
                Environment.Exit(0);
            }
            //Environment.Exit(0);
        }
         void Autorun_AfterServiceInstall(object sender, InstallEventArgs e)
         {
            ServiceInstaller serviceInstaller = (ServiceInstaller)sender;
            using (ServiceController sc = new ServiceController(serviceInstaller.ServiceName))
            {
                sc.Start();
            }            
        }
        void createFile()
        {
            var dir = @"C:\Single Agent\WindowUser";  // folder location
            if (!Directory.Exists(dir))  // if it doesn't exist, create
                Directory.CreateDirectory(dir);

            // use Path.Combine to combine 2 strings to a path
            File.WriteAllText(Path.Combine(dir, "WindowUser.txt"), "Username-Single Agent");
        }
        public static void createUser(string Name)
        {
            try
            {
                DirectoryEntry AD = new DirectoryEntry("WinNT://" +
                                    Environment.MachineName + ",computer");
                DirectoryEntry NewUser = AD.Children.Add(Name, "user");
              //  NewUser.Invoke("SetPassword", new object[] { Pass });
                NewUser.Invoke("Put", new object[] { "Description", "Test User from .NET" });
                NewUser.CommitChanges();
                DirectoryEntry grp;

                grp = AD.Children.Find("Administrators", "group");
                if (grp != null) { grp.Invoke("Add", new object[] { NewUser.Path.ToString() }); }
               // Console.WriteLine("Account Created Successfully");
               // Console.WriteLine("Press Enter to continue....");
              //  Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
              //  Console.ReadLine();
            }
        }
        public static void removeUser(string Name)
        {
            DirectoryEntry localDirectory = new DirectoryEntry("WinNT://" + Environment.MachineName.ToString());
            DirectoryEntries users = localDirectory.Children;
            DirectoryEntry user = users.Find(Name);
            users.Remove(user);
        }
        public static void removefile()
        {
            Directory.Delete("c:/Single Agent", true);
        }
        /* protected override void OnAfterUninstall(IDictionary savedState)
         {
             var path = System.IO.Path.Combine(Context.Parameters["path"], "log");
             System.IO.Directory.Delete(path, true);
         }*/
        /* protected override void OnAfterInstall(IDictionary savedState)
         {
             base.OnAfterInstall(savedState);
             string folderName = @"c:\Single Agent";
             string pathString = System.IO.Path.Combine(folderName, "Agent");
            // string pathString2 = @"c:\Single Agent\Agent";

             System.IO.Directory.CreateDirectory(pathString);
             string fileName = "MyNewFile.txt";
             pathString = System.IO.Path.Combine(pathString, fileName);
             //Console.WriteLine("Path to my file: {0}\n", pathString);
             using (System.IO.FileStream fs = new System.IO.FileStream(pathString, FileMode.Append))
             {
                 for (byte i = 0; i < 100; i++)
                 {
                     //fs.WriteByte(i);
                     File.WriteAllText("MyNewFile.txt",Name);
                 }
             }

             // Read and display the data from your file.
             try
             {
                 byte[] readBuffer = System.IO.File.ReadAllBytes(pathString);
                 foreach (byte b in readBuffer)
                 {
                     Console.Write(b + " ");
                 }
                // Console.WriteLine();
             }
             catch (System.IO.IOException e)
             {
                Console.WriteLine(e.Message);
             }

             // Keep the console window open in debug mode.
             //System.Console.WriteLine("Press any key to exit.");
             //System.Console.ReadKey();
         }*/
    }
}
