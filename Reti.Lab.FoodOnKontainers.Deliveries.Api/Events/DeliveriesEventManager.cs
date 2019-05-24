using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Reti.Lab.FoodOnKontainers.Deliveries.Api.DTO;
using Reti.Lab.FoodOnKontainers.Deliveries.Api.Mappers;
using Reti.Lab.FoodOnKontainers.Deliveries.Api.Services;
using Reti.Lab.FoodOnKontainers.Events.DTO;
using Reti.Lab.FoodOnKontainers.Events.DTO.Delivery;
using Reti.Lab.FoodOnKontainers.Events.DTO.Orders;
using Reti.Lab.FoodOnKontainers.Events.DTO.Reviews;
using Reti.Lab.FoodOnKontainers.Events.DTO.Utilities;
using Reti.Lab.FoodOnKontainers.Middleware.Dto;
using System;
using System.Threading.Tasks;

namespace Reti.Lab.FoodOnKontainers.Deliveries.Api.Events
{
    public class DeliveriesEventManager : IDeliveriesEventManager
    {
        private readonly ConnectionFactory _factory;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DeliveriesEventManager(IServiceScopeFactory serviceScopeFactory, [FromServices]RabbitMQConfigurations rabbitMQConfiguration)
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
                channel.QueueDeclare(queue: ApplicationEvents.OrderQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueDeclare(queue: ApplicationEvents.DeliveryQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += DeliveryEvent_Received;
                channel.BasicConsume(ApplicationEvents.DeliveryQueue, true, consumer);
            }
        }

        private async void DeliveryEvent_Received(object sender, BasicDeliverEventArgs e)
        {
            switch (e?.BasicProperties.Type)
            {
                case nameof(OrderAcceptedEvent):
                    HandleOrderAcceptedEvent(MessageSerializationHelper.DeserializeObjectFromBin<OrderAcceptedEvent>(e.Body));
                    break;
                case nameof(RiderRatingChangedEvent):
                    await HandleRiderRatingChangedEvent(MessageSerializationHelper.DeserializeObjectFromBin<RiderRatingChangedEvent>(e.Body));
                    break;
                default:
                    throw new NotImplementedException($"Event not supported: {e.BasicProperties?.Type}");
            }
        }

        /// <summary>
        /// Pubblica l'evento "Presa in consegna da rider" su coda Order
        /// </summary>
        /// <param name="deliveryPickedUp">Dati del messaggio</param>
        public void DeliveryPickedUp(DeliveryPickedUpEvent deliveryPickedUp)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                IBasicProperties props = channel.CreateBasicProperties();
                props.Type = nameof(DeliveryPickedUpEvent);
                var body = MessageSerializationHelper.SerializeObjectToBin(deliveryPickedUp);

                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: ApplicationEvents.OrderQueue,
                                     basicProperties: props,
                                     body: body);
            }
        }

        /// <summary>
        /// Pubblica l'evento "Consegna completata da rider" su coda Order
        /// </summary>
        /// <param name="deliveryCompleted">Dati del messaggio</param>
        public void DeliveryCompleted(DeliveryCompletedEvent deliveryCompleted)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                IBasicProperties props = channel.CreateBasicProperties();
                props.Type = nameof(DeliveryCompletedEvent);
                var body = MessageSerializationHelper.SerializeObjectToBin(deliveryCompleted);

                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: ApplicationEvents.OrderQueue,
                                     basicProperties: props,
                                     body: body);
            }
        }

        private void HandleOrderAcceptedEvent(OrderAcceptedEvent orderAccepted)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IDeliveryService deliveryService = scope.ServiceProvider.GetRequiredService<IDeliveryService>();
                deliveryService.AddDelivery(DeliveriesMapper.MapNewDeliveryEvent(orderAccepted));
            }
        }

        private async Task HandleRiderRatingChangedEvent(RiderRatingChangedEvent riderRatingChanged)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IRiderService riderService = scope.ServiceProvider.GetRequiredService<IRiderService>();

                var updatedRider = new Rider
                {
                    IdRider = riderRatingChanged.IdRider,
                    AverageRating = riderRatingChanged.newAverageRating
                };

                await riderService.UpdateRider(updatedRider);
            }
        }
    }
}
