using Microsoft.Extensions.DependencyInjection;
using YAEC.Shared.Mediator.Abstractions;

namespace YAEC.Shared.Mediator;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SendAsync(IRequest request, CancellationToken ct = default)
    {
        var handlerGenericType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType());
        var invokeMethod = (Task)InvokeMethod(request, handlerGenericType, ct);
        await invokeMethod;
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
    {
        var handlerGenericType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var invokeMethod = (Task<TResponse>)InvokeMethod(request, handlerGenericType, ct);
        return await invokeMethod;
    }

    private object InvokeMethod(IRequest request, Type handlerGenericType, CancellationToken ct = default)
    {
        var handler =  _serviceProvider.CreateScope().ServiceProvider.GetService(handlerGenericType);
        if (handler is null) throw new InvalidOperationException($"No handler registered for {request.GetType()}");
        
        var method = handlerGenericType.GetMethod("HandleAsync");
        if (method is null) throw new InvalidOperationException($"No method implement for {handlerGenericType}");
        
        return method.Invoke(handler, [request, ct])!;
    }
}