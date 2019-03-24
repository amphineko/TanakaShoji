using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace TanakaShoji.Discord.Models.Gateway
{
    [JsonObject]
    public class BaseGatewayMessage
    {
        [JsonProperty(PropertyName = "op", Required = Required.Always)] [DataMember(Name = "op", IsRequired = true)]
        public GatewayEventOpCode OpCode;
    }

    [JsonObject]
    [DataContract]
    public class GatewayEvent<T> : BaseGatewayMessage
    {
        [JsonProperty(PropertyName = "t")] [DataMember(Name = "t", IsRequired = false)]
        public string EventName;

        [JsonProperty(PropertyName = "d", Required = Required.AllowNull)] [DataMember(Name = "d", IsRequired = false)]
        public T Payload;

        [JsonProperty(PropertyName = "s")] [DataMember(Name = "s", IsRequired = false)]
        public int? SequenceNumber;
    }


    public enum GatewayEventOpCode
    {
        Hello = 10
    }
}