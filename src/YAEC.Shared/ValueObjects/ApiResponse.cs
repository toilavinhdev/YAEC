namespace YAEC.Shared.ValueObjects;

public class ApiResponse
{
    public int Code { get; set; }
    
    public string? Message { get; set; }
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
}