using Newtonsoft.Json;

namespace TanakaShoji.Discord.Contracts.Gateway
{
    [JsonObject]
    internal class GatewayHeartbeat : PayloadGatewayMessage<object>
    {
        public GatewayHeartbeat()
        {
            OpCode = GatewayOpCode.Heartbeat;
        }

        public GatewayHeartbeat(int? lastSequenceNumber) : this()
        {
            SequenceNumber = lastSequenceNumber;
        }
    }
}