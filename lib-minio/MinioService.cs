using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace LibMinio;

// Enum to represent different types of images
public enum ImageType
{
    PinterestThumbnail,
    PinterestFullImage,
    YoutubeThumbnail
}

// use to interact with MinIO for managing image files
public class MinioService
{
    //==========================================================================================================================
    private readonly MinioClient _minioClient;
    private readonly string _defaultBucketName;
    private readonly Dictionary<ImageType, string> _bucketMap;
    //==========================================================================================================================
    // constructor
    public MinioService(string defaultBucketName, string endpoint, string accessKey, string secretKey)
    {
        _defaultBucketName = defaultBucketName;

        // Initialize the bucket mapping
        _bucketMap = new Dictionary<ImageType, string>
        {
            { ImageType.PinterestThumbnail, "pinterest-thumbnails" },
            { ImageType.PinterestFullImage, "pinterest-full-images" },
            { ImageType.YoutubeThumbnail, "youtube-thumbnails" }
        };

        _minioClient = new();
        _minioClient.WithEndpoint(endpoint);
        _minioClient.WithCredentials(accessKey, secretKey);
        _minioClient.Build();
    }

    // Get the appropriate bucket name based on image type
    private string GetBucketName(ImageType? imageType = null)
    {
        if (imageType == null)
        {
            return _defaultBucketName;
        }

        return _bucketMap[(ImageType)imageType];
    }

    // Ensure the bucket exists, create if it doesn't
    private async Task EnsureBucketExistsAsync(string bucketName)
    {
        BucketExistsArgs bucketExistArgs = new();
        bucketExistArgs.WithBucket(bucketName);

        MakeBucketArgs makeBucketArgs = new();
        makeBucketArgs.WithBucket(bucketName);

        // Ensure the bucket exists
        bool bucketExists = await _minioClient.BucketExistsAsync(bucketExistArgs);

        if (!bucketExists)
        {
            await _minioClient.MakeBucketAsync(makeBucketArgs);
            Console.WriteLine($"Bucket '{bucketName}' created.");
        }
    }

    //==========================================================================================================================
    // Upload an image
    public async Task<string> UploadImageAsync(byte[] file, string objectName, ImageType? imageType = null)
    {
        string bucketName = GetBucketName(imageType);
        
        try
        {
            await EnsureBucketExistsAsync(bucketName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return $"Error: {ex.Message}";
        }

        if (await UploadImageToMinio(file, objectName, bucketName))
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
    async Task<bool> UploadImageToMinio(byte[] fileData, string objectName, string bucketName)
    {
        try
        {
            string contentType = "application/octet-stream";

            // Check if the object already exists in the bucket
            if (await ObjectExistInBucket(objectName, bucketName))
            {
                Console.WriteLine($"File '{objectName}' already exists in the bucket '{bucketName}'.");
                return true;
            }

            // Upload the image
            using (MemoryStream stream = new(fileData))
            {
                PutObjectArgs putObjectArgs = new();

                putObjectArgs.WithBucket(bucketName);
                putObjectArgs.WithObject(objectName);
                putObjectArgs.WithStreamData(stream);
                putObjectArgs.WithObjectSize(fileData.Length);
                putObjectArgs.WithContentType(contentType);

                await _minioClient.PutObjectAsync(putObjectArgs);
            }

            Console.WriteLine($"File '{objectName}' uploaded successfully to bucket '{bucketName}'.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to upload '{objectName}' to bucket '{bucketName}'. {ex.Message}");
            return false;
        }
    }
    //==========================================================================================================================
    public async Task<bool> ObjectExistInBucket(string objectName, string bucketName = null)
    {
        bucketName = bucketName ?? _defaultBucketName;
        
        try
        {
            StatObjectArgs args = new();

            args.WithBucket(bucketName);
            args.WithObject(objectName);

            // Check if the object exists by calling StatObject
            ObjectStat stat = await _minioClient.StatObjectAsync(args);
            return true;
        }
        catch (ObjectNotFoundException)
        {
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occurred: {e.Message}");
            return true;
        }
    }

    // Overload to support the enum version
    public async Task<bool> ObjectExistInBucket(string objectName, ImageType imageType)
    {
        return await ObjectExistInBucket(objectName, GetBucketName(imageType));
    }
    //==========================================================================================================================
    // Download an image
    public async Task<byte[]> DownloadImageAsByteArrayAsync(string objectName, ImageType? imageType = null)
    {
        string bucketName = GetBucketName(imageType);
        
        try
        {
            using (MemoryStream memoryStream = new())
            {
                GetObjectArgs args = new();

                args.WithBucket(bucketName);
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
    public async Task<List<string>> ListObjectsAsync(ImageType? imageType = null)
    {
        string bucketName = GetBucketName(imageType);
        
        try
        {
            ListObjectsArgs args = new();

            args.WithBucket(bucketName);

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
    public async Task<bool> DeleteImageAsync(string objectName, ImageType? imageType = null)
    {
        string bucketName = GetBucketName(imageType);
        
        try
        {
            RemoveObjectArgs args = new();

            args.WithBucket(bucketName);
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
}