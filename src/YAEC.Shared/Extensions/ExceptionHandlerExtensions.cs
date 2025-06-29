using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using YAEC.Shared.ValueObjects;

namespace YAEC.Shared.Extensions;

public static class ExceptionHandlerExtensions
{
    public static void UseCoreExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(errApp =>
        {
            errApp.Run(async httpContext =>
            {
                var feature = httpContext.Features.Get<IExceptionHandlerFeature>();

                if (feature is not null)
                {
                    var exception = feature.Error;
                    httpContext.Response.ContentType = "application/problem+json";
                    httpContext.Response.StatusCode = exception switch
                    {
                        UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                        _ => (int)HttpStatusCode.InternalServerError,
                    };
                    await httpContext.Response.WriteAsJsonAsync(
                        new ApiResponse()
                        {
                            Code = exception switch
                            {
                                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                                _ => (int)HttpStatusCode.InternalServerError,
                            },
                            Message = exception.Message
                        });
                }
            });
        });
    }
}