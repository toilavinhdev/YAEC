using System.Net;

namespace YAEC.Shared.ValueObjects;

public class ApiResponse
{
    public int Code { get; set; }
    
    public string? Message { get; set; }

    public static ApiResponse Create(string? message = null)
    {
        return new ApiResponse
        {
            Code = (int)HttpStatusCode.OK,
            Message = message
        };
    }
    
    public static ApiResponse Create(int code, string? message = null)
    {
        return new ApiResponse
        {
            Code = code,
            Message = message
        };
    }
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
    
    public static ApiResponse<T> Create(T? data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Data = data,
            Code = (int)HttpStatusCode.OK,
            Message = message
        };
    }
    
    public static ApiResponse<T> Create(T? data, int code, string? message = null)
    {
        return new ApiResponse<T>
        {
            Data = data,
            Code = code,
            Message = message
        };
    }
}