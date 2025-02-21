
namespace UtilityHttpRequestManager;

/// <summary>
/// Tracks the status of HTTP requests.
/// </summary>
public class HttpRequestStatus
{
    public HttpRequest Request { get; set; }
    public string Status { get; set; }
    public DateTime SentTime { get; set; }
    public DateTime ResponseReceivedTime { get; set; }
}