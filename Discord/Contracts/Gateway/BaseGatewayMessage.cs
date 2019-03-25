using Newtonsoft.Json;

namespace TanakaShoji.Discord.Contracts.Gateway
{
    [JsonObject]
    public class BaseGatewayMessage
    {
        [JsonProperty(PropertyName = "op", Required = Required.Always)]
        public GatewayOpCode OpCode;
    }
}