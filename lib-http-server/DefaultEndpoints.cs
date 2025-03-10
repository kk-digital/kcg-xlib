using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

namespace UtilityHttpServer;

public static class DefaultEndpoints
{
    private static HttpServer _server = null;
    public static void RegisterEndpoints(HttpServer server)
    {
        _server = server;
        
        server.AddGetEndPoint("/status",
                () =>
                {
                    return new ServerStatus { Status = "OK", Timestamp = DateTime.UtcNow };
                })
            .WithTags("Status")
            .Produces<ServerStatus>(200)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Checks if the server is running",
                Description = "This endpoint is used to check if the server is running."
            });

        server.AddGetEndPoint("/status/pubkey",
                () =>
                {
                    return GetPubkey();
                })
            .WithTags("Status")
            .Produces<PubkeySession>(200)
            .Produces(404)
            .Produces(401)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Returns the public key of the server",
                Description = "This endpoint is used to return the public key of the server."
            });
    }
    
    private static IResult GetPubkey()
    {
        if (_server._pubkeySession == null || string.IsNullOrEmpty(_server._pubkeySession.Pubkey))
        {
            return Results.NotFound(new {Error = "Public key not defined in server." });
        }

        if (DateTime.UtcNow > _server._pubkeySession.ValidTimeEnd ||
            DateTime.UtcNow < _server._pubkeySession.ValidTimeStart)
        {
            return Results.Unauthorized();
        }
        
        var response = new PubkeyServer {Type = _server._pubkeySession.Type, 
                                         Pubkey = _server._pubkeySession.Pubkey };
        return Results.Ok(response);
    }
}