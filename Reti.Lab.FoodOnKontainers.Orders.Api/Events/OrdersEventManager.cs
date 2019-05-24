using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Reti.Lab.FoodOnKontainers.Events.DTO;
using Reti.Lab.FoodOnKontainers.Events.DTO.Delivery;
using Reti.Lab.FoodOnKontainers.Events.DTO.Orders;
using Reti.Lab.FoodOnKontainers.Events.DTO.Utilities;
using Reti.Lab.FoodOnKontainers.Middleware.Dto;
using Reti.Lab.FoodOnKontainers.Orders.Api.Services;
using System;

namespace Reti.Lab.FoodOnKontainers.Orders.Api.Events
{
    public class OrdersEventManager : IOrdersEventManager
    {
        private readonly ConnectionFactory _factory;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public OrdersEventManager(IServiceScopeFactory serviceScopeFactory, [FromServices]RabbitMQConfigurations rabbitMQConfiguration)
        {
            _serviceScopeFactory = serviceScopeFactory;

            _factory = new ConnectionFactory()
            {
                HostName = rabbitMQConfiguration.HostName,
                Port = rabbitMQConfiguration.Port
            };

            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: ApplicationEvents.DeliveryQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueDeclare(queue: ApplicationEvents.PaymentQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueDeclare(queue: ApplicationEvents.UserQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueDeclare(queue: ApplicationEvents.OrderQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += OrderEvent_Received;
                channel.BasicConsume(ApplicationEvents.OrderQueue, true, consumer);
            }
        }

        private void OrderEvent_Received(object sender, BasicDeliverEventArgs e)
        {
            switch (e?.BasicProperties.Type)
            {
                case nameof(DeliveryPickedUpEvent):
                    HandleStatusChangedEvent(MessageSerializationHelper.DeserializeObjectFromBin<DeliveryPickedUpEvent>(e.Body).idOrder, DTO.Status.Delivering);
                    break;
                case nameof(DeliveryCompletedEvent):
                    HandleStatusChangedEvent(MessageSerializationHelper.DeserializeObjectFromBin<DeliveryCompletedEvent>(e.Body).idOrder, DTO.Status.Completed);
                    break;
                default:
                    throw new NotImplementedException($"Event not supported: {e.BasicProperties?.Type}");
            }
        }

        /// <summary>
        /// Pubblica l'evento "Ordine Accettato da gestore" su coda Delivery
        /// </summary>
        /// <param name="orderRejected">Dati del messaggio</param>
        public void OrderAccepted(OrderAcceptedEvent orderAccepted)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                IBasicProperties props = channel.CreateBasicProperties();
                props.Type = nameof(OrderAcceptedEvent);
                var body = MessageSerializationHelper.SerializeObjectToBin(orderAccepted);
                
                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: ApplicationEvents.DeliveryQueue,
                                     basicProperties: props,
                                     body: body);
            }
        }

        /// <summary>
        /// Pubblica l'evento "Ordine Rifiutato da gestore" su coda Payment e User
        /// </summary>
        /// <param name="orderRejected">Dati del messaggio</param>
        public void OrderRejected(OrderRejectedEvent orderRejected)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                IBasicProperties props = channel.CreateBasicProperties();
                props.Type = nameof(OrderRejectedEvent);
                var body = MessageSerializationHelper.SerializeObjectToBin(orderRejected);
                
                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: ApplicationEvents.PaymentQueue,
                                     basicProperties: props,
                                     body: body);
                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: ApplicationEvents.UserQueue,
                                     basicProperties: props,
                                     body: body);
            }
        }

        private void HandleStatusChangedEvent(int idOrder, DTO.Status status)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IOrdersService ordersService = scope.ServiceProvider.GetRequiredService<IOrdersService>();
                ordersService.UpdateStatusOrder(idOrder, status);
            }
        }
    }
}
