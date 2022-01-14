using System;
using System.Text.Json.Serialization;

namespace ACA.SampleCode.ApiClients.ConsoleNET5.Authorization
{
    public sealed class AccessToken
    {
        [JsonPropertyName("access_token")]
        public string Token { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        [JsonConverter(typeof(TokenExpireDateJsonConverter))]
        public DateTime ExpiresAtUtc { get; set; }
    }
}
