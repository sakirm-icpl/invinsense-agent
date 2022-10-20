using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;

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
                string agentExecutable = Context.Parameters["assemblypath"];

                foreach (System.Collections.DictionaryEntry item in Context.Parameters)
                {
                    Context.LogMessage($"Key: {item.Key}, Value: {item.Value} {Environment.NewLine}");
                }
                
                Process.Start(agentExecutable);
            }
            catch (Exception ex)
            {
                Context.LogMessage(ex.StackTrace);
            }
        }
    }
}
