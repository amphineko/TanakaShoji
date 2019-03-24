using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace TanakaShoji.Discord.Models.Gateway
{
    [JsonObject]
    [DataContract]
    public class GatewayEndpoint
    {
        [JsonProperty(PropertyName = "session_start_limit")]
        [DataMember(Name = "session_start_limit", IsRequired = false)]
        public SessionStartLimit SessionStartLimit;

        [JsonProperty(PropertyName = "shards")] [DataMember(Name = "shards", IsRequired = false)]
        public int Shards;

        [JsonProperty(PropertyName = "url", Required = Required.Always)] [DataMember(Name = "url", IsRequired = true)]
        public string Url;
    }
}