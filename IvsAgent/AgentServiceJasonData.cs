using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvsAgent
{
    public class AgentServiceJasonData
    {
        public string ServiceStart { get; set; }
        public string ServiceStarted {   get; set; }

        public string ServiceSchedule { get; set; }

        public string TaskAdd { get; set; }

        public string ServiceStopped { get; set; }

        public string ServicePause { get; set; }

        public string ServiceContinue { get; set; }

        public int ServiceCustomCommand { get; set; }

        public string ServiceStudDown { get; set; }

        public string IvsTrayStatus { get; set; }

        public string IvsTrayInfo { get; set; }

        public string AvName { get; set; }

        public string AvDescription { get; set; }   

        public string SysmonServiceVerfied { get; set; }

        public string SysmonServiceListing { get; set; }

        public string SysmonServiceError { get; set; }

        public string OsQueryServiceVerfied { get; set; }

        public string OsQueryServiceListing { get; set; }

        public string OsQueryServiceError { get; set; }

        public string WazuhServiceVerfied { get; set; }

        public string WazuhServiceListing { get; set; }

        public string WazuhServiceError { get; set; }

        public string DbytesServiceVerfied { get; set; }

        public string DbytesServiceListing { get; set; }

        public string DbytesServiceError { get; set; }

        public string FakeUser { get; set; }
    }
}
