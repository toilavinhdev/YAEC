using Microsoft.AspNetCore.Mvc;
using Package.ObjectStorage;
using Package.ObjectStorage.Models;
using YAEC.Shared.Extensions;
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
        group.MapGet("/{*key}", 
            ([FromServices] IObjectStorageService objectStorageService, [FromServices] HttpClient httpClient, string key) =>
            {
                var url = objectStorageService.GetPreviewUrl(key.ToUnescape());
                return Results.Redirect(url);
            })
            .WithSummary("Preview single file")
            .MapToApiVersion(1);
        
        group.MapDelete("/{key}", async
            ([FromServices] IObjectStorageService objectStorageService, string key) =>
            {
                await objectStorageService.DeleteObjectAsync(key.ToUnescape());
                return Results.Ok(ApiResponse.Create());
            })
            .WithSummary("Delete single file")
            .MapToApiVersion(1);
        
        group.MapGet("/{key}/presigned-url", async
            ([FromServices] IObjectStorageService objectStorageService, string key) =>
            {
                var url = await objectStorageService.PresignedGetObjectUrlAsync(key.ToUnescape());
                return Results.Ok(ApiResponse<string>.Create(url));
            })
            .WithSummary("Get file public url")
            .MapToApiVersion(1);
        
        group.MapGet("/{key}/stream", async
            ([FromServices] IObjectStorageService objectStorageService, string key) =>
            {
                var data = await objectStorageService.ReadStreamObjectAsync(key.ToUnescape());
                return Results.Stream(data.Stream, data.ContentType);
            })
            .WithSummary("Stream single file")
            .MapToApiVersion(1);
        
        group.MapPost("/upload", async
            ([FromServices] IObjectStorageService objectStorageService, IFormFile file, [FromQuery] bool? permanent) =>
            {
                var request = new UploadObjectRequest
                {
                    OriginalFileName = file.FileName,
                    Stream = file.OpenReadStream()
                };
                var data = permanent == true
                    ? await objectStorageService.UploadPermanentObjectAsync(request)
                    : await objectStorageService.UploadTempObjectAsync(request);
                return Results.Ok(ApiResponse<UploadObjectResponse>.Create(data));
            })
            .WithSummary("Upload single file")
            .Produces<ApiResponse<UploadObjectResponse>>()
            .WithFormOptions(valueLengthLimit: int.MaxValue, multipartBodyLengthLimit: int.MaxValue)
            .WithMetadata(new DisableRequestSizeLimitAttribute(), new ConsumesAttribute("multipart/form-data"))
            .DisableAntiforgery()
            .MapToApiVersion(1);
        
        group.MapPut("{key}/commit", async
            ([FromServices] IObjectStorageService objectStorageService, string key) =>
            {
                var newKey = await objectStorageService.CommitTempObjectAsync(key.ToUnescape());
                return Results.Ok(ApiResponse<string>.Create(newKey));
            })
            .WithSummary("Commit temporary file")
            .MapToApiVersion(1);
    }

    private static void V2(RouteGroupBuilder group)
    {
        group.MapGet("/", Results.NoContent)
            .WithSummary("Stream single file")
            .MapToApiVersion(2);
    }
}