using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolManager.AgentWrappers
{
    internal class ToolsDataJason
    {
        public string DbytesStatus { get; set; }

        public string MsiInstallerStatus { get; set; }

        public string DbytesMsiPath { get; set; }

        public string DbytesLogPath { get; set; }
        public string DbytesServerIp { get; set; }

        public string DbytesApi { get; set; }

        public string DbytesError { get; set; }
        public string OsQueryStatus { get; set; }

        public string OsQueryMsiPath { get; set; }

        public string OsQueryLogPath { get; set; }
        public string OsQueryError { get; set; }

        public string SysmonStatus { get; set; }

        public string SysmonExePath { get; set; }

        public string SysmonLogPath { get; set; }
        public string SysmonError { get; set; }

        public string WazuhStatus { get; set; }

        public string WazuhMsiPath { get; set; }

        public string WazuhLogPath { get; set; }
        public string WazuhError { get; set; }

        public string WazuhRegistrationIp { get; set; }

        public string WazuhManagerIp { get; set; }

        public string WazuhAgentGroup { get; set; }
        public string platform { get; set; }
        public string system = "system";
        public string providerName { get; set; }

        public string version { get; set; }
        public string message { get; set; }
        
        public string eventData { get; set; }

        public string utcTime { get; set; }
    }
}
