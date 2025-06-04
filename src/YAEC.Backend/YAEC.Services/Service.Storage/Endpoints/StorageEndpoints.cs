using Microsoft.AspNetCore.Mvc;
using Package.OpenApi.MinimalApi;
using Package.ObjectStorage;
using Package.ObjectStorage.Models;
using Package.Shared.ValueObjects;
using Service.Storage.Services;

namespace Service.Storage.Endpoints;

public class StorageEndpoints : IEndpoints
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/storage")
            .WithTags("Storage");
        V1(group);
    }

    private static void V1(RouteGroupBuilder group)
    {
        group.MapGet("/", async
            ([FromServices] IS3Manager s3Manager, [FromQuery] string key) =>
            {
                var data = await s3Manager.ReadStreamObjectAsync(Uri.UnescapeDataString(key));
                return Results.Stream(data.Stream, data.ContentType);
            })
            .WithSummary("Stream single file")
            .MapToApiVersion(1);
        
        group.MapPost("/upload", async
            ([FromServices] IS3Manager s3Manager, IFormFile file) =>
            {
                var data = await s3Manager.UploadObjectAsync(new UploadObjectRequest
                {
                    OriginalFileName = file.FileName,
                    Stream = file.OpenReadStream()
                });
                return Results.Ok(new ApiResponse<UploadObjectResponse>
                {
                    Data = data,
                    Message = "Upload file success"
                });
            })
            .WithSummary("Upload single file")
            .Produces<ApiResponse<UploadObjectResponse>>()
            .DisableAntiforgery()
            .MapToApiVersion(1);
        
        group.MapPost("/video/process", async 
            (IFormFile file, IVideoProcessorService videoProcessorService) =>
            {
                await videoProcessorService.ProcessVideoAsync(file);
            })
            .WithSummary("Process video")
            .DisableAntiforgery()
            .MapToApiVersion(1);
    }
}