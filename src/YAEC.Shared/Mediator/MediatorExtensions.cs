using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using YAEC.Shared.Mediator.Abstractions;

namespace YAEC.Shared.Mediator;

public static class MediatorExtensions
{
    public static void AddCoreMediator(this IServiceCollection  services, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetExecutingAssembly();
        services.AddSingleton<IMediator, Mediator>();
        var handlerTypes = assembly.ExportedTypes
            .Where(t => t
                .GetInterfaces()
                .Any(i =>
                    i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();
        foreach (var handlerType in handlerTypes)
        {
            var handlerInterface = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
            services.AddScoped(handlerInterface, handlerType);
        }
    }
}