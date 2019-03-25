using Newtonsoft.Json;

namespace TanakaShoji.Discord.Contracts.Resources.Users
{
    [JsonObject]
    public class User
    {
        [JsonProperty(PropertyName = "avatar", Required = Required.AllowNull)]
        public string Avatar;

        [JsonProperty(PropertyName = "discriminator", Required = Required.Always)]
        public string Discriminator;

        [JsonProperty(PropertyName = "email", Required = Required.Default)]
        public string Email;

        [JsonProperty(PropertyName = "flags", Required = Required.Default)]
        public int Flags;

        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Id;

        [JsonProperty(PropertyName = "bot", Required = Required.Default)]
        public bool IsBot;

        [JsonProperty(PropertyName = "mfa_enabled", Required = Required.Default)]
        public bool IsMfaEnabled;

        [JsonProperty(PropertyName = "locale", Required = Required.Default)]
        public string Locale;

        [JsonProperty(PropertyName = "PremiumType", Required = Required.Default)]
        public int PremiumType;

        [JsonProperty(PropertyName = "username", Required = Required.Always)]
        public string Username;

        [JsonProperty(PropertyName = "verified", Required = Required.Default)]
        public bool Verified;

        public override string ToString()
        {
            return $"{Username}#{Discriminator} ({Id})";
        }
    }
}