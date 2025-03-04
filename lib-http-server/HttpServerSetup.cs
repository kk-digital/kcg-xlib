using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Sockets;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using assert;


namespace UtilityHttpServer
{
    public class HttpServerSetup
    {
        public IConfigurationRoot appSettings;
        public WebApplicationBuilder builder;
        private int _connectionCounter = 0;
        private string _swaggerName;
        private string _swaggerVersion;
        private bool _useRazorViews;

        public HttpServerSetup(string[] args,
                               string configPath,
                               string configName,
                               string swaggerName,
                               string swaggerVersion,
                               string swaggerTitle,
                               string swaggerDescription,
                               string swaggerXml,
                               bool useRazorViews = false
                               )
        {
            _swaggerName = swaggerName;
            _swaggerVersion = swaggerVersion;

            builder = WebApplication.CreateBuilder(args);
            appSettings = CreateConfig(configPath, configName);

            SetLogging();
            SetSwaggerGen(swaggerName,
                          swaggerVersion,
                          swaggerTitle,
                          swaggerDescription,
                          swaggerXml);

            _useRazorViews = useRazorViews;
            if (useRazorViews)
            {
                // Add MVC services
                builder.Services.AddRazorPages();
                builder.Services.AddControllersWithViews();
            }

            AddCORSService();

            // Configure server to listen on the specified URL
            string serverUrl = appSettings.GetSection("Orchestration:Url")?.Value ?? throw new InvalidOperationException("Server URL is not configured.");

            Utils.Assert(!IsAddressInUse(serverUrl), $"Server URL {serverUrl} is already in use.");

            builder.WebHost.UseUrls(serverUrl);
        }

        public HttpServer InitServer(int maxConnections)
        {
            WebApplication app = builder.Build();
            DefaultAppSetup(app, maxConnections);
            return new HttpServer(app);
        }

        private static bool IsAddressInUse(string url)
        {
            try
            {
                // Parse the URL to extract host and port
                var uri = new Uri(url);
                int port = uri.Port;

                // If port is not specified in URL, use default HTTP/HTTPS ports
                if (port == -1)
                {
                    port = uri.Scheme.ToLower() == "https" ? 443 : 80;
                }

                // Try to create a TCP listener on the port
                var listener = new TcpListener(IPAddress.Any, port);
                try
                {
                    listener.Start();
                    listener.Stop();
                    return false; // Port is available
                }
                finally
                {
                    listener.Server.Close();
                }
            }
            catch (SocketException)
            {
                return true; // Port is in use
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

        private void SetSwaggerGen(string name,
                                string version,
                                string title,
                                string description,
                                string xml)
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

                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xml));
                options.CustomSchemaIds(type => type.ToString());
            });
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
            string corsUrl = appSettings.GetSection("CORS:Url")?.Value ?? string.Empty;

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                builder =>
                {
                    builder.WithOrigins(corsUrl)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
        }

        private void DefaultAppSetup(WebApplication app, int maxConnections)
        {
            // Enable CORS
            app.UseCors("AllowSpecificOrigin");
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/{_swaggerVersion}/swagger.json", _swaggerName);
                options.RoutePrefix = "docs";
            });

            if (_useRazorViews)
            {
                // Add support for static files and enable routing for MVC
                app.UseStaticFiles();
                app.UseRouting();
                app.MapControllers();
                app.MapRazorPages();
            }
            
            // Enable Buffering for request body payload
            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
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

}
