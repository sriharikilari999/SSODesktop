using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Identity.Client;

namespace WpfSsoApp
{
    public partial class MainWindow : Window
    {
        private const string ClientId = "your-client-id"; // Replace with Azure App Registration Client ID
        private const string TenantId = "your-tenant-id"; // Replace with your Tenant ID
        private static readonly string[] Scopes = { "User.Read" }; // Permissions (e.g., Microsoft Graph API)
        private const string RedirectUri = "http://localhost"; // Redirect URI for public client apps

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
                .WithRedirectUri(RedirectUri) // System browser uses this URI
                .Build();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Acquire Token using system browser
                var authResult = await AcquireTokenInteractiveAsync();
                ResultTextBlock.Text = $"Welcome, {authResult.Account.Username}!\nAccess Token: {authResult.AccessToken}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Authentication failed: {ex.Message}";
            }
        }

        private async Task<AuthenticationResult> AcquireTokenInteractiveAsync()
        {
            return await _msalClient.AcquireTokenInteractive(Scopes)
                                    .WithSystemWebViewOptions(new SystemWebViewOptions
                                    {
                                        OpenBrowserAsync = SystemBrowserLauncher // Launch system browser
                                    })
                                    .ExecuteAsync();
        }

        private async Task SystemBrowserLauncher(Uri uri)
        {
            // Open the system browser to the provided URI
            await Task.Run(() => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = uri.ToString(),
                UseShellExecute = true // Ensures it opens in the default system browser
            }));
        }
    }
}
