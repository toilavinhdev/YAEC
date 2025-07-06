using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Package.ObjectStorage.Models;
using Package.Serilog;

namespace Package.ObjectStorage;

public interface IObjectStorageService
{
    string GetPreviewUrl(string key);
    
    Task MakeBucketAsync(CancellationToken cancellationToken = default);
    
    Task<ReadStreamObjectResponse> ReadStreamObjectAsync(string key, CancellationToken cancellationToken = default);

    Task<UploadObjectResponse> UploadPermanentObjectAsync(UploadObjectRequest upload, CancellationToken cancellationToken = default);

    Task<UploadObjectResponse> UploadTempObjectAsync(UploadObjectRequest upload, CancellationToken cancellationToken = default);

    Task<string> CommitTempObjectAsync(string key, CancellationToken cancellationToken = default);
    
    Task DeleteObjectAsync(string key, CancellationToken cancellationToken = default);

    Task<string> PresignedGetObjectUrlAsync(string key, CancellationToken cancellationToken = default);
}

public class ObjectStorageService : IObjectStorageService
{
    private readonly AmazonS3Client _client;

    private readonly ObjectStorageOptions _options;

    private readonly ILogger<ObjectStorageService> _logger;

    private const string TempPrefix = "temp/";

    private const string PermanentPrefix = "";
    
    private const int TempExpiryDays = 1;
    
    private const int IncompleteMultipartUploadExpiryDays = 1;

    public ObjectStorageService(ILogger<ObjectStorageService> logger, AmazonS3Client client,
        ObjectStorageOptions options)
    {
        _logger = logger;
        _client = client;
        _options = options;
    }

    private static string GenerateObjectKey(string prefix, string originalFileName)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
        var key = $"{prefix}{DateTime.UtcNow:yyyy/MM/dd}/{fileName}";
        return key;
    }
    
    public string GetPreviewUrl(string key)
    {
        _logger.LogMethodInformation(key);
        return $"{_options.ServiceUrl}/{_options.BucketName}/{key}";
    }

    public async Task MakeBucketAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogMethodInformation();
        if (!await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_client, _options.BucketName))
        {
            // Create bucket
            await _client.PutBucketAsync(new PutBucketRequest
            {
                BucketName = _options.BucketName
            }, cancellationToken);
        };

        // Setup bucket policies
        var policy =
            @"{
                ""Version"": ""2012-10-17"",
                ""Statement"": [
                    {
                        ""Sid"": ""public-read"",
                        ""Effect"": ""Allow"",
                        ""Principal"": ""*"",
                        ""Action"": ""s3:GetObject"",
                        ""Resource"": ""arn:aws:s3:::" + _options.BucketName + @"/*""
                    }
                ]
            }";
        await _client.PutBucketPolicyAsync(new PutBucketPolicyRequest
        {
            BucketName = _options.BucketName,
            Policy = policy
        }, cancellationToken);
        
        // Setup lifecycle polices
        var lifecycleConfiguration = new LifecycleConfiguration
        {
            Rules =
            [
                new LifecycleRule
                {
                    Id = "temp-objects-cleanup",
                    Status = LifecycleRuleStatus.Enabled,
                    Filter = new LifecycleFilter
                    {
                        LifecycleFilterPredicate = new LifecyclePrefixPredicate
                        {
                            Prefix = TempPrefix
                        }
                    },
                    Expiration = new LifecycleRuleExpiration
                    {
                        Days = TempExpiryDays
                    },
                    AbortIncompleteMultipartUpload = new LifecycleRuleAbortIncompleteMultipartUpload
                    {
                        DaysAfterInitiation = IncompleteMultipartUploadExpiryDays
                    }
                }

            ]
        };
        await _client.PutLifecycleConfigurationAsync(new PutLifecycleConfigurationRequest
        {
            BucketName = _options.BucketName,
            Configuration = lifecycleConfiguration
        }, cancellationToken);
    }
    
    public async Task<ReadStreamObjectResponse> ReadStreamObjectAsync(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodInformation(key);
        var request = new GetObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key
        };
        var response = await _client.GetObjectAsync(request, cancellationToken);
        return new ReadStreamObjectResponse
        {
            Stream = response.ResponseStream,
            ContentType = response.Headers.ContentType,
            ContentLength = response.Headers.ContentLength,
        };
    }

    public async Task<UploadObjectResponse> UploadPermanentObjectAsync(UploadObjectRequest upload, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodInformation();
        var key = GenerateObjectKey(PermanentPrefix, upload.OriginalFileName);
        return await UploadObjectAsync(key, upload, cancellationToken);
    }
    
    public async Task<UploadObjectResponse> UploadTempObjectAsync(UploadObjectRequest upload, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodInformation();
        var key = GenerateObjectKey(TempPrefix, upload.OriginalFileName);
        return await UploadObjectAsync(key, upload, cancellationToken);
    }
    
    public async Task<string> CommitTempObjectAsync(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodInformation(key);
        var newKey = key.Replace(TempPrefix, PermanentPrefix);
        var copyRequest = new CopyObjectRequest
        {
            SourceBucket = _options.BucketName,
            SourceKey = key,
            DestinationBucket = _options.BucketName,
            DestinationKey = newKey
        };
        copyRequest.Metadata.Add("X-Amz-Meta-Committed-At", DateTime.Now.ToString("yy-MM-dd HH:mm:ss"));
        copyRequest.Metadata.Add("X-Amz-Meta-Source-Temp-Key", key);
        await _client.CopyObjectAsync(copyRequest, cancellationToken);
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key
        };
        await _client.DeleteObjectAsync(deleteRequest, cancellationToken);
        return newKey;
    }

    private async Task<UploadObjectResponse> UploadObjectAsync(string key, UploadObjectRequest upload, CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key,
            InputStream = upload.Stream,
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.None,
        };
        request.Metadata.Add("X-Amz-Meta-Original-File-Name", upload.OriginalFileName);
        request.Metadata.Add("X-Amz-Meta-Created-At", DateTime.Now.ToString("yy-MM-dd HH:mm:ss"));
        await _client.PutObjectAsync(request, cancellationToken);
        return new UploadObjectResponse
        {
            Key = key,
        };
    }
    
    public async Task DeleteObjectAsync(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodInformation(key);
        var request = new DeleteObjectRequest
        {
            Key = key,
            BucketName = _options.BucketName
        };
        await _client.DeleteObjectAsync(request, cancellationToken);
    }

    public async Task<string> PresignedGetObjectUrlAsync(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodInformation(key);
        if (cancellationToken.IsCancellationRequested) return string.Empty;
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddDays(1),
            Verb = HttpVerb.GET,
            Protocol = _options.Ssl ? Protocol.HTTPS : Protocol.HTTP
        };
        var publicUrl = await _client.GetPreSignedURLAsync(request);
        return publicUrl;
    }
}