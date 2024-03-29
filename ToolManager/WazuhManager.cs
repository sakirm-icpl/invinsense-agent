﻿using Serilog;
using Common.ConfigProvider;
using Common.FileHelpers;
using Common.RegistryHelpers;
using System.IO;
using System;
using System.Xml;
using Common.Mappers;
using Common.Models;

namespace ToolManager
{
    public sealed class WazuhManager : ProductManager
    {
        public WazuhManager(ToolDetail toolDetail) : base(toolDetail, Log.ForContext(typeof(WazuhManager)))
        {
            _logger.Information($"Initializing {nameof(WazuhManager)} Manager");
        }

        protected override void BeforeUninstall(InstallStatusWithDetail detail)
        {
            
        }

        /// <summary>
        /// 1. Copy local_internal_options.conf
        /// 2. Extract active-response\\bin
        /// 3. Check REGISTRATION_PASSWORD from install instruction and set to authd.pass
        /// 4. Set environment variables
        /// 5. Enable OSQueryWoodle
        /// 6. Set I18n
        /// </summary>
        /// <returns></returns>
        protected override void PostInstall()
        {
            var sourceFolder = Path.Combine(CommonUtils.ArtifactsFolder, _toolDetail.Name);
            var destinationFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "ossec-agent");

            _logger.Information($"Source: {sourceFolder}, Destination: {destinationFolder}");

            //Copy local_internal_options.conf to installation path
            var configSource = Path.Combine(sourceFolder, "local_internal_options.conf");
            var configDestination = Path.Combine(destinationFolder, "local_internal_options.conf");
            CommonFileHelpers.EnsureSourceToDestination(configSource, configDestination);

            //Copy active to installation path
            var packsSourcePath = Path.Combine(sourceFolder, "active-response.zip");
            var packsDestinationPath = Path.Combine(destinationFolder, "active-response", "bin");
            CommonFileHelpers.ExtractSourceToDestination(packsSourcePath, packsDestinationPath);

            //Set environment variables
            EnsureEnvironmentVariables(destinationFolder);

            //Enable osquery for END_POINT_DETECTION_AND_RESPONSE
            EnableOsQueryWoodle(destinationFolder);

            //Set Isolation message
            EnsureIsolationMessage(destinationFolder);
        }

        /// <summary>
        /// Enable osquery for END_POINT_DETECTION_AND_RESPONSE
        /// </summary>
        private void EnableOsQueryWoodle(string destinationFolder)
        {
            _logger.Information($"Enabling {ToolName.OsQuery} for {ToolName.Wazuh}");

            var confFile = Path.Combine(destinationFolder, "ossec.conf");

            if (!File.Exists(confFile))
            {
                _logger.Error("END_POINT_DETECTION_AND_RESPONSE configuration not found.");
                return;
            }

            XmlDocument document = new XmlDocument();
            document.Load(confFile);
            XmlNodeList osQueryDisableNodeItems = document.SelectNodes("/ossec_config/wodle[@name='osquery']/disabled");
            if (osQueryDisableNodeItems.Count > 0 && osQueryDisableNodeItems[0].InnerText != "no")
            {
                osQueryDisableNodeItems[0].InnerText = "no";
            }

            XmlNodeList osQueryRunDaemonNodeItems = document.SelectNodes("/ossec_config/wodle[@name='osquery']/run_daemon");
            if (osQueryRunDaemonNodeItems.Count > 0 && osQueryRunDaemonNodeItems[0].InnerText != "no")
            {
                osQueryRunDaemonNodeItems[0].InnerText = "no";
            }

            document.Save(confFile);
        }

        /// <summary>
        /// Environment Variables: WAZUH_MANAGER, WAZUH_REGISTRATION_SERVER
        /// </summary>
        private void EnsureEnvironmentVariables(string destinationFolder)
        {
            foreach (var arg in _toolDetail.InstallInstruction.InstallArgs)
            {
                if (arg.StartsWith("WAZUH_MANAGER="))
                {
                    var managerIp = arg.Split('=')[1];
                    Environment.SetEnvironmentVariable("WAZUH_MANAGER", managerIp, EnvironmentVariableTarget.Machine);
                    continue;
                }

                if (arg.StartsWith("WAZUH_REGISTRATION_SERVER="))
                {
                    var registrationIp = arg.Split('=')[1];
                    Environment.SetEnvironmentVariable("WAZUH_REGISTRATION_SERVER", registrationIp, EnvironmentVariableTarget.Machine);
                    continue;
                }

                //Set password and environment variables
                if (arg.StartsWith("REGISTRATION_PASSWORD="))
                {
                    var password = arg.Split('=')[1];
                    var passFile = Path.Combine(destinationFolder, "authd.pass");
                    File.WriteAllText(passFile, password);
                }
            }
        }

        /// <summary>
        /// This is quick implementation to set Isolation message in registry.
        /// </summary>
        private void EnsureIsolationMessage(string destinationFolder)
        {
            var groups = WinRegistryHelper.GetPropertyByName($"{Common.Constants.CompanyName}", "Groups");
            _logger.Information($"Groups from registry: {groups}");

            if (string.IsNullOrEmpty(groups))
            {
                var response = ReadGroups(destinationFolder);

                if(response.Item1)
                {
                    groups = response.Item3;
                }
                else
                {
                    _logger.Warning("Default groups are not set. Setting default.");
                    groups = "default";
                }

                _logger.Information($"Groups from config: {groups}");

                WinRegistryHelper.SetPropertyByName($"{Common.Constants.CompanyName}", "Groups", groups);
            }

            var displayMessage = DisplayMessageMapper.MapNetworkIsolationMessage(groups.Split(','));

            var i18Path = $"{Common.Constants.CompanyName}\\i18n";
            WinRegistryHelper.SetPropertyByName(i18Path, "Groups", groups);
            WinRegistryHelper.SetPropertyByName(i18Path, "IsolationTitle", displayMessage.Title);
            WinRegistryHelper.SetPropertyByName(i18Path, "IsolationMessage", displayMessage.Message);
        }

        private static (bool, string, string) ReadGroups(string destinationFolder)
        {
            var configPath = Path.Combine(destinationFolder, "ossec.conf");

            if (File.Exists(configPath) == false)
            {
                return (false, "ossec.conf file not found under program files. Please ensure that wazuh agent is installed.", "");
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
                    return (true, "", groupsText);
                }
                else
                {
                    return (false, "The <groups> element was not found.", "");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, "");
            }
        }
    }
}