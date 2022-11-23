using System;
using System.Windows.Forms;

namespace FormsTest
{
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            notifyIcon1.ShowBalloonTip(1000, "test", "Hello tray", ToolTipIcon.Info);
        }
    }
}
