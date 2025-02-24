using System.Collections.Concurrent;
using lib;

namespace LibMinio;

/// <summary>
/// Manages multiple MinIO client connections using Uid64 IDs.
/// </summary>
public class MinioClientManager
{
    //==========================================================================================================================
    private readonly ConcurrentDictionary<Uid64, MinioClientHandler> _clients = new();

    //==========================================================================================================================
    /// <summary>
    /// Default constructor - initializes an empty client manager.
    /// </summary>
    public MinioClientManager() { }

    //==========================================================================================================================
    /// <summary>
    /// Constructor that initializes the manager with a single MinIO client.
    /// </summary>
    public MinioClientManager(Uid64 id, string endpoint, string accessKey, string secretKey)
    {
        AddClient(id, new MinioClientHandler(endpoint, accessKey, secretKey));
    }

    //==========================================================================================================================
    /// <summary>
    /// Constructor that initializes the manager with a single pre-configured MinIO client.
    /// </summary>
    public MinioClientManager(Uid64 id, MinioClientHandler client)
    {
        AddClient(id, client);
    }

    //==========================================================================================================================
    /// <summary>
    /// Constructor that initializes the manager with multiple MinIO clients.
    /// </summary>
    public MinioClientManager(IEnumerable<(Uid64 id, string endpoint, string accessKey, string secretKey)> clients)
    {
        foreach (var (id, endpoint, accessKey, secretKey) in clients)
        {
            AddClient(id, new MinioClientHandler(endpoint, accessKey, secretKey));
        }
    }

    //==========================================================================================================================
    /// <summary>
    /// Constructor that initializes the manager with multiple pre-configured MinIO clients.
    /// </summary>
    public MinioClientManager(IEnumerable<(Uid64 id, MinioClientHandler client)> clients)
    {
        foreach (var (id, client) in clients)
        {
            AddClient(id, client);
        }
    }

    //==========================================================================================================================
    /// <summary>
    /// Adds a new MinIO client to the manager.
    /// </summary>
    public bool AddClient(Uid64 id, MinioClientHandler client)
    {
        if (_clients.ContainsKey(id))
        {
            Console.WriteLine($"Client with ID '{id}' already exists.");
            return false;
        }
        return _clients.TryAdd(id, client);
    }

    //==========================================================================================================================
    /// <summary>
    /// Adds multiple MinIO clients to the manager.
    /// </summary>
    public int AddClients(IEnumerable<(Uid64 id, MinioClientHandler client)> clients)
    {
        int addedCount = 0;
        foreach (var (id, client) in clients)
        {
            if (AddClient(id, client))
            {
                addedCount++;
            }
        }
        return addedCount;
    }

    //==========================================================================================================================
    /// <summary>
    /// Retrieves a MinIO client by its UUID.
    /// </summary>
    public MinioClientHandler GetClient(Uid64 id) => _clients.TryGetValue(id, out var client) ? client : null;

    //==========================================================================================================================
    /// <summary>
    /// Retrieves all MinIO clients managed by this instance.
    /// </summary>
    public List<MinioClientHandler> GetAllClients() => _clients.Values.ToList();

    //==========================================================================================================================
    /// <summary>
    /// Updates an existing MinIO client with a new client instance.
    /// </summary>
    public bool UpdateClient(Uid64 id, MinioClientHandler newClient)
    {
        if (!_clients.ContainsKey(id))
        {
            Console.WriteLine($"No client found with ID '{id}' to update.");
            return false;
        }
        _clients[id] = newClient;
        return true;
    }

    //==========================================================================================================================
    /// <summary>
    /// Deletes a MinIO client by its UUID.
    /// </summary>
    public bool DeleteClient(Uid64 id) => _clients.TryRemove(id, out _);

    //==========================================================================================================================
    /// <summary>
    /// Deletes all MinIO clients managed by this instance.
    /// </summary>
    public void DeleteAllClients() => _clients.Clear();

    //==========================================================================================================================
    /// <summary>
    /// Tests the connection for a specific MinIO client.
    /// </summary>
    public async Task<bool> TestClientConnectionAsync(Uid64 id)
    {
        if (_clients.TryGetValue(id, out var client))
        {
            return await client.TestConnectionAsync();
        }
        Console.WriteLine($"No client found with ID '{id}'.");
        return false;
    }

    //==========================================================================================================================
    /// <summary>
    /// Tests the connections for all MinIO clients.
    /// </summary>
    /// <returns>True if all clients successfully connect, false otherwise.</returns>
    public async Task<bool> TestAllConnectionsAsync()
    {
        var tasks = _clients.Values.Select(client => client.TestConnectionAsync());
        bool[] results = await Task.WhenAll(tasks);
        return results.All(success => success);
    }
    //==========================================================================================================================
}