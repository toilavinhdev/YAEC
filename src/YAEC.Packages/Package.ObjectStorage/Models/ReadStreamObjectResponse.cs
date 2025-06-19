namespace Package.ObjectStorage.Models;

public class ReadStreamObjectResponse
{
    public Stream Stream { get; set; } = null!;
    
    public string ContentType { get; set; } = null!;
    
    public long ContentLength { get; set; }
}