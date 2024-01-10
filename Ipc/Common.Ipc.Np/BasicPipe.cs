using Serilog;
using System;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Ipc.Np
{
    public abstract class BasicPipe
    {
        protected readonly ILogger _logger;
        protected Action<BasicPipe> asyncReaderStart;

        protected PipeStream pipeStream;

        public BasicPipe(string Context)
        {
            _logger = Log.ForContext("PipeContext", Context);
        }

        public event EventHandler<PipeEventArgs> DataReceived;
        public event EventHandler<EventArgs> PipeClosed;

        public void Close()
        {
            _logger.Verbose("Closing pipe");

            pipeStream.WaitForPipeDrain();
            pipeStream.Close();
            pipeStream.Dispose();
            pipeStream = null;
        }

        /// <summary>
        ///     Reads an array of bytes, where the first [n] bytes (based on the server's intsize) indicates the number of bytes to
        ///     read
        ///     to complete the packet.
        /// </summary>
        public void StartByteReaderAsync()
        {
            StartByteReaderAsync(b => DataReceived?.Invoke(this, new PipeEventArgs(b, b.Length)));
        }

        /// <summary>
        ///     Reads an array of bytes, where the first [n] bytes (based on the server's intsize) indicates the number of bytes to
        ///     read
        ///     to complete the packet, and invokes the DataReceived event with a string converted from UTF8 of the byte array.
        /// </summary>
        public void StartStringReaderAsync()
        {
            StartByteReaderAsync(b =>
            {
                var str = Encoding.UTF8.GetString(b).TrimEnd('\0');
                DataReceived?.Invoke(this, new PipeEventArgs(str));
            });
        }

        public void Flush()
        {
            _logger.Verbose("Flushing pipe");
            pipeStream.Flush();
        }

        public Task WriteString(string str)
        {
            _logger.Verbose("Writing string: {str}", str);
            if (!pipeStream.IsConnected)
            {
                _logger.Verbose("Skipping as pipe is not connected.");
                return Task.CompletedTask;
            }

            return WriteBytes(Encoding.UTF8.GetBytes(str));
        }

        public Task WriteBytes(byte[] bytes)
        {
            var bufferLen = BitConverter.GetBytes(bytes.Length);
            var buffer = bufferLen.Concat(bytes).ToArray();

            return pipeStream.WriteAsync(buffer, 0, buffer.Length);
        }

        protected void StartByteReaderAsync(Action<byte[]> packetReceived)
        {
            var intSize = sizeof(int);
            var bDataLength = new byte[intSize];

            pipeStream.ReadAsync(bDataLength, 0, intSize).ContinueWith(t =>
            {
                var len = t.Result;

                if (len == 0)
                {
                    PipeClosed?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    var dataLength = BitConverter.ToInt32(bDataLength, 0);
                    var data = new byte[dataLength];

                    pipeStream.ReadAsync(data, 0, dataLength).ContinueWith(t2 =>
                    {
                        len = t2.Result;

                        if (len == 0)
                        {
                            PipeClosed?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            packetReceived(data);
                            StartByteReaderAsync(packetReceived);
                        }
                    });
                }
            });
        }
    }
}