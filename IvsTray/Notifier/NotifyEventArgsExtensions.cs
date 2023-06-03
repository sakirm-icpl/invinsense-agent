using Common.Persistance;
using System.Windows.Forms;

namespace IvsTray
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
                case RunningStatus.NotFound:
                case RunningStatus.Error:
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