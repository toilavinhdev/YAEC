using MongoDB.Bson;
using MongoDB.Driver;
using Package.MongoDb;
using Package.Shared.Extensions;
using Package.Shared.Mediator;
using Package.Shared.ValueObjects;
using Service.Identity.Domain.Aggregates.UserAggregate;

namespace Service.Identity.Application.UserModule.Commands;

public class SignUpCommand : IRequest<IResult>
{
    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Password { get; set; } = null!;
}

public class SignUpCommandHandler : IRequestHandler<SignUpCommand, IResult>
{
    private readonly IMongoDbService _mongoDbService;

    public SignUpCommandHandler(IMongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    public async Task<IResult> HandleAsync(SignUpCommand request, CancellationToken cancellationToken)
    {
        var userAsyncCursor = await _mongoDbService.Collection<User>()
            .FindAsync(x => x.Email == request.Email || x.PhoneNumber == request.PhoneNumber,
                cancellationToken: cancellationToken);
        var user = await userAsyncCursor.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (user is not null)
        {
            return Results.Conflict(new ApiResponse
            {
                Message = "Email hoặc số điện thoại đã tồn tại"
            });
        }
        
        user = new User
        {
            Id = ObjectId.GenerateNewId().ToString(),
            AutoId = await _mongoDbService.NextSequenceAsync<User>(cancellationToken),
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = request.Password.ToSha256(),
            CreatedAt = DateTimeExtensions.Now,
        };
        await _mongoDbService.Collection<User>().InsertOneAsync(user, cancellationToken: cancellationToken);
        return Results.Ok(new ApiResponse<SignUpResponse>
        {
            Data = new SignUpResponse
            {
                Id = user.Id,
                AutoId = user.AutoId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt
            }
        });
    }
}

public class SignUpResponse
{
    public string Id { get; set; } = null!;

    public long AutoId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}