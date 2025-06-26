using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Xabe.FFmpeg.Downloader;

namespace Package.Sharing.Ffmpeg;

public static class FFmpegExtensions
{
    public static async Task InitializeFFmpeg(this WebApplication app)
    {
        var executablesPath = Path.Combine(app.Environment.ContentRootPath, "FFmpeg");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) executablesPath = Path.Combine(executablesPath, "windows");
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) executablesPath = Path.Combine(executablesPath, "linux");
        else throw new PlatformNotSupportedException();
        var needToInstall =
            !File.Exists(Path.Combine(executablesPath, "ffmpeg.exe")) && !File.Exists(Path.Combine(executablesPath, "ffmpeg"));
        if (needToInstall)
        {
            app.Logger.LogInformation("Starting download FFmpeg: OSPlatform = {OS}", RuntimeInformation.OSDescription);
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, executablesPath);
            var information = await File.ReadAllTextAsync(Path.Combine(executablesPath, "version.json"));
            app.Logger.LogInformation("Finishing download FFmpeg: Information = {@Information}", information);
        }
        Xabe.FFmpeg.FFmpeg.SetExecutablesPath(executablesPath);
        app.Logger.LogInformation("FFmpeg executables path: {Path}", executablesPath);
    }
}