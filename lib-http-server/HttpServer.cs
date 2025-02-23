using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using System.Reflection;


namespace HttpServer;

public class HttpServerConfiguration
{
    public required string[] args;
    public required IConfigurationRoot appSettings;
    public required string swaggerName;
    public required string swaggerVersion;
    public required string swaggerTitle;
    public required string swaggerDescription;
    public required Type[] scopedServices;
    public required int maxConnections;
    public required Type[] middlewares;

}

public class HttpServer
{
    private readonly WebApplicationBuilder _builder;
    public WebApplication app;
    private IConfigurationRoot _appSettings;
    private int _connectionCounter = 0;  

    public HttpServer(HttpServerConfiguration serverConfig)
    {
        _builder = WebApplication.CreateBuilder(serverConfig.args);
        SetConfig(serverConfig.appSettings);
        SetLogging();
        SetSwaggerGen(serverConfig.swaggerName,
                      serverConfig.swaggerVersion,
                      serverConfig.swaggerTitle,
                      serverConfig.swaggerDescription);

        foreach (var service in serverConfig.scopedServices)
        {
            AddScopedService(service);
        }

        AddCORSService();
        BuildServer(serverConfig.maxConnections);

        foreach (var middleware in serverConfig.middlewares)
        {
            UseMiddleware(middleware);
        }
    }

    private void BuildServer(int maxConnections)
    {
        if (app == null)
        {
            app = _builder.Build();
            DefaultAppSetup(maxConnections);
        }
    }


    private void SetConfig(IConfigurationRoot appSettings)
    {
        _appSettings = appSettings;
        _builder.Services.AddSingleton<IConfiguration>(_appSettings);
    }

    private void SetLogging()
    {
        _builder.Logging.ClearProviders();
        _builder.Logging.AddConfiguration(_appSettings.GetSection("Logging"));
        _builder.Logging.AddConsole();
    }


    private void SetSwaggerGen(string name, string version, string title, string description)
    { 
        _builder.Services.AddHttpContextAccessor(); // Register IHttpContextAccessor
        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        _builder.Services.AddEndpointsApiExplorer();
        // Add services to the container.
        // Add Authorization Services
        _builder.Services.AddAuthorization();
        _builder.Services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.SwaggerDoc(name: name, new OpenApiInfo
            {
                Version = version,
                Title = title,
                Description = description
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            options.CustomSchemaIds(type => type.ToString());
        });
    }

    private void AddScopedService(Type serviceType)
    {
        var method = typeof(ServiceCollectionServiceExtensions)
            .GetMethod("AddScoped", 1, new[] { typeof(IServiceCollection) })!
            .MakeGenericMethod(serviceType);

        method.Invoke(null, new object[] { _builder.Services });
    }

    public void AddGetEndPoint(string route, Delegate handler)
    {
        app.MapGet(route, handler);
    }
    
    public void AddPostEndPoint(string route, Delegate handler)
    {
        app.MapPost(route, handler);
    }
    
    public void AddPutEndPoint(string route, Delegate handler)
    {
        app.MapPut(route, handler);
    }
    
    public void AddDeleteEndPoint(string route, Delegate handler)
    {
        app.MapDelete(route, handler);
    }

    private void AddCORSService()
    {
        if (_appSettings == null)
        {
            return;
        }
        
        // Add CORS services
        string webUiUrl = _appSettings.GetSection("WebUI:Url").Value;

        _builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
            builder =>
            {
                builder.WithOrigins(webUiUrl)
                   .AllowAnyMethod()
                   .AllowAnyHeader();
            });
        });
    }

    private void DefaultAppSetup(int maxConnections)
    {
        // Enable CORS
        app.UseCors("AllowSpecificOrigin");
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = "docs";
        });

        // Middleware to track current number of connections
        app.Use(async (context, next) =>
        {
            int currentConnections = Interlocked.Increment(ref _connectionCounter); // Increment when a request starts

            if (currentConnections > maxConnections)
            {
                Interlocked.Decrement(ref _connectionCounter); // Decrement when a request ends
                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsync("Server busy. Try again later.");
                return;
            }

            try
            {
                await next.Invoke();
            }
            finally
            {
                Interlocked.Decrement(ref _connectionCounter);
            }
        });
        app.UseRouting();
    }

    private void UseMiddleware(Type middlewareType)
    {
        var method = typeof(IApplicationBuilder)
            .GetMethod("UseMiddleware", 1, Type.EmptyTypes)!
            .MakeGenericMethod(middlewareType);

        method.Invoke(app, null);
    }


}
