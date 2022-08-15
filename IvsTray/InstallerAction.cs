using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;

namespace IvsTray
{
    [RunInstaller(true)]
    public partial class InstallerAction : Installer
    {
        public InstallerAction()
        {
            InitializeComponent();

            // Attach the 'Committed' event.
            Committed += new InstallEventHandler(CommittedEventHandler);
        }

        // Event handler for 'Committed' event.
        private void CommittedEventHandler(object sender, InstallEventArgs e)
        {
            Process.Start(Path.GetDirectoryName(Context.Parameters["AssemblyPath"]) + @"\IvsTray.exe");
        }
    }
}
