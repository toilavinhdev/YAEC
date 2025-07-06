namespace YAEC.Shared.Extensions;

public static class FileExtensions
{
    private static readonly Dictionary<string, string> ContentTypes = new Dictionary<string, string>
    {
        // Images
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".bmp", "image/bmp" },
        { ".webp", "image/webp" },
        { ".svg", "image/svg+xml" },
                
        // Videos
        { ".mp4", "video/mp4" },
        { ".avi", "video/x-msvideo" },
        { ".mov", "video/quicktime" },
        { ".wmv", "video/x-ms-wmv" },
        { ".flv", "video/x-flv" },
        { ".webm", "video/webm" },
        { ".mkv", "video/x-matroska" },
                
        // Documents
        { ".pdf", "application/pdf" },
        { ".doc", "application/msword" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".xls", "application/vnd.ms-excel" },
        { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                
        // Others
        { ".txt", "text/plain" },
        { ".json", "application/json" },
        { ".xml", "application/xml" },
        { ".zip", "application/zip" },
        { ".rar", "application/x-rar-compressed" }
    };
    
    public static string GetContentType(this string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (ContentTypes.ContainsKey(extension) && ContentTypes.TryGetValue(extension, out var contentType))
        {
            return contentType;
        }
        return "application/octet-stream";
    }
}