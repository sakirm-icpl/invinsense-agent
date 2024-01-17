using System.Windows.Forms;

namespace IvsTray.UserControls
{
    public partial class FooterUserControl : UserControl
    {
        public FooterUserControl()
        {
            InitializeComponent();
            lblVersion.Text = Common.Constants.ProductVersion;
        }
    }
}
