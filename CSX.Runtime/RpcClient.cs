using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CSX.Runtime
{
    public static class RpcClient
    {
        // TODO: Configuration for BaseAddress
        private static readonly HttpClient _client = new HttpClient { BaseAddress = new Uri("http://localhost:5200") };

        public static async Task CallAsync(string component, string method, params object[]? args)
        {
            Console.WriteLine($"[RpcClient] POST /_rpc/{component}/{method}");
            try
            {
                var response = await _client.PostAsJsonAsync($"/_rpc/{component}/{method}", args ?? Array.Empty<object>());
                response.EnsureSuccessStatusCode();
                Console.WriteLine("[RpcClient] Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RpcClient] Error: {ex.Message}");
            }
        }
    }
}
