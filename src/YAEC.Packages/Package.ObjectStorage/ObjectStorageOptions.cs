namespace Package.ObjectStorage;

public class ObjectStorageOptions
{
    public string ServiceUrl { get; set; } = null!;
    
    public string BucketName { get; set; } = null!;
    
    public string Credential { get; set; } = null!;
    
    public bool Ssl { get; set; }
}