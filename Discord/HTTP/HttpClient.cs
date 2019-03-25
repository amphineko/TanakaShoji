using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using TanakaShoji.Discord.Contracts;
using TanakaShoji.Discord.Contracts.Resources.Users;

namespace TanakaShoji.Discord.HTTP
{
    using NetHttpClient = System.Net.Http.HttpClient;

    public class HttpClient
    {
        private static readonly Uri ApiEndpoint = new Uri("https://discordapp.com/api/v6/");
        private static readonly ProductInfoHeaderValue UserAgent;

        private readonly NetHttpClient _client = new NetHttpClient();

        static HttpClient()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var product = assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            UserAgent = new ProductInfoHeaderValue(product, version);
        }

        public HttpClient(string tokenType, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);
            _client.DefaultRequestHeaders.UserAgent.Add(UserAgent);
        }

        public async Task<User> GetCurrentUser()
        {
            return await GetUser(@"@me");
        }

        public async Task<User> GetUser(string id)
        {
            var uri = new Uri(ApiEndpoint, $@"users/{id}");
            return await RequestObjectAsync<User>(uri, HttpMethod.Get);
        }

        public async Task<GatewayEndpoint> GetGateway()
        {
            // FIXME: cache gateway url
            var uri = new Uri(ApiEndpoint, "gateway");
            return await RequestObjectAsync<GatewayEndpoint>(uri, HttpMethod.Get);
        }

        public async Task<GatewayEndpoint> GetGatewayBot()
        {
            // FIXME: cache gateway url
            var uri = new Uri(ApiEndpoint, "gateway/bot");
            return await RequestObjectAsync<GatewayEndpoint>(uri, HttpMethod.Get);
        }

        private async Task<HttpResponseMessage> RequestAsync(Uri uri, HttpMethod method)
        {
            var request = new HttpRequestMessage(method, uri);
            return await _client.SendAsync(request);
        }

        private async Task<T> RequestObjectAsync<T>(Uri uri, HttpMethod method)
        {
            var response = await RequestAsync(uri, method);
            var stream = await response.Content.ReadAsStreamAsync();
            return (T) new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
        }
    }
}