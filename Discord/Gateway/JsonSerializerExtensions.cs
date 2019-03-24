using System.IO;
using Newtonsoft.Json;

namespace TanakaShoji.Discord.Gateway
{
    public static class JsonSerializerExtensions
    {
        public static T Deserialize<T>(this JsonSerializer that, StreamReader streamReader)
        {
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                jsonReader.CloseInput = false;
                return that.Deserialize<T>(jsonReader);
            }
        }

        public static T Deserialize<T>(this JsonSerializer that, Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                return that.Deserialize<T>(streamReader);
            }
        }
    }
}