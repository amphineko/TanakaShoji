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
        private static async Task Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();

            var client = new HttpClient("Bot", "NTU4ODkyNDUxOTk0MTQwNjc0.D3dk0A.gl3stOVNWpqZJgS_pRhTpjyGKYg");
            var currentUser = client.GetCurrentUser().Result;
            Console.WriteLine($"Authorized as {currentUser}");

            const bool isBot = true;
            var gatewayEndpoint = isBot ? await client.GetGatewayBot() : await client.GetGateway();
            logger.Info($"Assigned Gateway WebSocket: {gatewayEndpoint.Url}");
            if (isBot)
                logger.Info(
                    $"Gateway Quota: {gatewayEndpoint.SessionStartLimit.Remaining}/{gatewayEndpoint.SessionStartLimit.Total}, " +
                    $"reset after {gatewayEndpoint.SessionStartLimit.Expire}ms");

            var wsEndpoint = await client.GetGateway();
            Console.WriteLine($"Connecting to Gateway endpoint {wsEndpoint}");
            var gateway = new GatewayClient(wsEndpoint.Url, "Bot",
                "NTU4ODkyNDUxOTk0MTQwNjc0.D3dk0A.gl3stOVNWpqZJgS_pRhTpjyGKYg");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}