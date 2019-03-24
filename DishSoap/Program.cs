using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using TanakaShoji.Discord.Gateway;
using TanakaShoji.Discord.HTTP;

namespace DishSoap
{
    internal class Program
    {
        private const bool IsBot = true;
        private const string TokenType = "Bot";
        private const string Token = "<YOUR_TOKEN_HERE>";

        private static async Task Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();

            var client = new HttpClient(TokenType, Token);
            var currentUser = client.GetCurrentUser().Result;
            Console.WriteLine($"Authorized as {currentUser}");

            var gatewayEndpoint = IsBot ? await client.GetGatewayBot() : await client.GetGateway();
            logger.Info($"Assigned Gateway WebSocket: {gatewayEndpoint.Url}");
            if (IsBot)
                logger.Info(
                    $"Gateway Quota: {gatewayEndpoint.SessionStartLimit.Remaining}/{gatewayEndpoint.SessionStartLimit.Total}, " +
                    $"reset after {gatewayEndpoint.SessionStartLimit.Expire}ms");

            var wsEndpoint = await client.GetGateway();
            Console.WriteLine($"Connecting to Gateway endpoint {wsEndpoint}");
            var gateway = new GatewayClient(wsEndpoint.Url, TokenType, Token);

            Thread.Sleep(Timeout.Infinite);
        }
    }
}