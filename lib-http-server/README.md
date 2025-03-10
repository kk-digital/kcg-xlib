# lib-http-server Library Usage Guide

## Overview
The `lib-http-server` is a library designed to facilitate the creation of an HTTP server and API endpoint registration.

## Quick Start Guide

### 1. Server Initialization

```csharp
// Create a server setup with configuration
var serverSetup = new HttpServerSetup(
    args,                   // Command-line arguments
    configPath,             // Path to configuration
    configName,             // Configuration file name
    swaggerName,            // Swagger configuration
    swaggerVersion,         // Swagger version
    swaggerTitle,           // Swagger title
    swaggerDescription,     // Swagger description
    swaggerXmlPath          // Swagger XML documentation path
);

// Initialize the server
// Parameters:
// - maxConnections: Maximum concurrent connections
// - useDefaultEndpoints: Whether to include default status/pubkey endpoints
var httpServer = serverSetup.InitServer(
    maxConnections: 100,      // Maximum 100 concurrent connections
    useDefaultEndpoints: true // Include default status endpoints
);
```

### 2. Default Endpoints

The library automatically includes two default endpoints when `useDefaultEndpoints` is true:

- `/status`: Returns server status
    - Response: `{ status: "OK", timestamp: DateTime }`

- `/status/pubkey`: Returns server public key
    - Response: Depends on pubkey session configuration
    - Possible responses: 200 OK, 404 Not Found, 401 Unauthorized

### 3. Configuring Public Key

```csharp
var pubkeySession = new PubkeySession 
{
    Type = "RSA",
    Pubkey = "your-public-key-here",
    ValidTimeStart = DateTime.UtcNow,
    ValidTimeEnd = DateTime.UtcNow.AddHours(1)
};

httpServer.StartPubkeySession(pubkeySession);
```

### 4. Adding Custom Endpoints

```csharp
// Supports various HTTP methods
httpServer.AddGetEndPoint("/users", () => 
{
    return Results.Ok(userService.GetAllUsers());
});

httpServer.AddPostEndPoint("/users", (User newUser) => 
{
    var createdUser = userService.CreateUser(newUser);
    return Results.Created($"/user/{createdUser.Id}", createdUser);
});

// Other supported methods:
// - AddPutEndPoint
// - AddDeleteEndPoint
// - AddPatchEndPoint
```

### 5. Middleware

```csharp
// Add middleware
httpServer.UseMiddleware<CustomMiddleware>();
```

### 6. Running the Server

```csharp
// Start the server
httpServer.Run();
```

## Key Features

- Flexible HTTP server setup
- Automatic default endpoints
- Easy endpoint registration
- Middleware support
- Configurable public key management
- Swagger integration

## Configuration Options

The `HttpServerSetup` constructor allows extensive configuration:
- Command-line argument parsing
- Custom configuration loading
- Swagger documentation setup
- CORS configuration
- Logging setup

## Recommendations

1. Always specify `maxConnections`
2. Configure public key session when needed
3. Use default endpoints for basic health checks
4. Leverage built-in Swagger integration
5. Implement proper error handling in custom endpoints

## Limitations and Considerations

- Public key session has time-based validation
- Default endpoints provide basic functionality
- Extend and customize as per project requirements