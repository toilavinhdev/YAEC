using Microsoft.AspNetCore.Routing;

namespace YAEC.Shared.OpenApi.Abstractions;

public interface IEndpoints
{
    void MapEndpoints(IEndpointRouteBuilder app);

    void V1(RouteGroupBuilder group);
}