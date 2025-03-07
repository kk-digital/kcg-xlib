using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace LibMinio;

// use to interact with MinIO for managing image files
public class MinioService
{
    //==========================================================================================================================
    private readonly MinioClient _minioClient;
    private readonly string _bucketName;
    //==========================================================================================================================
    // constructor
    public MinioService(string bucketName, string endpoint, string accessKey, string secretKey)
    {
        _bucketName = bucketName;

        _minioClient = new();
        _minioClient.WithEndpoint(endpoint);
        _minioClient.WithCredentials(accessKey, secretKey);
        _minioClient.Build();
    }

    //==========================================================================================================================
    // Upload an image
    public async Task<string> UploadImageAsync(byte[] file, string objectName)
    {
        try
        {
            BucketExistsArgs bucketExistArgs = new();
            bucketExistArgs.WithBucket(_bucketName);

            MakeBucketArgs makeBucketArgs = new();
            makeBucketArgs.WithBucket(_bucketName);

            // Ensure the bucket exists
            bool bucketExists = await _minioClient.BucketExistsAsync(bucketExistArgs);

            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(makeBucketArgs);
                Console.WriteLine($"Bucket '{_bucketName}' created.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return $"Error: {ex.Message}";
        }

        if (await UploadImageToMinio(file, objectName))
        {
            return $"File '{objectName}' uploaded successfully.";
        }
        else
        {
            Console.WriteLine($"Error uploading file '{objectName}'!");
            return $"Error uploading file '{objectName}'!";
        }
    }
    //==========================================================================================================================
    async Task<bool> UploadImageToMinio(byte[] fileData, string objectName)
    {
        try
        {
            string contentType = "application/octet-stream";

            // Check if the object already exists in the bucket
            if (await ObjectExistInBucket(objectName))
            {
                Console.WriteLine($"File '{objectName}' already exists in the bucket.");
                return true;
            }

            // Upload the image
            using (MemoryStream stream = new(fileData))
            {
                PutObjectArgs putObjectArgs = new();

                putObjectArgs.WithBucket(_bucketName);
                putObjectArgs.WithObject(objectName);
                putObjectArgs.WithStreamData(stream);
                putObjectArgs.WithObjectSize(fileData.Length);
                putObjectArgs.WithContentType(contentType);

                await _minioClient.PutObjectAsync(putObjectArgs);
            }

            Console.WriteLine($"File '{objectName}' uploaded successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to upload '{objectName}'. {ex.Message}");
            return false;
        }
    }
    //==========================================================================================================================
    public async Task<bool> ObjectExistInBucket(string objectName)
    {
        try
        {
            StatObjectArgs args = new();

            args.WithBucket(_bucketName);
            args.WithObject(objectName);

            // Check if the object exists by calling StatObject
            ObjectStat stat = await _minioClient.StatObjectAsync(args);
            
            // Checks if object is valid, if not MinIO probably is unavailable.
            if (!IsValidObjectStat(stat))
            {
                Console.WriteLine($"Could not find object '{objectName}', please check server availability.");
                return false;
            }
            
            return true;
        }
        catch (ObjectNotFoundException)
        {
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occurred while checking object existence: {e.Message}");
            return false;
        }
    }
    //==========================================================================================================================
    // Download an image
    public async Task<byte[]> DownloadImageAsByteArrayAsync(string objectName)
    {
        try
        {
            using (MemoryStream memoryStream = new())
            {
                GetObjectArgs args = new();

                args.WithBucket(_bucketName);
                args.WithObject(objectName);
                args.WithCallbackStream((stream) =>
                {
                    stream.CopyTo(memoryStream);
                });

                // Get the object as a stream from MinIO
                ObjectStat minioObject = await _minioClient.GetObjectAsync(args);

                // Return the byte array from memory stream
                return memoryStream.ToArray();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;  // Return null if an error occurs (e.g., file not found)
        }
    }
    //==========================================================================================================================
    // Get a list of all objects in the bucket
    public async Task<List<string>> ListObjectsAsync()
    {
        try
        {
            ListObjectsArgs args = new();

            args.WithBucket(_bucketName);

            List<string> objectList = new();

            IAsyncEnumerable<Item> items = _minioClient.ListObjectsEnumAsync(args);

            await foreach (Item item in items)
            {
                objectList.Add(item.Key);
            }

            return objectList;
        }
        catch (Exception)
        {
            return null;
        }
    }
    //==========================================================================================================================
    // Delete an image from the bucket
    public async Task<bool> DeleteImageAsync(string objectName)
    {
        try
        {
            RemoveObjectArgs args = new();

            args.WithBucket(_bucketName);
            args.WithObject(objectName);

            await _minioClient.RemoveObjectAsync(args);
            return true;
        }
        catch
        {
            return false;
        }
    }
    //==========================================================================================================================
    // Validate ObjectStat because when the server is offline
    // it returns a valid ObjectStat that is null
    private bool IsValidObjectStat(ObjectStat stat)
    {
        if (string.IsNullOrWhiteSpace(stat.ObjectName))
            return false;

        if (stat.Size <= 0)
            return false;

        if (stat.LastModified == default || stat.LastModified.Year <= 1)
            return false;

        if (string.IsNullOrWhiteSpace(stat.ContentType))
            return false;

        if (string.IsNullOrWhiteSpace(stat.ETag))
            return false;

        if ((stat.MetaData == null || stat.MetaData.Count == 0) &&
            (stat.ExtraHeaders == null || stat.ExtraHeaders.Count == 0))
            return false;

        return true;
    }
    //==========================================================================================================================
}