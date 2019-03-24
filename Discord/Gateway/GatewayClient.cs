using System;
using System.Threading;
using NLog;
using TanakaShoji.Discord.Gateway.WebSocket;

namespace TanakaShoji.Discord.Gateway
{
    public class GatewayClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();

        public GatewayClient(string endpoint, string tokenType, string token)
        {
            var socket = new WebSocketClient(new Uri(endpoint), tokenType, token);
            new Thread(() => socket.Start(_cancel.Token)).Start();
        }
    }
}