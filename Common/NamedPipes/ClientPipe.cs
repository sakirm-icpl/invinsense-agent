using System;
using System.IO.Pipes;

namespace Common.NamedPipes
{
    public class ClientPipe : BasicPipe
    {
        protected NamedPipeClientStream clientPipeStream;

        public ClientPipe(string serverName, string pipeName, Action<BasicPipe> asyncReaderStart) : base(pipeName)
        {
            this.asyncReaderStart = asyncReaderStart;
            clientPipeStream = new NamedPipeClientStream(serverName, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            pipeStream = clientPipeStream;
        }

        public void Connect()
        {
            _logger.Verbose("Connecting to pipe");
            clientPipeStream.Connect();
            asyncReaderStart(this);
        }
    }
}