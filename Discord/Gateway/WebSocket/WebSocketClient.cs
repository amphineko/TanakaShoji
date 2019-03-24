using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using TanakaShoji.Discord.Models.Gateway;

namespace TanakaShoji.Discord.Gateway.WebSocket
{
    public class WebSocketClient
    {
        public const int ReceiveBufferSize = 1024 * 8;

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly ClientWebSocket _socket;
        private readonly ObjectPool<MemoryStream> _streamPool;
        private readonly Task _connectTask;

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

        private async Task ReceiveMessageAsync(CancellationToken cancellationToken)
        {
            var (stream, result) = await ReceiveMessageStreamAsync(cancellationToken);
            using (stream)
            {
                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                        ParseTextMessage(stream);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void ParseTextMessage(MemoryStream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                stream.Position = 0;
                var baseMessage = new JsonSerializer().Deserialize<BaseGatewayMessage>(reader);

                switch (baseMessage.OpCode)
                {
                    case GatewayEventOpCode.Hello:
                        stream.Position = 0;
                        var hello = new JsonSerializer().Deserialize<GatewayEvent<GatewayEventHelloPayload>>(reader);
                        foreach (var server in hello.Payload.ConnectedServers)
                            Logger.Debug($"Connected to Gateway server {server}");
                        Logger.Info($"Set Heartbeat interval to {hello.Payload.Heartbeat}");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public void Start(CancellationToken cancellationToken)
        {
            _connectTask.GetAwaiter().GetResult();

            while (!cancellationToken.IsCancellationRequested)
                ReceiveMessageAsync(cancellationToken).GetAwaiter().GetResult();

            while (!_streamPool.IsEmpty)
                _streamPool.Get().Dispose();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}