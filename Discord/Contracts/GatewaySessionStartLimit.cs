using System.Runtime.Serialization;

namespace TanakaShoji.Discord.Contracts
{
    public class GatewaySessionStartLimit
    {
        [DataMember(Name = "reset_after", IsRequired = true)]
        public int Expire;

        [DataMember(Name = "remaining", IsRequired = true)]
        public int Remaining;

        [DataMember(Name = "total", IsRequired = true)]
        public int Total;
    }
}