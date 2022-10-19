using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;

namespace IvsTray
{
    [RunInstaller(true)]
    public partial class IvsTrayInstaller : Installer
    {
        public IvsTrayInstaller()
        {
            InitializeComponent();

            // Attach the 'Committed' event.
            Committed += new InstallEventHandler(CommittedEventHandler);
        }

        // Event handler for 'Committed' event.
        private void CommittedEventHandler(object sender, InstallEventArgs e)
        {
            try
            {
                foreach (System.Collections.DictionaryEntry item in Context.Parameters)
                {
                    File.AppendAllText("C:\\trayerror.log", $"Key: {item.Key}, Value: {item.Value} {Environment.NewLine}");
                }

                string agentExecutable = Context.Parameters["assemblypath"];
                Process.Start(agentExecutable);
            }
            catch (Exception ex)
            {
                File.AppendAllText("C:\\trayerror.log", ex.StackTrace);
            }
        }
    }
}
