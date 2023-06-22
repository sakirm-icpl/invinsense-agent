using System;
using System.IO.Pipes;
using System.Security.AccessControl;

namespace Common.NamedPipes
{
    public class ServerPipe : BasicPipe
    {
        public event EventHandler<EventArgs> Connected;

        protected NamedPipeServerStream serverPipeStream;

        protected string PipeName { get; set; }

        public ServerPipe(string pipeName, Action<BasicPipe> asyncReaderStart) : base(pipeName)
        {
            this.asyncReaderStart = asyncReaderStart;

            PipeName = pipeName;

            var pipeSecurity = new PipeSecurity();
            pipeSecurity.AddAccessRule(new PipeAccessRule("Everyone", PipeAccessRights.FullControl, AccessControlType.Allow));

            serverPipeStream = new NamedPipeServerStream(
                pipeName,
                PipeDirection.InOut,
                NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous,
                0,
                0,
                pipeSecurity);

            pipeStream = serverPipeStream;
            serverPipeStream.BeginWaitForConnection(new AsyncCallback(PipeConnected), null);
        }

        protected void PipeConnected(IAsyncResult ar)
        {
            _logger.Verbose("Pipe connected");
            serverPipeStream.EndWaitForConnection(ar);
            Connected?.Invoke(this, new EventArgs());
            asyncReaderStart(this);
        }
    }
}