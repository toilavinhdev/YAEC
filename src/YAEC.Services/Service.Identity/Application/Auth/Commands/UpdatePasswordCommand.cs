using MongoDB.Driver;
using Package.Identity.Extensions;
using Package.MongoDb;
using Service.Identity.Domain.Aggregates.UserAggregate;
using YAEC.Shared.Extensions;
using YAEC.Shared.Mediator.Abstractions;
using YAEC.Shared.ValueObjects;

namespace Service.Identity.Application.Auth.Commands;

public class UpdatePasswordCommand : IRequest<IResult>
{
    public string CurrentPassword { get; set; } = null!;

    public string NewPassword { get; set; } = null!;
}

public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, IResult>
{
    private readonly IMongoDbService _mongoDbService;
    
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdatePasswordCommandHandler(IMongoDbService mongoDbService, IHttpContextAccessor httpContextAccessor)
    {
        _mongoDbService = mongoDbService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IResult> HandleAsync(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        var userClaims = _httpContextAccessor.UserClaims();
        var userFilterBuilder = Builders<User>.Filter.Eq(x => x.Id, userClaims.Id);
        var userCursor = await _mongoDbService.Collection<User>()
            .FindAsync(userFilterBuilder, cancellationToken: cancellationToken);
        var user = await userCursor.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (user is null) return Results.NotFound(ApiResponse.Create("User was not found"));
        if (user.PasswordHash != request.CurrentPassword.ToSha256())
            return Results.BadRequest(ApiResponse.Create("Current password was not match"));
        
        user.PasswordHash = request.NewPassword.ToSha256();
        user.ModifiedAt = DateTimeExtensions.Now;
        var userUpdateBuilder = Builders<User>.Update
            .Set(u => u.PasswordHash, user.PasswordHash)
            .Set(u => u.ModifiedAt, user.ModifiedAt);
        await _mongoDbService.Collection<User>()
            .UpdateOneAsync(userFilterBuilder, userUpdateBuilder, cancellationToken: cancellationToken);
        return Results.Ok(ApiResponse.Create());
    }
}