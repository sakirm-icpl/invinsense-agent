using Common.Models;
using System.Windows.Forms;

namespace IvsTray.Extensions
{
    public static class NotifyEventArgsExtensions
    {
        public static ToolTipIcon ConvertIcon(NotifyType notifyType)
        {
            switch (notifyType)
            {
                case NotifyType.Error:
                    return ToolTipIcon.Error;
                case NotifyType.Warning:
                    return ToolTipIcon.Warning;
                case NotifyType.Info:
                    return ToolTipIcon.Info;
                default:
                    return ToolTipIcon.None;
            }
        }

        public static NotifyType ConvertStatus(RunningStatus runningStatus)
        {
            switch (runningStatus)
            {
                case RunningStatus.Running:
                    return NotifyType.Info;
                case RunningStatus.Warning:
                    return NotifyType.Warning;
                case RunningStatus.Unknown:
                case RunningStatus.Error:
                case RunningStatus.Stopped:
                    return NotifyType.Error;
                default:
                    return NotifyType.Info;
            }
        }

        public static string BuildMessage(string toolName, RunningStatus runningStatus)
        {
            return $"{toolName} status: {runningStatus}";
        }
    }
}