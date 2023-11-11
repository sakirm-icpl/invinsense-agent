using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

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

        private void ShowToolTipClick(object sender, EventArgs e)
        {
            NotifyTrayIcon.ShowBalloonTip(1000, Properties.Resources.IsolationDialogTitle, Properties.Resources.IsolationDialogMessage, ToolTipIcon.Info);
        }

        private void ReadFirstFileClick(object sender, EventArgs e)
        {
            var (success, error, configValues) = ReadFile("ossec_1.conf");

            if(success)
            {
                lblConfigValue.Text = string.Join(",", configValues);
                lblCulture.Text = MapRequiredCulture(configValues);
            }
            else
            {
                lblConfigValue.Text = error;
                lblCulture.Text = "en-US";
            }
        }

        private void ReadSecondFileClick(object sender, EventArgs e)
        {
            var (success, error, configValues) = ReadFile("ossec_2.conf");

            if (success)
            {
                lblConfigValue.Text = string.Join(",", configValues);
                lblCulture.Text = MapRequiredCulture(configValues);
            }
            else
            {
                lblConfigValue.Text = error;
                lblCulture.Text = "en-US";
            }
        }

        private (bool, string, IEnumerable<string>) ReadFile(string configPath)
        {
            if(File.Exists(configPath) == false)
            {
                return (false, "File not found", Array.Empty<string>());
            }

            try
            {
                string xmlContent = File.ReadAllText(configPath);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                XmlNode groupsNode = doc.SelectSingleNode("//groups");
                if (groupsNode != null)
                {
                    string groupsText = groupsNode.InnerText;
                    string[] values = groupsText.Split(',');
                    return (true, "", values);
                }
                else
                {
                    return (false, "The <groups> element was not found.", Array.Empty<string>());
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, Array.Empty<string>());
            }
        }

        private string MapRequiredCulture(IEnumerable<string> groups)
        {
            if(groups.Any(x => x == "singapore"))
            {
                return "en-SG";
            }
            else if(groups.Any(x => x == "malaysia"))
            {
                return "ms-MY";
            }
            else if(groups.Any(x => x == "thailand"))
            {
                return "th-TH";
            }
            else if(groups.Any(x => x == "indonesia"))
            {
                return "id-ID";
            }
            else if(groups.Any(x => x == "philippines"))
            {
                return "fil-PH";
            }
            else if(groups.Any(x => x == "vietnam"))
            {
                return "vi-VN";
            }
            
            return "en-US";
        }
    }
}
