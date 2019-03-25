using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace TanakaShoji.Discord.Gateway.WebSocket
{
    public class WebSocketClient
    {
        private const int ReceiveBufferSize = 1024 * 8;
        private const int SendBufferSize = 1024 * 4;

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly Task _connectTask;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly ClientWebSocket _socket;
        private readonly ObjectPool<MemoryStream> _streamPool;

        public WebSocketClient(ClientWebSocket socket)
        {
            _socket = socket;
            _streamPool = new ObjectPool<MemoryStream>(() =>
            {
                var stream = new MemoryStream(ReceiveBufferSize);
                stream.SetLength(ReceiveBufferSize);
                return stream;
            });
        }

        public WebSocketClient(Uri endpoint, string tokenType, string token) : this(new ClientWebSocket())
        {
            _socket.Options.SetRequestHeader("Authorization", $@"{tokenType} {token}");
            Logger.Debug($@"Connecting to {endpoint}");
            _connectTask = _socket.ConnectAsync(endpoint, _cts.Token);
            _connectTask.GetAwaiter().OnCompleted(() => Logger.Debug($@"Connected to {endpoint}"));
        }

        public event WebSocketMessageHandler OnMessage;

        private async Task<(MemoryStream, WebSocketReceiveResult)> ReceiveMessageStreamAsync(
            CancellationToken cancellationToken)
        {
            var streams = new List<MemoryStream> {_streamPool.Get()};
            // FIXME: catch exception then release streams
            var message = await _socket.ReceiveAsync(streams[0].GetBuffer(), cancellationToken);

            for (var lastChunk = message; !lastChunk.EndOfMessage;)
            {
                var stream = _streamPool.Get();
                lastChunk = await _socket.ReceiveAsync(new ArraySegment<byte>(stream.GetBuffer()), cancellationToken);
                stream.SetLength(lastChunk.Count);
                streams.Add(stream);
            }

            var outputStream = new MemoryStream((int) streams.Sum(stream => stream.Length)) {Position = 0};
            foreach (var chunk in streams)
            {
                chunk.CopyTo(outputStream);
                _streamPool.Put(chunk);
            }

            outputStream.Position = 0;
            return (outputStream, message);
        }

        public async Task SendMessageStreamAsync(MemoryStream stream, WebSocketMessageType type,
            CancellationToken cancellationToken)
        {
            var buffer = stream.GetBuffer();
            for (var offset = 0; offset < stream.Length; offset += SendBufferSize)
            {
                var segment = new ReadOnlyMemory<byte>(buffer, offset,
                    (int) Math.Min(stream.Length - offset, SendBufferSize));
                await _socket.SendAsync(segment, type, offset + SendBufferSize >= stream.Length, cancellationToken);
            }
        }

        public void Start(CancellationToken cancellationToken)
        {
            _connectTask.GetAwaiter().GetResult();

            while (!cancellationToken.IsCancellationRequested)
            {
                var (stream, result) = ReceiveMessageStreamAsync(cancellationToken).GetAwaiter().GetResult();
                OnMessage?.Invoke(stream, result.MessageType);
            }

            while (!_streamPool.IsEmpty)
                _streamPool.Get().Dispose();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}