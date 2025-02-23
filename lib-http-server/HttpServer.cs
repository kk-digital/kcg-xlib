using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using System.Reflection;


namespace HttpServer;

public class HttpServer
{
    private readonly WebApplicationBuilder _builder;
    public WebApplication app = null!;
    private IConfigurationRoot _appSettings = null!;
    private int _connectionCounter = 0;  

    public HttpServer(string[] args)
    {
        _builder = WebApplication.CreateBuilder(args);
    }

    public void BuildServer()
    {
        if (app == null)
        {
            app = _builder.Build();
            DefaultAppSetup();
        }
    }

    public void SetConfig(string configBasePath, string configFileName)
    {
        string env = _builder.Environment.EnvironmentName;

        if (_appSettings == null)
        {
            IConfigurationRoot _appSettings = new ConfigurationBuilder()
                .SetBasePath(configBasePath)
                .AddJsonFile($"{configFileName}.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"{configFileName}.{env}.json", optional: true, reloadOnChange: false)
                .Build();

            _builder.Services.AddSingleton<IConfiguration>(_appSettings);
            _builder.Logging.ClearProviders();
            _builder.Logging.AddConfiguration(_appSettings.GetSection("Logging"));
            _builder.Logging.AddConsole();
        }
    }
    public void SetConfig(IConfigurationRoot appSettings)
    {
        _builder.Services.AddSingleton<IConfiguration>(appSettings);
        
        _builder.Logging.ClearProviders();
        _builder.Logging.AddConfiguration(appSettings.GetSection("Logging"));
        _builder.Logging.AddConsole();
    }

    public void SetSwaggerGen(string name, string version, string title, string description)
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

    public void AddScopedService<TService>() where TService : class
    {
        _builder.Services.AddScoped<TService>();
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

    public void AddCORSService()
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

    public void DefaultAppSetup()
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

        int ConnectionCounter = 0;  // Variable to track active number of connections

        app.Use(async (context, next) =>
        {
            int maxConnections = 1000; // Example limit. hardcoded for testing..this can be read from a settings file
            int currentConnections = Interlocked.Increment(ref ConnectionCounter); // Increment when a request starts

            if (currentConnections > maxConnections)
            {
                Interlocked.Decrement(ref ConnectionCounter); // Decrement when a request ends
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
                Interlocked.Decrement(ref ConnectionCounter);
            }
        });
        app.UseRouting();
    }

    public void UseMiddleWare<TMiddleware>() where TMiddleware : class
    {
        app.UseMiddleware<TMiddleware>();
    }

}
