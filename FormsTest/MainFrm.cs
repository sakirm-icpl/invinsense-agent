using Common.Mappers;
using Common.RegistryHelpers;
using FormsTest.Properties;
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
        ///     https://azuliadesigns.com/c-sharp-tutorials/list-net-culture-country-codes/
        /// </summary>
        public MainFrm()
        {
            InitializeComponent();

            //use localizable strings
            Text = Resources.MainFormTitle;
            ToolTipButton.Text = Resources.ButtonText;
        }

        private void ShowToolTipClick(object sender, EventArgs e)
        {
            MessageBox.Show(Resources.IsolationDialogMessage, Resources.IsolationDialogTitle, MessageBoxButtons.OK);
            NotifyTrayIcon.ShowBalloonTip(1000, Resources.IsolationDialogTitle, Resources.IsolationDialogMessage, ToolTipIcon.Info);
        }

        private void ReadConfigFileClick(object sender, EventArgs e)
        {
            var tag = ((Control)sender).Tag.ToString();
            var configName = string.IsNullOrEmpty(tag) ? "samples\\ossec.conf" : $"samples\\ossec.{tag}.conf";

            var (success, error, configValues) = ReadFile(configName);

            if (success)
            {
                lblConfigValue.Text = string.Join(",", configValues);
                SetCulture(MapRequiredCulture(configValues));

                EnsureIsolationMessage(configValues.ToArray());
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
            if (File.Exists(configPath) == false) return (false, "File not found", Array.Empty<string>());

            try
            {
                var xmlContent = File.ReadAllText(configPath);

                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                var groupsNode = doc.SelectSingleNode("//groups");
                if (groupsNode != null)
                {
                    var groupsText = groupsNode.InnerText;
                    var values = groupsText.Split(',');
                    return (true, "", values);
                }

                return (false, "The <groups> element was not found.", Array.Empty<string>());
            }
            catch (Exception ex)
            {
                return (false, ex.Message, Array.Empty<string>());
            }
        }

        /// <summary>
        ///     This is quick implementation to set Isolation message in registry.
        /// </summary>
        private static void EnsureIsolationMessage(params string[] groups)
        {
            var displayMessage = DisplayMessageMapper.MapNetworkIsolationMessage(groups);

            WinRegistryHelper.SetPropertyByName("Infopercept\\I18N", "Groups", string.Join(",", groups));
            WinRegistryHelper.SetPropertyByName("Infopercept\\I18N", "IsolationTitle", displayMessage.Title);
            WinRegistryHelper.SetPropertyByName("Infopercept\\I18N", "IsolationMessage", displayMessage.Message);
        }

        private string MapRequiredCulture(IEnumerable<string> groups)
        {
            if (groups.Any(x => x == "singapore"))
                return "en-SG";
            if (groups.Any(x => x == "malaysia"))
                return "ms-MY";
            if (groups.Any(x => x == "thailand"))
                return "th-TH";
            if (groups.Any(x => x == "indonesia"))
                return "id-ID";
            if (groups.Any(x => x == "philippines"))
                return "fil-PH";
            if (groups.Any(x => x == "vietnam")) return "vi-VN";

            return "en-US";
        }

        private void ShowDialogButtonClick(object sender, EventArgs e)
        {
            var title = WinRegistryHelper.GetPropertyByName("Infopercept\\I18N", "IsolationTitle");
            var message = WinRegistryHelper.GetPropertyByName("Infopercept\\I18N", "IsolationMessage");
            MessageBox.Show(message, title, MessageBoxButtons.OK);
        }

        private void RestartButtonClick(object sender, EventArgs e)
        {
            Application.Restart();
        }
    }
}