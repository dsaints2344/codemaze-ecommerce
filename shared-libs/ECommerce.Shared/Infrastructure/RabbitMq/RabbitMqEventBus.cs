using System.Text.Json;
using ECommerce.Shared.Infrastructure.EventBus;
using ECommerce.Shared.Infrastructure.EventBus.Abstractions;
using RabbitMQ.Client;

namespace ECommerce.Shared.Infrastructure.RabbitMq;

public class RabbitMqEventBus : IEventBus
{
    private const string ExchangeName = "ecommerce-exchange";

    private readonly IRabbitMqConnection _rabbitMqConnection;

    public RabbitMqEventBus(IRabbitMqConnection rabbitMqConnection)
    {
        _rabbitMqConnection = rabbitMqConnection;
    }

    public async Task PublishAsync(Event @event)
    {
        await using var channel = await _rabbitMqConnection.Connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: "fanout",
            durable: false,
            autoDelete: false,
            arguments: null);

        var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType());

        await channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: string.Empty,
            mandatory: false,
            body: body);
    }
}
