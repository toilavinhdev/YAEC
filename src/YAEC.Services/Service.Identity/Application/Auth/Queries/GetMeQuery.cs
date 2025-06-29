using MongoDB.Driver;
using Package.Identity.Extensions;
using Package.MongoDb;
using Service.Identity.Application.Auth.Responses;
using Service.Identity.Domain.Aggregates.UserAggregate;
using YAEC.Shared.Mediator.Abstractions;
using YAEC.Shared.ValueObjects;

namespace Service.Identity.Application.Auth.Queries;

public class GetMeQuery : IRequest<IResult>;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, IResult>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly IMongoDbService _mongoDbService;

    public GetMeQueryHandler(IHttpContextAccessor httpContextAccessor, IMongoDbService mongoDbService)
    {
        _httpContextAccessor = httpContextAccessor;
        _mongoDbService = mongoDbService;
    }

    public async Task<IResult> HandleAsync(GetMeQuery request, CancellationToken cancellationToken)
    {
        var userClaims = _httpContextAccessor.UserClaims();
        var userCursor = await _mongoDbService.Collection<User>()
            .FindAsync(Builders<User>.Filter.Eq(x => x.Id, userClaims.Id),
                cancellationToken: cancellationToken);
        var user = await userCursor.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (user is null) return Results.NotFound(ApiResponse.Create("User was not found"));
        return Results.Ok(ApiResponse<GetMeResponse>.Create(new GetMeResponse
        {
            Id = user.Id,
            SubId = user.SubId,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = user.CreatedAt,
        }));
    }
}