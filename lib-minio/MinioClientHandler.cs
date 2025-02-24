using Minio;

namespace LibMinio;

/// <summary>
/// Handles the creation and management of a MinIO client connection.
/// </summary>
public class MinioClientHandler
{
    //==========================================================================================================================
    private readonly MinioClient _client;
    private readonly string _endpoint;
    private readonly string _accessKey;
    private readonly string _secretKey;

    //==========================================================================================================================
    /// <summary>
    /// Gets the MinIO client instance.
    /// </summary>
    public MinioClient Client => _client;

    //==========================================================================================================================
    /// <summary>
    /// Initializes a MinIO client handler with an existing MinIO client.
    /// </summary>
    /// <param name="client">Existing MinIO client.</param>
    public MinioClientHandler(MinioClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    //==========================================================================================================================
    /// <summary>
    /// Initializes a new MinIO client handler with the specified connection details.
    /// </summary>
    public MinioClientHandler(string endpoint, string accessKey = "", string secretKey = "")
    {
        _endpoint = endpoint;
        _accessKey = accessKey ?? Environment.GetEnvironmentVariable("MINIO_ROOT_USER");
        _secretKey = secretKey ?? Environment.GetEnvironmentVariable("MINIO_ROOT_PASSWORD");

        if (string.IsNullOrEmpty(_accessKey))
            _accessKey = "root";

        if (string.IsNullOrEmpty(_secretKey))
            _secretKey = "testings";

        _client = new MinioClient();
        _client.WithEndpoint(_endpoint);
        _client.WithCredentials(_accessKey, _secretKey);
        _client.Build();
    }

    //==========================================================================================================================
    /// <summary>
    /// Tests the connection to the MinIO server.
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            var buckets = await _client.ListBucketsAsync();

            if (buckets.Buckets == null)
            {
                Console.WriteLine($"MinIO connection failed");
                return false;
            }

            Console.WriteLine($"Connection successful to {_endpoint}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MinIO connection failed: {ex.Message}");
            return false;
        }
    }
    //==========================================================================================================================
}