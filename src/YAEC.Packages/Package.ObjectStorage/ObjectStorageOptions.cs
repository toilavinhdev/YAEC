namespace Package.ObjectStorage;

public class ObjectStorageOptions
{
    public string ServiceUrl { get; set; } = null!;
    
    public string BucketName { get; set; } = null!;
    
    public string AccessKey { get; set; } = null!;
    
    public string SecretKey { get; set; } = null!;
}