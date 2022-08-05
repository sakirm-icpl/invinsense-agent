using System;
using System.ServiceProcess;
using System.Timers;

namespace Invinsense30
{
    public partial class SingleAgentService : ServiceBase
    {
        private readonly Timer timer = new Timer(); 
       
        public SingleAgentService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
          //  WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds  
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
          //  WriteToFile("Service is stopped at " + DateTime.Now);
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
          //  WriteToFile("Service is recall at " + DateTime.Now);
        }
    }
}
