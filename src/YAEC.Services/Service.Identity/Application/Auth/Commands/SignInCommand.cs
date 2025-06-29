using MongoDB.Driver;
using Package.Identity;
using Package.MongoDb;
using Service.Identity.Application.Auth.Responses;
using Service.Identity.Domain.Aggregates.UserAggregate;
using YAEC.Shared.Extensions;
using YAEC.Shared.Mediator.Abstractions;
using YAEC.Shared.ValueObjects;

namespace Service.Identity.Application.Auth.Commands;

public class SignInCommand : IRequest<IResult>
{
    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;
}

public class SignInCommandHandler : IRequestHandler<SignInCommand, IResult>
{
    private readonly IMongoDbService _mongoDbService;
    
    private readonly IdentityOptions _identityOptions;

    public SignInCommandHandler(IMongoDbService mongoDbService, IdentityOptions identityOptions)
    {
        _mongoDbService = mongoDbService;
        _identityOptions = identityOptions;
    }

    public async Task<IResult> HandleAsync(SignInCommand request, CancellationToken cancellationToken)
    {
        var userCursor = await _mongoDbService.Collection<User>()
            .FindAsync(Builders<User>.Filter.Or(
                    Builders<User>.Filter.Eq(x => x.Email, request.UserName),
                    Builders<User>.Filter.Eq(x => x.PhoneNumber, request.UserName)),
                cancellationToken: cancellationToken);
        var user = await userCursor.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (user is null) return Results.NotFound(ApiResponse.Create("User name not found"));
        if (user.PasswordHash != request.Password.ToSha256())
            return Results.BadRequest(ApiResponse.Create("Invalid password"));
        
        var userClaims = new IdentityUserClaims
        {
            Id = user.Id,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Policies = []
        };
        var accessToken = userClaims.GenerateAccessToken(_identityOptions);
        var refreshToken = StringExtensions.RandomString(36);
        return Results.Ok(ApiResponse<SignInResponse>.Create(new SignInResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiration = DateTimeExtensions.Now.AddMinutes(_identityOptions.AccessTokenDurationInMinutes),
            RefreshToken = refreshToken,
            RefreshTokenExpiration = DateTimeExtensions.Now.AddDays(_identityOptions.RefreshTokenDurationInDays),
        }));
    }
}