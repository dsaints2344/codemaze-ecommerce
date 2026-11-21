using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Basket.Service.IntegrationEvents;
using Basket.Service.Infrastructure.Data;
using ECommerce.Shared.Infrastructure.RabbitMq;

namespace Basket.Service.Infrastructure.RabbitMq;

public class RabbitMqHostedService : IHostedService
{
    private const string ExchangeName = "ecommerce-exchange";
    private const string QueueName = "basket-microservice";

    private readonly IServiceProvider _serviceProvider;

    public RabbitMqHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Factory.StartNew(async () =>
        {
            var rabbitMQConnection = _serviceProvider.GetRequiredService<IRabbitMqConnection>();

            var channel = await rabbitMQConnection.Connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: "fanout",
                durable: false,
                autoDelete: false,
                arguments: null);

            await channel.QueueDeclareAsync(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += OnMessageReceived;

            await channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: true,
                consumer: consumer);

            await channel.QueueBindAsync(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: string.Empty);
        },
        TaskCreationOptions.LongRunning);

        return Task.CompletedTask;
    }

    private Task OnMessageReceived(object? sender, BasicDeliverEventArgs eventArgs)
    {
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        var @event = JsonSerializer.Deserialize(message, typeof(OrderCreatedEvent)) as OrderCreatedEvent;

        using var scope = _serviceProvider.CreateScope();

        var basketStore = scope.ServiceProvider.GetRequiredService<IBasketStore>();

        basketStore.DeleteCustomerBasket(@event.CustomerId);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
