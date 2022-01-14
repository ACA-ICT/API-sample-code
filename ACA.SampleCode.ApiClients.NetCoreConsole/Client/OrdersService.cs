
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ACA.SampleCode.ApiClients.ConsoleNET5.Authorization;

namespace ACA.SampleCode.ApiClients.ConsoleNET5.Client
{
    /// <summary>
    /// Sample class providing method to GET single order overview record from ACA API.
    /// </summary>
    public class OrdersService
    {
        private const string OrderEndpointsPath = "v1/orders/";

        private readonly HttpClient _apiClient;

        public OrdersService(Uri acaApiBaseUrl, AccessToken accessToken, string languageCode)
        {
            if (acaApiBaseUrl is null) 
                throw new ArgumentNullException(nameof(acaApiBaseUrl));

            if (accessToken is null) 
                throw new ArgumentNullException(nameof(accessToken));

            // setup common http client for orders endpoints
            _apiClient = new HttpClient();

            _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiClient.DefaultRequestHeaders.Add("cache-control", "no-cache");
            _apiClient.BaseAddress = new Uri(acaApiBaseUrl, OrderEndpointsPath);

            if (!string.IsNullOrWhiteSpace(languageCode))
            {
                _apiClient.DefaultRequestHeaders.Add("language-code", languageCode);
            }
        }

        public async Task<string> GetOrderOverviewJsonAsync(int orderId)
        {
            var tokenResponse = await _apiClient.GetAsync($"{orderId}");

            if (tokenResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception($"Api call was unauthorized.");
            }

            if (tokenResponse.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception($"Given order was not found.");
            }

            if (tokenResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Invalid response status: {tokenResponse.StatusCode}");
            }

            var jsonToken = await tokenResponse.Content.ReadAsStringAsync(CancellationToken.None);

            return jsonToken;
        }
    }
}
