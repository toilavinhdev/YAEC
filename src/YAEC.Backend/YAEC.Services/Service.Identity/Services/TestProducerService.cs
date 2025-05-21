using System.Text;
using Package.RabbitMQ.Services;
using Package.Shared.Constants.RabbitMQ;
using Package.Shared.Extensions;
using Package.Shared.Models.Events;
using RabbitMQ.Client;

namespace Service.Identity.Services;

public class TestProducerService(IServiceProvider serviceProvider) : RabbitMQProducerService<TestMessageBrokerEvent>(serviceProvider)
{
    protected override string ExchangeName => Exchanges.Default;

    protected override async Task PublishAsync(IChannel channel, TestMessageBrokerEvent message, CancellationToken cancellationToken = default)
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
        var body = Encoding.UTF8.GetBytes(message.ToJson());
        await channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: RoutingKey,
            body: body,
            cancellationToken: cancellationToken);
    }
}