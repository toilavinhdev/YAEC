using Microsoft.AspNetCore.Mvc;
using Service.Storage.Services;
using YAEC.Shared.OpenApi.Abstractions;

namespace Service.Storage.Endpoints;

public class MediaEndpoints : IEndpoints
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/media")
            .WithTags("Media");
        V1(group);
    }

    public void V1(RouteGroupBuilder group)
    {
        group.MapPost("/video/process", async 
            ([FromServices] IVideoProcessorService videoProcessorService, IFormFile file) =>
            {
                await videoProcessorService.ProcessVideoAsync(file);
            })
            .WithSummary("Process video")
            .DisableAntiforgery()
            .MapToApiVersion(1);
    }
}