using Common.Models;

namespace Common.Events
{
    public class NotifyEventArgs
    {
        public NotifyEventArgs(NotifyType notifyType, string title, string message)
        {
            NotifyType = notifyType;
            Title = title;
            Message = message;
        }

        public NotifyType NotifyType { get; protected set; }

        public string Title { get; protected set; }

        public string Message { get; protected set; }
    }
}