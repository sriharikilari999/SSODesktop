private async Task<string> GetAuthorizationCodeAsync()
{
    string redirectUri = "http://localhost:5000"; // Ensure this matches your Azure AD App Registration
    var listener = new HttpListenerHelper(redirectUri);

    // Start the listener and capture the redirected query string
    string queryString = await listener.CaptureRedirectAsync();

    // Parse the query string to extract the 'code' parameter
    string code = QueryStringParser.GetQueryStringValue(queryString, "code");

    if (!string.IsNullOrEmpty(code))
    {
        MessageBox.Show($"Authorization Code: {code}");
        return code;
    }

    throw new Exception("Authorization code not found in the redirect URL.");
}
private async void LoginButton_Click(object sender, RoutedEventArgs e)
{
    try
    {
        string code = await GetAuthorizationCodeAsync();
        MessageBox.Show($"Authorization Code: {code}");
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}");
    }
}using System.Collections.Specialized;
using System.Web;

public class QueryStringParser
{
    public static string GetQueryStringValue(string query, string key)
    {
        NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
        return queryParams[key]; // Returns the value of the specified key (e.g., "code")
    }
}

