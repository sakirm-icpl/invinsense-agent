using Common.Models;
using System.Drawing;

namespace IvsTray
{
    public static class StatusIconExtensions
    {
        public static Image Convert(RunningStatus runningStatus)
        {
            switch (runningStatus)
            {
                case RunningStatus.Unknown:
                    return Properties.Resources.gray;
                case RunningStatus.Running:
                    return Properties.Resources.green;
                case RunningStatus.Warning:
                    return Properties.Resources.yellow;
                case RunningStatus.Stopped:
                case RunningStatus.Error:
                    return Properties.Resources.red;
                default:
                    return Properties.Resources.gray;
            }
        }
    }
}
