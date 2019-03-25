using Newtonsoft.Json;

namespace TanakaShoji.Discord.Contracts
{
    [JsonObject]
    public class GatewayEndpoint
    {
        [JsonProperty(PropertyName = "session_start_limit")]
        public GatewaySessionStartLimit GatewaySessionStartLimit;

        [JsonProperty(PropertyName = "shards")]
        public int Shards;

        [JsonProperty(PropertyName = "url", Required = Required.Always)]
        public string Url;
    }
}