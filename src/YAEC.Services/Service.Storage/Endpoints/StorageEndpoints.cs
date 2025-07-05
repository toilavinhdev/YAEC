using Microsoft.AspNetCore.Mvc;
using Package.ObjectStorage;
using Package.ObjectStorage.Models;
using Service.Storage.Services;
using YAEC.Shared.OpenApi.Abstractions;
using YAEC.Shared.ValueObjects;

namespace Service.Storage.Endpoints;

public class StorageEndpoints : IEndpoints
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/")
            .WithTags("Storage");
        V1(group);
        V2(group);
    }

    public void V1(RouteGroupBuilder group)
    {
        group.MapGet("/", async
            ([FromServices] IObjectStorageService objectStorageService, [FromQuery] string key) =>
            {
                var data = await objectStorageService.ReadStreamObjectAsync(Uri.UnescapeDataString(key));
                return Results.Stream(data.Stream, data.ContentType);
            })
            .WithSummary("Stream single file")
            .MapToApiVersion(1);
        
        group.MapPost("/upload", async
            ([FromServices] IObjectStorageService objectStorageService, IFormFile file) =>
            {
                var data = await objectStorageService.UploadObjectAsync(new UploadObjectRequest
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
            ([FromServices] IVideoProcessorService videoProcessorService, IFormFile file) =>
            {
                await videoProcessorService.ProcessVideoAsync(file);
            })
            .WithSummary("Process video")
            .DisableAntiforgery()
            .MapToApiVersion(1);
    }

    private static void V2(RouteGroupBuilder group)
    {
        group.MapGet("/", Results.NoContent)
            .WithSummary("Stream single file")
            .MapToApiVersion(2);
    }
}