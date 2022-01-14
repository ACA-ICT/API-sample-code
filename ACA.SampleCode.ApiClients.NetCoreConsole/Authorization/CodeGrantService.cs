using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace ACA.SampleCode.ApiClients.ConsoleNET5.Authorization
{
    /// <summary>
    /// Class is responsible for logging and receiving granted authorization code in oauth2 code flow.
    /// </summary>
    /// <remarks>
    /// To work for console application, it uses an instance of HttpListener, which temporarly listens for code redirect callback from authorization server.
    /// </remarks>
    public class CodeGrantService
    {
        private const string AuthorizeUrlPattern = "{0}auth/authorize?response_type=code&client_id={1}&redirect_uri={2}&scope={3}&state={4}";

        private readonly Uri _acaPortalBaseUrl;
        private readonly string _clientId;
        private readonly Uri _redirectUri;
        private readonly string _scope;

        public CodeGrantService(Uri acaPortalBaseUrl, string clientId, Uri redirectUri, string scope)
        {
            if (acaPortalBaseUrl is null) 
                throw new ArgumentNullException(nameof(acaPortalBaseUrl));

            if (string.IsNullOrEmpty(clientId)) 
                throw new ArgumentException($"'{nameof(clientId)}' cannot be null or empty.", nameof(clientId));

            if (redirectUri is null) 
                throw new ArgumentNullException(nameof(redirectUri));

            if (string.IsNullOrEmpty(scope)) 
                throw new ArgumentException($"'{nameof(scope)}' cannot be null or empty.", nameof(scope));

            if (!HttpListener.IsSupported)
                throw new PlatformNotSupportedException("This class uses an instance of HttpListener, which isn't supported on given machine.");

            _acaPortalBaseUrl = acaPortalBaseUrl;
            _clientId = clientId;
            _redirectUri = redirectUri;
            _scope = scope;
        }

        /// <summary>
        /// Opens ACA Portal authorization page to login user and retrieve authorization code from redirect request when user is logged in correctly.
        /// </summary>
        /// <returns>Task with authorization code as result.</returns>
        public async Task<string> GrantCodeAsync()
        {
            var state = Guid.NewGuid().ToString("N");

            var authorizeUrl = string.Format(
                AuthorizeUrlPattern, 
                _acaPortalBaseUrl,
                Uri.EscapeDataString(_clientId),
                Uri.EscapeDataString(_redirectUri.AbsoluteUri),
                Uri.EscapeDataString(_scope),
                Uri.EscapeDataString(state));
            
            // Create a listener.
            HttpListener listener = new HttpListener();

            // Add authorization code callback URI prefix.
            listener.Prefixes.Add(_redirectUri.AbsoluteUri);

            // Start listener in asynchronous mode
            listener.Start();

            // Open browser with Portal authorization server page to login and get code
            // Note that UseShellExecute = true is required for .NET Core console application to work.
            // It should not be used in Universal Windows Platform (UWP) applications though, since it will throw error.
            var browserProcess = Process.Start(new ProcessStartInfo(authorizeUrl) { UseShellExecute = true });

            // Wait for code challenge redirect uri request
            var context = await listener.GetContextAsync();

            // Obtain state and code from request object.
            HttpListenerRequest request = context.Request;
            var callbackState = request.QueryString["state"];

            if (callbackState != state)
            {
                throw new Exception("State value from code challenge redirect uri is invalid.");
            }

            var code = request.QueryString["code"];

            // Set response to NoContent
            HttpListenerResponse response = context.Response;
            response.StatusCode = (int) HttpStatusCode.NoContent;
            response.ContentLength64 = 0;
            response.OutputStream.Close();

            if (browserProcess != null)
            {
                // should close browser with authorization page if it was opened in new window, but will not close tab in exisisting window
                browserProcess.Kill();
            }

            // Close listener
            listener.Close();

            return code;
        }
    }
}
