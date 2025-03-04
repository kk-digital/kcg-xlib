using Microsoft.AspNetCore.Builder;

namespace UtilityHttpServer;

public class HttpServer
{
    private WebApplication _app;
    
    public HttpServer(WebApplication newApp)
    {
        _app = newApp;
    }
    
    public RouteHandlerBuilder AddGetEndPoint(string route, Delegate handler)
    {
        return _app.MapGet(route, handler);
    }

    public RouteHandlerBuilder AddPostEndPoint(string route, Delegate handler)
    {
        return _app.MapPost(route, handler);
    }

    public RouteHandlerBuilder AddPutEndPoint(string route, Delegate handler)
    {
        return _app.MapPut(route, handler);
    }

    public RouteHandlerBuilder AddDeleteEndPoint(string route, Delegate handler)
    {
        return _app.MapDelete(route, handler);
    }

    public RouteHandlerBuilder AddPatchEndPoint(string route, Delegate handler)
    {
        return _app.MapPatch(route, handler);
    }

    public IApplicationBuilder UseMiddleware<TMiddleware>()
    {
        return _app.UseMiddleware<TMiddleware>();
    }

    public void Run()
    {
        _app.Run();
    }
}
