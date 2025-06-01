using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Package.WS;

public static class WebSocketExtensions
{
    public static void AddWebSocketHandlers<TAssembly>(this IServiceCollection services)
    {
        services.AddTransient<WebSocketConnectionManager>();
        var handlerTypes = typeof(TAssembly).Assembly
            .ExportedTypes
            .Where(t => t.GetTypeInfo().BaseType == typeof(WebSocketHandler));
        var serviceDescriptors = handlerTypes
            .Select(type => ServiceDescriptor.Singleton(typeof(WebSocketHandler), type));
        services.TryAddEnumerable(serviceDescriptors);
    }
    
    public static void MapWebSocketHandler<THandler>(this WebApplication app, PathString pathMatch)
        where THandler : WebSocketHandler
    {
        var handler = app.Services.GetService<THandler>();
        app.Map(pathMatch, c => c.UseMiddleware<WebSocketMiddleware>(handler));
    }
}