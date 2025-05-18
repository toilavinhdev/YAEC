using Microsoft.AspNetCore.Mvc;
using Package.OpenApi.MinimalApi;
using Package.Shared.Mediator;
using Package.Shared.ValueObjects;
using Service.Identity.Application.UserModule.Queries;

namespace Service.Identity.Endpoints;

public class MeEndpoints : IEndpoints
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/me")
            .WithTags("Me");
        V1(group);
    }

    private static void V1(RouteGroupBuilder group)
    {
        group.MapGet("/", async
            ([FromServices] IMediator mediator) => await mediator.SendAsync(new GetMeQuery()))
            .WithSummary("Xem thông tin người dùng")
            .Produces<ApiResponse<GetMeResponse>>()
            .RequireAuthorization()
            .MapToApiVersion(1);
        
        group.MapPut("/update-password",
            ([FromServices] IMediator mediator) => Results.Ok())
            .WithSummary("Cập nhật mật khẩu")
            .RequireAuthorization()
            .MapToApiVersion(1);
        
        group.MapPut("/forgot-password",
            ([FromServices] IMediator mediator) => Results.Ok())
            .WithSummary("Gửi yêu cầu lấy lại mật khẩu")
            .MapToApiVersion(1);
    }
}