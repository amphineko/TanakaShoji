using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using TanakaShoji.Discord.Contracts.Gateway;
using TanakaShoji.Discord.Gateway.WebSocket;

namespace TanakaShoji.Discord.Gateway
{
    public class GatewayClient : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly WebSocketClient _socket;

        private readonly CancellationTokenSource _socketCancellation = new CancellationTokenSource();

        private CancellationTokenSource _heartbeatCancellation;
        private Task _heartbeatTask;
        private Task _socketTask;

        private int? LastSequenceNumber;

        public GatewayClient(string endpoint, string tokenType, string token)
        {
            _socket = new WebSocketClient(new Uri(endpoint), tokenType, token);
            _socket.OnMessage += (stream, type) => Task.Run(() => HandleWebSocketMessage(stream, type));
        }

        public void Dispose()
        {
            _socketCancellation?.Dispose();
        }

        public event EventHandler OnClose;

        private void ConfigureHeartbeat(int interval)
        {
            _ = StopHeartbeatAsync();

            _heartbeatCancellation = new CancellationTokenSource();
            _heartbeatTask = StartHeartbeatAsync(_heartbeatCancellation.Token, interval);
        }

        private async Task StartHeartbeatAsync(CancellationToken cancellationToken, int interval)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await SendHeartbeatAsync(cancellationToken);
                Logger.Debug("Heartbeat sent");
                await Task.Delay(interval, cancellationToken);
            }
        }

        private async Task SendHeartbeatAsync(CancellationToken cancellationToken)
        {
            await SendObjectAsync(new GatewayHeartbeat(LastSequenceNumber), cancellationToken);
        }

        private async Task SendObjectAsync<T>(T obj, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream())
            using (var streamWriter = new StreamWriter(stream))
            using (var writer = new JsonTextWriter(streamWriter))
            {
                new JsonSerializer().Serialize(writer, obj, typeof(T));
                await _socket.SendMessageStreamAsync(stream, WebSocketMessageType.Text, cancellationToken);
            }
        }

        private void HandleTextMessage(MemoryStream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                stream.Position = 0;
                var baseMessage = new JsonSerializer().Deserialize<BaseGatewayMessage>(reader);

                switch (baseMessage.OpCode)
                {
                    case GatewayOpCode.Dispatch:
                        break;
                    case GatewayOpCode.Hello:
                        stream.Position = 0;
                        var hello = new JsonSerializer().Deserialize<GatewayHello>(reader).Payload;
                        foreach (var server in hello.ConnectedServers)
                            Logger.Debug($"Connected to server {server}");
                        Logger.Info("Connected to Gateway");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void HandleWebSocketMessage(MemoryStream stream, WebSocketMessageType type)
        {
            switch (type)
            {
                case WebSocketMessageType.Binary:
                    throw new NotImplementedException();
                case WebSocketMessageType.Text:
                    HandleTextMessage(stream);
                    break;
                case WebSocketMessageType.Close:
                    OnClose?.Invoke(this, null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Not implemented WebSocket message type");
            }
        }

        public void Start()
        {
            _socketTask = Task.Run(() => _socket.Start(_socketCancellation.Token));
        }

        public void Stop()
        {
            StopHeartbeatAsync().GetAwaiter().GetResult();

            _socketCancellation.Cancel();
            _socketTask.GetAwaiter().GetResult();
        }

        private async Task StopHeartbeatAsync()
        {
            _heartbeatCancellation?.Cancel();
            if (_heartbeatTask != null)
                await _heartbeatTask;
            _heartbeatCancellation?.Dispose();
        }
    }
}