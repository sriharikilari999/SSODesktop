using System.Collections.Specialized;
using System.Web;

public class QueryStringParser
{
    public static NameValueCollection ParseQueryString(string query)
    {
        return HttpUtility.ParseQueryString(query);
    }
}
