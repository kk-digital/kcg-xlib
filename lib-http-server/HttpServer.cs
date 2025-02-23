using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using System.Reflection;


namespace HttpServer;

public struct HttpServerSetup
{
    public string[] args;
    public IConfigurationRoot appSettings;
    public string swaggerName;
    public string swaggerVersion;
    public string swaggerTitle;
    public string swaggerDescription;
    public Type[] scopedServices;
    public int maxConnections;
    public Type[] middlewares;

}

public class HttpServer
{
    private readonly WebApplicationBuilder _builder;
    public WebApplication app;
    private IConfigurationRoot _appSettings;
    private int _connectionCounter = 0;  

    public HttpServer(HttpServerSetup serverSetup)
    {
        _builder = WebApplication.CreateBuilder(serverSetup.args);
        SetConfig(serverSetup.appSettings);
        SetLogging();
        SetSwaggerGen(serverSetup.swaggerName,
                      serverSetup.swaggerVersion,
                      serverSetup.swaggerTitle,
                      serverSetup.swaggerDescription);

        foreach (var service in serverSetup.scopedServices)
        {
            AddScopedService(service);
        }

        AddCORSService();
        BuildServer(serverSetup.maxConnections);

        foreach (var middleware in serverSetup.middlewares)
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

    public void AddEndPoint(HttpMethod httpMethod, string route, Delegate handler)
    {
        if (app == null)
        {
            return;
        }

        if (httpMethod == HttpMethod.Get)
        {
            app.MapGet(route, handler);
        }
        else if (httpMethod == HttpMethod.Post)
        {
            app.MapPost(route, handler);
        }
        else if (httpMethod == HttpMethod.Put)
        {
            app.MapPut(route, handler);
        }
        else if (httpMethod == HttpMethod.Delete)
        {
            app.MapDelete(route, handler);
        }
        else
        {
            throw new NotSupportedException($"HTTP method {httpMethod} is not supported.");
        }
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
