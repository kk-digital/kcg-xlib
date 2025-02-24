using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using System.Reflection;


namespace UtilityHttpServer;

public class HttpServerConfiguration
{
    public required string[] args;
    public required string configPath;
    public required string configName;
    public required string swaggerName;
    public required string swaggerVersion;
    public required string swaggerTitle;
    public required string swaggerDescription;
    public required int maxConnections;
}

public class HttpServer
{
    public WebApplicationBuilder builder;
    public WebApplication app;
    public IConfigurationRoot appSettings;
    private int _connectionCounter = 0;  
    private int _maxConnections = 0;
    private HttpServerConfiguration _serverConfig;

    public HttpServer(HttpServerConfiguration serverConfig)
    {
        _serverConfig = serverConfig;
        builder = WebApplication.CreateBuilder(serverConfig.args);
        appSettings = CreateConfig(serverConfig.configPath, serverConfig.configName);
        SetLogging();
        SetSwaggerGen(serverConfig.swaggerName,
                      serverConfig.swaggerVersion,
                      serverConfig.swaggerTitle,
                      serverConfig.swaggerDescription);

        AddCORSService();
        _maxConnections = serverConfig.maxConnections;

    }

    public void BuildApp()
    {
        if (app == null)
        {
            app = builder.Build();
            DefaultAppSetup(_maxConnections);
        }
    }


    private IConfigurationRoot CreateConfig(string configPath, string configName)
    {
        string configBasePath = Path.Combine(builder.Environment.ContentRootPath, configPath);
        IConfigurationRoot appSettings = new ConfigurationBuilder()
            .SetBasePath(configBasePath)
            .AddJsonFile($"{configName}.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"{configName}.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
            .Build();

        return appSettings;
    }

    private void SetLogging()
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConfiguration(appSettings.GetSection("Logging"));
        builder.Logging.AddConsole();
    }


    private void SetSwaggerGen(string name, string version, string title, string description)
    { 
        builder.Services.AddHttpContextAccessor(); // Register IHttpContextAccessor
        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        // Add services to the container.
        // Add Authorization Services
        builder.Services.AddAuthorization();
        builder.Services.AddSwaggerGen(options =>
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

    /*
    Configures and adds CORS (Cross-Origin Resource Sharing) services to the application.
    Allows requests only from the specified Web UI URL defined in the application settings.
    */
    private void AddCORSService()
    {
        if (appSettings == null)
        {
            return;
        }
        
        // Add CORS services
        string webUiUrl = appSettings.GetSection("WebUI:Url").Value;

        builder.Services.AddCors(options =>
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
            options.SwaggerEndpoint($"/swagger/{_serverConfig.swaggerVersion}/swagger.json", _serverConfig.swaggerName);
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

}
