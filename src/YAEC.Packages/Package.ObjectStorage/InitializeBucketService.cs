using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Package.ObjectStorage;

public class InitializeBucketService : IHostedService
{
    private readonly IObjectStorageService _objectStorageService;
    
    private readonly ILogger<InitializeBucketService> _logger;

    public InitializeBucketService(IObjectStorageService objectStorageService, ILogger<InitializeBucketService> logger)
    {
        _objectStorageService = objectStorageService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _objectStorageService.MakeBucketAsync(cancellationToken);
            _logger.LogInformation("S3 lifecycle policy setup completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup S3 lifecycle policy");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}