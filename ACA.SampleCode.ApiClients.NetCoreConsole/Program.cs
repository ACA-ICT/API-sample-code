/*
ACA.WebAPI Sample C# Client usage

This NET5 console application shows example how to use Oauth2 authorization code flow provided by ACA.Portal Oauth server and how to call ACA WebAPI endpoint using access tokens.

Notes:
1) Code grant challenge requires redirect uri to be set for OAuth client and it must be over SSL. This sample is using HttpListener to temporary open listening port for this redirect uri.
HttpListener won't receive redirect request over SSL without binding SSL cert with port.
Create SSL cert for your particular application, add it to trusted certs store on local machine using certml snappin and bind your cert with given port. 
Example commands in PowerShell for creating self-signed cert and binding:

# Creating self-signed example cert for testing purposes - note that DnsName will be used as redirect uri (AuthRedirectUrl field in Program class) when granting code.
New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation "cert:\LocalMachine\My"

# Bind SSL cert to callback host name and port. CertHash is a thumprint from cert generated in previous step. AppId can be some random generated guid, to differentiate SSL binding.
netsh http add sslcert hostnameport=localhost:5018 certstorename=MY certhash="27d061ecd4867e744af6c5df1dacff276de19f21" appid="{45ecb54b-b013-4933-84af-9b942474b553}"

2) If you have System.Net.Http.HttpRequestException informing about something wrong with SSL cert, 
make sure your Portal / API SSL certs have no name mismatch and are added to trusted root (if you are having local ACA environment setup for testing purposes).
*/

using System;
using System.Text.Json;
using System.Threading.Tasks;

using ACA.SampleCode.ApiClients.ConsoleNET5.Authorization;
using ACA.SampleCode.ApiClients.ConsoleNET5.Client;

namespace ACA.SampleCode.ApiClients.ConsoleNET5
{
    internal class Program
    {
        /// <summary>
        /// [CHANGE_ME] Base address of ACA Portal, that provides oauth2 server capabillities.
        /// Given example value is for staging ACA environment. The correct production URL can be found in documentation. 
        /// </summary>
        private const string AcaPortalBaseAddress = "https://test-portal.acavzw.be";

        /// <summary>
        /// [CHANGE_ME] Base address of ACA Api.
        /// Given example value is for staging ACA environment. The correct production URL can be found in documentation. 
        /// </summary>
        private const string AcaApiBaseAddress = "https://test-api.acavzw.be";

        /// <summary>
        /// [CHANGE_ME] Your Oauth2 client identifier. 
        /// Given value must be added on ACA side.
        /// </summary>
        private const string ClientId = "testconsole";

        /// <summary>
        /// [CHANGE_ME] Your Oauth2 client secret. 
        /// Given value must be added on ACA side.
        /// </summary>
        private const string ClientSecret = "secret";

        /// <summary>
        /// [CHANGE_ME] Your Oauth2 code grant redirect url. 
        /// Given value must be added on ACA side.
        /// </summary>
        private const string AuthRedirectUrl = "https://urlOfYourApp";

        /// <summary>
        /// Scope of authorization. Default "api" value means full access. 
        /// Each Oauth2 client must have configured its available scopes on ACA side.
        /// </summary>
        private const string Scope = "api";

        static async Task Main(string[] args)
        {
            Console.WriteLine("ACA.WebAPI Sample C# Client usage");

            var acaPortalBaseUri = new Uri(AcaPortalBaseAddress);
            var acaApiBaseUri = new Uri(AcaApiBaseAddress);
            var redirectUri = new Uri(AuthRedirectUrl);

            Console.WriteLine("Getting authorization code");
            var codeGrantService = new CodeGrantService(acaPortalBaseUri, ClientId, redirectUri, Scope);
            var code = await codeGrantService.GrantCodeAsync();
            Console.WriteLine($"Code received: {code}");

            Console.WriteLine("Getting access token");
            var accessTokenService = new AccessTokenService(acaPortalBaseUri, ClientId, redirectUri, ClientSecret);
            var accessToken = await accessTokenService.GetAccessTokenAsync(code);
            Console.WriteLine($"Token data received: {JsonSerializer.Serialize(accessToken)}");

            int exampleOrderId = 100;
            Console.WriteLine($"Calling example api GET OrderOverview endpoint (OrderId = {exampleOrderId})");
            var ordersService = new OrdersService(acaApiBaseUri, accessToken, SupportedLanguageCodes.Dutch);
            try
            {
                var json = await ordersService.GetOrderOverviewJsonAsync(exampleOrderId);
                Console.WriteLine("Response:");
                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}