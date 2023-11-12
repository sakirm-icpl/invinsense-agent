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
            MessageBox.Show(Properties.Resources.IsolationDialogMessage, Properties.Resources.IsolationDialogTitle, MessageBoxButtons.OK);
            NotifyTrayIcon.ShowBalloonTip(1000, Properties.Resources.IsolationDialogTitle, Properties.Resources.IsolationDialogMessage, ToolTipIcon.Info);
        }

        private void ReadConfigFileClick(object sender, EventArgs e)
        {
            var tag = ((Control)sender).Tag.ToString();
            var configName = string.IsNullOrEmpty(tag) ? "ossec.conf" : $"ossec.{tag}.conf";
            
            var (success, error, configValues) = ReadFile(configName);

            if(success)
            {
                lblConfigValue.Text = string.Join(",", configValues);
                SetCulture(MapRequiredCulture(configValues));
            }
            else
            {
                lblConfigValue.Text = error;
                SetCulture("en-US");
            }
        }

        private void SetCulture(string culture)
        {
            Environment.SetEnvironmentVariable("IVS_CULTURE", culture, EnvironmentVariableTarget.Machine);
            lblCulture.Text = culture;
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

        private void RestartButtonClick(object sender, EventArgs e)
        {
            Application.Restart();
        }
    }
}
