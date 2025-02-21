namespace UtilityHttpRequestManager;

/// <summary>
/// Represents an HTTP response.
/// </summary>
public class HttpResponse
{
    public int StatusCode { get; set; }
    public string Headers { get; set; }
    public string Body { get; set; }
}