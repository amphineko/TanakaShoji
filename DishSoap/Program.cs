using System;
using System.Configuration;
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

            if (Configuration.Token == null)
                throw new ConfigurationErrorsException("Discord Bot token is not configured");

            var client = new HttpClient(Configuration.TokenType, Configuration.Token);
            var currentUser = client.GetCurrentUser().Result;
            Console.WriteLine($"Authorized as {currentUser}");

            var gatewayEndpoint = Configuration.IsBot ? await client.GetGatewayBot() : await client.GetGateway();
            logger.Info($"Assigned Gateway WebSocket: {gatewayEndpoint.Url}");
            if (Configuration.IsBot)
                logger.Info(
                    $"Gateway Quota: {gatewayEndpoint.GatewaySessionStartLimit.Remaining}/{gatewayEndpoint.GatewaySessionStartLimit.Total}, " +
                    $"reset after {gatewayEndpoint.GatewaySessionStartLimit.Expire}ms");

            var wsEndpoint = await client.GetGateway();
            Console.WriteLine($"Connecting to Gateway endpoint {wsEndpoint}");
            var gateway = new GatewayClient(wsEndpoint.Url, Configuration.TokenType, Configuration.Token);
            gateway.Start();

            Thread.Sleep(Timeout.Infinite);

            gateway.Stop();
        }
    }
}