using System.Runtime.Serialization;

namespace TanakaShoji.Discord.Models
{
    [DataContract]
    public class User
    {
        [DataMember(Name = "avatar", IsRequired = false)]
        public string Avatar;

        [DataMember(Name = "discriminator", IsRequired = true)]
        public string Discriminator;

        [DataMember(Name = "email", IsRequired = false)]
        public string Email;

        [DataMember(Name = "flags", IsRequired = false)]
        public int Flags;

        [DataMember(Name = "id", IsRequired = true)]
        public string Id;

        [DataMember(Name = "bot", IsRequired = false)]
        public bool IsBot;

        [DataMember(Name = "mfa_enabled", IsRequired = false)]
        public bool IsMfaEnabled;

        [DataMember(Name = "locale", IsRequired = false)]
        public string Locale;

        [DataMember(Name = "PremiumType", IsRequired = false)]
        public int PremiumType;

        [DataMember(Name = "username", IsRequired = true)]
        public string Username;

        [DataMember(Name = "verified", IsRequired = false)]
        public bool Verified;

        public override string ToString()
        {
            return $"{Username}#{Discriminator} ({Id})";
        }
    }
}