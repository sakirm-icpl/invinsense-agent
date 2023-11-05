using System;
using System.Windows.Forms;

namespace FormsTest
{
    public partial class MainFrm : Form
    {
        /// <summary>
        /// https://azuliadesigns.com/c-sharp-tutorials/list-net-culture-country-codes/
        /// </summary>
        public MainFrm()
        {
            InitializeComponent();

            //use localizable strings
            Text = Properties.Resources.MainFormTitle;
            ToolTipButton.Text = Properties.Resources.ButtonText;
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            NotifyTrayIcon.ShowBalloonTip(1000, Properties.Resources.IsolationDialogTitle, Properties.Resources.IsolationDialogMessage, ToolTipIcon.Info);
        }
    }
}
