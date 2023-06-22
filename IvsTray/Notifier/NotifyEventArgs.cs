namespace IvsTray.Notifier
{
    public class NotifyEventArgs
    {
        public NotifyType NotifyType { get; protected set; }

        public string Title { get; protected set; }

        public string Message { get; protected set; }

        public NotifyEventArgs(NotifyType notifyType, string title, string message)
        {
            NotifyType = notifyType;
            Title = title;
            Message = message;
        }
    }
}