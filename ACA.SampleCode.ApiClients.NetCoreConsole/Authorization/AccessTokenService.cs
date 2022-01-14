using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ACA.SampleCode.ApiClients.ConsoleNET5.Authorization
{
    /// <summary>
    /// Provides method for retrieving access tokens for ACA API.
    /// </summary>
    public class AccessTokenService
    {
        private const string TokenUrlPattern = "{0}auth/token";

        private readonly Uri _acaPortalBaseUrl;
        private readonly string _clientId;
        private readonly Uri _redirectUri;
        private readonly string _clientSecret;

        public AccessTokenService(Uri acaPortalBaseUrl, string clientId, Uri redirectUri, string clientSecret)
        {
            if (acaPortalBaseUrl is null)
                throw new ArgumentNullException(nameof(acaPortalBaseUrl));

            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException($"'{nameof(clientId)}' cannot be null or empty.", nameof(clientId));

            if (redirectUri is null) 
                throw new ArgumentNullException(nameof(redirectUri));

            if (string.IsNullOrEmpty(clientSecret)) 
                throw new ArgumentException($"'{nameof(clientSecret)}' cannot be null or empty.", nameof(clientSecret));

            _acaPortalBaseUrl = acaPortalBaseUrl;
            _clientId = clientId;
            _redirectUri = redirectUri;
            _clientSecret = clientSecret;
        }

        /// <summary>
        /// Gets access token data for authorizing requests sent to ACA API endpoints.
        /// </summary>
        /// <param name="grantedCode">Authorization code that was granted.</param>
        /// <returns>Task with access token data as result.</returns>
        public async Task<AccessToken> GetAccessTokenAsync(string grantedCode)
        {
            var client = new HttpClient();

            var authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("cache-control", "no-cache");

            var form = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"code", grantedCode},
                {"client_id", _clientId },
                {"redirect_uri", _redirectUri.AbsoluteUri }
            };

            var tokenResponse = await client.PostAsync(string.Format(TokenUrlPattern, _acaPortalBaseUrl.AbsoluteUri), new FormUrlEncodedContent(form));

            if (tokenResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Invalid auth token response status: {tokenResponse.StatusCode}");
            }

            var jsonToken = await tokenResponse.Content.ReadAsStringAsync(CancellationToken.None);
            var accessToken = JsonSerializer.Deserialize<AccessToken>(jsonToken);
            return accessToken;
        }
    }
}
