using Newtonsoft.Json;

namespace TanakaShoji.Discord.Contracts.Gateway
{
    public class PayloadGatewayMessage<T> : BaseGatewayMessage
    {
        [JsonProperty(PropertyName = "t", Required = Required.Default)]
        public string EventName;

        [JsonProperty(PropertyName = "d", Required = Required.Default)]
        public T Payload;

        [JsonProperty(PropertyName = "s", Required = Required.Default)]
        public int? SequenceNumber;
    }
}