using Newtonsoft.Json;

namespace TanakaShoji.Discord.Contracts.Gateway
{
    [JsonObject]
    public class GatewayHelloPayload
    {
        [JsonProperty(PropertyName = "_trace", Required = Required.Default)]
        public string[] ConnectedServers;

        [JsonProperty(PropertyName = "heartbeat_interval", Required = Required.Always)]
        public int Heartbeat;
    }
}