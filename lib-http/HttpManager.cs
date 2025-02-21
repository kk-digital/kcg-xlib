
namespace UtilityHttpRequestManager;
    
/// <summary>
/// Manages HTTP requests.
/// </summary>
public class HttpManager
{
// properties
public Queue<HttpRequest> Requests { get; set; }
//===================================================================================
// constructor
public HttpManager()
{
    Requests = new Queue<HttpRequest>();
}
// methods
//===================================================================================
void AddRequest(HttpRequest request)
{
    Requests.Enqueue(request);
}
//===================================================================================
Queue<HttpRequest> GetPendingRequests()
{
    return Requests;
}
//===================================================================================
}
    