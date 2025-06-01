using MongoDB.Driver;
using Package.Identity.Extensions;
using Package.MongoDb;
using Package.Shared.Exceptions;
using Package.Shared.Mediator;
using Package.Shared.ValueObjects;
using Service.Identity.Domain.Aggregates.UserAggregate;

namespace Service.Identity.Application.UserModule.Queries;

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
        var userFilter = Builders<User>.Filter.Eq(x => x.Id, userClaims.Id);
        var userAsyncCursor = await _mongoDbService.Collection<User>()
            .FindAsync(userFilter, cancellationToken: cancellationToken);
        var user = await userAsyncCursor.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (user is null)
        {
            return Results.NotFound(new ApiResponse
            {
                Message = "User was not found"
            });
        }
        return Results.Ok(new ApiResponse<GetMeResponse>()
        {
            Data = new GetMeResponse
            {
                Id = user.Id,
                AutoId = user.AutoId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
            }
        });
    }
}

public class GetMeResponse
{
    public string Id { get; set; } = null!;
    
    public long AutoId { get; set; }
    
    public string FullName { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string PhoneNumber { get; set; } = null!;
    
    public DateTime? CreatedAt { get; set; }
}