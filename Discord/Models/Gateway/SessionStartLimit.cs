using System.Runtime.Serialization;

namespace TanakaShoji.Discord.Models.Gateway
{
    public class SessionStartLimit
    {
        [DataMember(Name = "reset_after", IsRequired = true)]
        public int Expire;

        [DataMember(Name = "remaining", IsRequired = true)]
        public int Remaining;

        [DataMember(Name = "total", IsRequired = true)]
        public int Total;
    }
}