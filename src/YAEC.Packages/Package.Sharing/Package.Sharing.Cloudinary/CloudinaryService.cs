using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Package.Sharing.Cloudinary;

public interface ICloudinaryService
{
    Task<ImageUploadResult?> UploadImageAsync(Stream stream, string fileId);

    Task<VideoUploadResult?> UploadVideoAsync(Stream stream, string publicId);

    Task<DeletionResult> DeleteAsync(string publicId);
}

public class CloudinaryService : ICloudinaryService
{
    private readonly CloudinaryOptions _options;
    
    private readonly CloudinaryDotNet.Cloudinary _cloudinary;

    public CloudinaryService(CloudinaryOptions options)
    {
        _options = options;
        _cloudinary = new CloudinaryDotNet.Cloudinary(options.Url)
        {
            Api =
            {
                Secure = true
            }
        };
    }
    
    public async Task<ImageUploadResult?> UploadImageAsync(Stream stream, string fileId)
    {
        var result = await _cloudinary.UploadAsync(new ImageUploadParams
        {
            File = new FileDescription(fileId, stream),
            Folder = _options.Folder,
            PublicId = fileId
        });
        return result;
    }
    
    public async Task<VideoUploadResult?> UploadVideoAsync(Stream stream, string publicId)
    {
        var result = await _cloudinary.UploadAsync(new VideoUploadParams
        {
            File = new FileDescription(publicId, stream),
            Folder = _options.Folder,
            PublicId = publicId
        });
        return result;
    }

    public async Task<DeletionResult> DeleteAsync(string publicId)
    {
        var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));
        return result;
    }
}