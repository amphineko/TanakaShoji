using Newtonsoft.Json;

namespace TanakaShoji.Discord.Contracts.Gateway
{
    [JsonObject]
    public class GatewayHello : PayloadGatewayMessage<GatewayHelloPayload>
    {
    }
}