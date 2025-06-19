using System.Diagnostics;
using Package.ObjectStorage;
using Xabe.FFmpeg;

namespace Service.Storage.Services;

public interface IVideoProcessorService
{
    Task ProcessVideoAsync(IFormFile file, CancellationToken cancellationToken = default);
}

public class VideoProcessorService : IVideoProcessorService
{
    private readonly ILogger<VideoProcessorService> _logger;

    private readonly IWebHostEnvironment _webHostEnvironment;

    private readonly IObjectStorageService _objectStorageService;

    public VideoProcessorService(ILogger<VideoProcessorService> logger, IObjectStorageService objectStorageService,
        IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _objectStorageService = objectStorageService;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task ProcessVideoAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var tempFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Conversions");
        var inputFileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
        var inputFilePath = Path.Combine(tempFolderPath, inputFileName);
        var outputFilePath = Path.Combine(
            tempFolderPath,
            $"{Path.GetFileNameWithoutExtension(inputFileName)}_resized{Path.GetExtension(inputFileName)}");
        await using (var fileStream = new FileStream(inputFilePath, FileMode.Create))
            await file.CopyToAsync(fileStream, cancellationToken);
        
        try
        {
            if (cancellationToken.IsCancellationRequested) return;
            var mediaInfo = await FFmpeg.GetMediaInfo(inputFilePath, cancellationToken);
            var conversion = FFmpeg.Conversions.New().SetOverwriteOutput(true);
            conversion.AddParameter("-hide_banner");
            conversion.AddParameter($"-i \"{inputFilePath}\"");

            // Audio
            conversion.AddParameter("-c:a aac"); // Codec audio: AAC
            conversion.AddParameter("-b:a 32k"); // Bitrate audio: 64k

            // Video
            conversion.AddParameter("-vf scale=w=480:h=-1"); // Video filter: Rộng 1280px, cao tự động, tỷ lệ khung hình gốc
            conversion.AddParameter("-crf 28"); // CRF: Điều chỉnh chất lượng video, càng cao thì chất lượng càng thấp
            conversion.AddParameter("-c:v libx264"); // Codec video: H.264, H.265
            
            // Output
            conversion.AddParameter($"\"{outputFilePath}\"");

            conversion.OnProgress += (_, args) =>
            {
                var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
                _logger.LogInformation(
                    "{InputVideoName} [{Duration} / {TotalLength}] {percent}%",
                    inputFileName,
                    args.Duration,
                    args.TotalLength,
                    percent);
            };
            
            _logger.LogInformation("ffmpeg{Parameters}", conversion.Build());
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await conversion.Start(cancellationToken);
            stopwatch.Stop();
            _logger.LogInformation("Video process duration: {Duration}", stopwatch.Elapsed.ToString("g"));
            
            _logger.LogInformation(
                "Video before process: Size= {Size} Mb, Video = {@Video}, Audio = {@Audio}",
                (double)mediaInfo.Size / (1024 * 1024),
                mediaInfo.VideoStreams.FirstOrDefault(),
                mediaInfo.AudioStreams.FirstOrDefault());
            
            var outputMediaInfo = await FFmpeg.GetMediaInfo(outputFilePath, cancellationToken);
            _logger.LogInformation(
                "Video after process: Size= {Size} Mb, Video = {@Video}, Audio = {@Audio}",
                (double)outputMediaInfo.Size / (1024 * 1024),
                outputMediaInfo.VideoStreams.FirstOrDefault(),
                outputMediaInfo.AudioStreams.FirstOrDefault());
        }
        catch (Exception ex)
        {
            _logger.LogError("Process video was exception: {@Exception}", ex);
        }
    }
}