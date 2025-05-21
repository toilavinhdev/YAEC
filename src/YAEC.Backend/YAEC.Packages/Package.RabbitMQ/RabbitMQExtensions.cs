using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Package.RabbitMQ;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class RabbitMQExtensions
{
    public static void AddRabbitMQ(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionFactory>(serviceProvider =>
        {
            var rabbitMQOptions = serviceProvider.GetRequiredService<RabbitMQOptions>();
            var connectionFactory = new ConnectionFactory
            {
                HostName = rabbitMQOptions.Host,
                Port = rabbitMQOptions.Port,
                UserName = rabbitMQOptions.UserName,
                Password = rabbitMQOptions.Password,
                VirtualHost = string.IsNullOrEmpty(rabbitMQOptions.VirtualHost) ? "/" : rabbitMQOptions.VirtualHost,
            };
            return connectionFactory;
        });
    }
}