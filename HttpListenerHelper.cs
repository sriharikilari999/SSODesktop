using System.IO;
using System.Net;
using System.Threading.Tasks;

public class HttpListenerHelper
{
    private readonly string _redirectUri;

    public HttpListenerHelper(string redirectUri)
    {
        _redirectUri = redirectUri;
    }

    public async Task<string> CaptureRedirectAsync()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add(_redirectUri + "/");
        listener.Start();

        try
        {
            var context = await listener.GetContextAsync();
            string queryString = context.Request.Url.Query;

            // Respond to the browser (optional)
            using var writer = new StreamWriter(context.Response.OutputStream);
            writer.WriteLine("<html><body>Authentication complete. You may close this window.</body></html>");
            context.Response.Close();

            return queryString;
        }
        finally
        {
            listener.Stop();
        }
    }
}
