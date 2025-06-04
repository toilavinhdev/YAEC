namespace Package.ObjectStorage;

public class S3Options
{
    public string ServiceUrl { get; set; } = null!;
    
    public string BucketName { get; set; } = null!;
    
    public string AccessKey { get; set; } = null!;
    
    public string SecretKey { get; set; } = null!;
}