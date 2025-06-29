using Microsoft.AspNetCore.Mvc;
using Service.Identity.Application.Auth.Commands;
using Service.Identity.Application.Auth.Responses;
using YAEC.Shared.Mediator.Abstractions;
using YAEC.Shared.OpenApi.Abstractions;
using YAEC.Shared.ValueObjects;

namespace Service.Identity.Endpoints;

public class AuthEndpoints : IEndpoints
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/auth")
            .WithTags("Auth");
        V1(group);
    }

    private static void V1(RouteGroupBuilder group)
    {
        group.MapPost("/sign-up", async
            ([FromServices] IMediator mediator, [FromBody] SignUpCommand command) => await mediator.SendAsync(command))
            .WithSummary("Register new account")
            .Produces<ApiResponse<SignUpResponse>>()
            .MapToApiVersion(1);

        group.MapPost("/sign-in", async
            ([FromServices] IMediator mediator, [FromBody] SignInCommand command) => await mediator.SendAsync(command))
            .WithSummary("Login your account")
            .Produces<ApiResponse<SignInResponse>>()
            .MapToApiVersion(1);
        
        group.MapPost("/update-password", async
            ([FromServices] IMediator mediator, [FromBody] UpdatePasswordCommand command) => await mediator.SendAsync(command))
            .RequireAuthorization()
            .WithSummary("Update password")
            .Produces<ApiResponse<SignInResponse>>()
            .MapToApiVersion(1);
    }
}