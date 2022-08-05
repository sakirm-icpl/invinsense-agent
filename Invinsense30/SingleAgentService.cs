using Serilog;
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
            Log.Information("Service is started");
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds  
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            Log.Information("Service is stopped");
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            Log.Information("Service timer");
        }
    }
}
