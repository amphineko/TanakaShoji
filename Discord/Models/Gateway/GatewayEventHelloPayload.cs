using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace TanakaShoji.Discord.Models.Gateway
{
    [JsonObject]
    [DataContract]
    public class GatewayEventHelloPayload
    {
        [JsonProperty(PropertyName = "_trace", Required = Required.Always)]
        [DataMember(Name = "_trace", IsRequired = true)]
        public string[] ConnectedServers;

        [JsonProperty(PropertyName = "heartbeat_interval", Required = Required.Always)]
        [DataMember(Name = "heartbeat_interval", IsRequired = true)]
        public int Heartbeat;
    }
}