namespace UtilityHttpRequestManager;

/// <summary>
/// Represents an HTTP request.
/// </summary>
public class HttpRequest
{
    public string Url { get; set; }
    public string Method { get; set; }
    public string Headers { get; set; }
    public string Body { get; set; }
    //===================================================================================
    void Send()
    {

    }
    //===================================================================================
}