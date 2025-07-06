namespace Package.ObjectStorage.Models;

public class UploadObjectResponse
{
    public string Key { get; set; } = null!;

    public long ContentLength { get; set; }
}