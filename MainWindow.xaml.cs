using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Identity.Client;

namespace WpfSsoApp
{
    public partial class MainWindow : Window
    {
        private const string ClientId = "your-client-id"; // Azure App Registration's Application (Client) ID
        private const string TenantId = "your-tenant-id"; // Azure App Registration's Directory (Tenant) ID
        private static readonly string[] Scopes = { "User.Read" }; // Permissions requested
        private const string RedirectUri = "http://localhost:5000"; // Redirect URI

        private static IPublicClientApplication _msalClient;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMsalClient();
        }

        private void InitializeMsalClient()
        {
            _msalClient = PublicClientApplicationBuilder
                .Create(ClientId)
                .WithAuthority(AzureCloudInstance.AzurePublic, TenantId)
                .WithRedirectUri(RedirectUri)
                .Build();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string code = await GetAuthorizationCodeAsync();
                if (!string.IsNullOrEmpty(code))
                {
                    var authResult = await GetAccessTokenAsync(code);
                    ResultTextBlock.Text = $"Welcome, {authResult.Account.Username}! Access Token acquired.";
                }
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Authentication failed: {ex.Message}";
            }
        }

        private async Task<string> GetAuthorizationCodeAsync()
        {
            var listener = new HttpListenerHelper(RedirectUri);

            // Start the listener and capture the redirect URL
            string queryString = await listener.CaptureRedirectAsync();

            // Parse the query string to extract the 'code' parameter
            var queryParams = QueryStringParser.ParseQueryString(queryString);
            string code = queryParams["code"];

            if (!string.IsNullOrEmpty(code))
            {
                return code;
            }

            throw new Exception("Authorization code not found in the redirect URL.");
        }

        private async Task<AuthenticationResult> GetAccessTokenAsync(string code)
        {
            return await _msalClient.AcquireTokenByAuthorizationCode(Scopes, code).ExecuteAsync();
        }
    }
}
