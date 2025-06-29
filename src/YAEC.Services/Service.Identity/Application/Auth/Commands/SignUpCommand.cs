using MongoDB.Bson;
using MongoDB.Driver;
using Package.MongoDb;
using Service.Identity.Application.Auth.Responses;
using Service.Identity.Domain.Aggregates.UserAggregate;
using YAEC.Shared.Constants.Enumerations.User;
using YAEC.Shared.Extensions;
using YAEC.Shared.Mediator.Abstractions;
using YAEC.Shared.ValueObjects;

namespace Service.Identity.Application.Auth.Commands;

public class SignUpCommand : IRequest<IResult>
{
    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Password { get; set; } = null!;
}

public class SignUpCommandHandler :  IRequestHandler<SignUpCommand, IResult>
{
    private readonly IMongoDbService _mongoDbService;

    public SignUpCommandHandler(IMongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    public async Task<IResult> HandleAsync(SignUpCommand request, CancellationToken cancellationToken)
    {
        var userFilterBuilder = Builders<User>.Filter.Or(
            Builders<User>.Filter.Eq(x => x.Email, request.Email),
            Builders<User>.Filter.Eq(x => x.PhoneNumber, request.PhoneNumber));
        var userCursor = await _mongoDbService.Collection<User>()
            .FindAsync(userFilterBuilder, cancellationToken: cancellationToken);
        var user = await userCursor.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (user is not null) return Results.Conflict(ApiResponse.Create("Email or phone already exists"));
        
        user = new User
        {
            Id = ObjectId.GenerateNewId().ToString(),
            SubId = await _mongoDbService.NextSequenceAsync<User>(cancellationToken),
            Email = request.Email,
            IsEmailVerified = false,
            PhoneNumber = request.PhoneNumber,
            IsPhoneVerified = false,
            Status = UserStatus.Active,
            PasswordHash = request.Password.ToSha256(),
            CreatedAt = DateTimeExtensions.Now,
        };
        await _mongoDbService.Collection<User>().InsertOneAsync(user, cancellationToken: cancellationToken);
        return Results.Ok(ApiResponse<SignUpResponse>.Create(new SignUpResponse
        {
            Id = user.Id,
            SubId = user.SubId,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = user.CreatedAt
        }));
    }
}