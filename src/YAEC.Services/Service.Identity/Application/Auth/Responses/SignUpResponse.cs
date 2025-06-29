namespace Service.Identity.Application.Auth.Responses;

public class SignUpResponse
{
    public string Id { get; set; } = null!;

    public long SubId { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}