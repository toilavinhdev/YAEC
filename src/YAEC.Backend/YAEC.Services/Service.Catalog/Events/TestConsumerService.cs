using System.Text;
using Package.RabbitMQ.Services;
using Package.Shared.Constants.RabbitMQ;
using Package.Shared.Events;
using Package.Shared.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Service.Catalog.Events;

public class TestConsumerService(IServiceProvider serviceProvider) : RabbitMQConsumerService<TestMessageBrokerEvent>(serviceProvider)
{
    protected override string ExchangeName => Exchanges.Default;
    
    protected override async Task SubscribeAsync(IChannel channel, CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Direct,
            durable: false, // Nếu true, exchange sẽ vẫn tồn tại sau khi server khởi động lại
            autoDelete: false, // Nếu true, exchange sẽ tự động bị xóa khi không có thêm bất kỳ queue hoặc exchange nào liên kết
            arguments: null,
            passive:  false, // Nếu true, server sẽ chỉ kiểm tra xem exchange có tồn tại hay không, không tạo mới exchange
            noWait: false, // Nếu true, server sẽ không gửi một response xác nhận sau khi khai báo exchange
            cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: false, // Nếu true, queue sẽ vẫn tồn tại sau khi server khởi động lại
            exclusive: false, // Nếu true, queue chỉ có thể được truy cập bởi một connection duy nhất, và sẽ bị xóa khi kết nối đó đóng
            autoDelete: false, // Nếu true, queue sẽ tự động bị xóa khi không có consumer nào
            arguments: null,
            cancellationToken: cancellationToken);
        await channel.QueueBindAsync(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: RoutingKey,
            arguments: null,
            noWait: false, // Nếu true, server sẽ không gửi một response xác nhận sau khi liên kết hàng đợi với exchange.
            cancellationToken: cancellationToken);
        
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            await Task.CompletedTask;
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body).ToObject<TestMessageBrokerEvent>();
            Logger.LogInformation("Received message: {Message}", message);
        };

        await channel.BasicConsumeAsync(
            queue: QueueName,
            consumer: consumer,
            autoAck: false, // Nếu true, server sẽ tự động gửi lại xác nhận đã nhận tin nhắn. Nếu false, client phải gửi xác nhận thủ công.
            cancellationToken: cancellationToken);
    }
}