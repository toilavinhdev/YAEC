using MongoDB.Driver;
using Package.Identity;
using Package.MongoDb;
using Package.Shared.Extensions;
using Package.Shared.Mediator;
using Package.Shared.ValueObjects;
using Service.Identity.Domain.Aggregates.UserAggregate;

namespace Service.Identity.Application.UserModule.Commands;

public class SignInCommand : IRequest<IResult>
{
    public string Key { get; set; } = null!;

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
        var userAsyncCursor = await _mongoDbService.Collection<User>()
            .FindAsync(x => x.Email == request.Key || x.PhoneNumber == request.Key,
                cancellationToken: cancellationToken);
        var user = await userAsyncCursor.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (user is null)
        {
            return Results.NotFound(new ApiResponse
            {
                Message = "Email hoặc số điện thoại không tồn tại"
            });
        }

        if (user.PasswordHash != request.Password.ToSha256())
        {
            return Results.BadRequest(new ApiResponse
            {
                Message = "Mật khẩu không chính xác"
            });
        }

        var userClaims = new IdentityUserClaims
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Policies = []
        };
        
        return Results.Ok(new ApiResponse<SignInResponse>
        {
            Data = new SignInResponse()
            {
                AccessToken = userClaims.AccessToken(_identityOptions.TokenOptions),
                RefreshToken = StringExtensions.RandomString(36),
            },
            Message = "Đăng nhập thành công"
        });
    }
}

public class SignInResponse
{
    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;
}