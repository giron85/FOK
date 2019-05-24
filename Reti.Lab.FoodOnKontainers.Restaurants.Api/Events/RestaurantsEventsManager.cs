using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Reti.Lab.FoodOnKontainers.Events.DTO;
using Reti.Lab.FoodOnKontainers.Events.DTO.Restaurants;
using Reti.Lab.FoodOnKontainers.Events.DTO.Utilities;
using Reti.Lab.FoodOnKontainers.Middleware.Dto;
using Reti.Lab.FoodOnKontainers.Restaurants.Api.Services;

namespace Reti.Lab.FoodOnKontainers.Restaurants.Api.Events
{

    public interface IRestaurantsEventsManager
    {
        void ProductPriceChanged(PriceChangedEvent priceChanged);
        void ProductAvailabilityChanged(ProductAvailabilityChangedEvent productAvailabilityChanged);
    }

    public class RestaurantsEventsManager : IRestaurantsEventsManager
    {
        private IModel channel;
        private ConnectionFactory cFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RestaurantsEventsManager([FromServices]RabbitMQConfigurations rabbitMQConfiguration, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            cFactory = new ConnectionFactory() { HostName = rabbitMQConfiguration.HostName, Port = rabbitMQConfiguration.Port };
            using (var connection = cFactory.CreateConnection())
            using (channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: ApplicationEvents.BasketQueue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.QueueDeclare(queue: ApplicationEvents.RestaurantQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += Restaurant_Received;
                channel.BasicConsume(ApplicationEvents.RestaurantQueue, true, consumer);
            }
        }

        #region received events
        private async void Restaurant_Received(object sender, BasicDeliverEventArgs e)
        {
            //handle average rating changed
            Console.WriteLine($"A message of type {e.BasicProperties?.Type} has been received received");

            switch (e?.BasicProperties.Type)
            {
                case nameof(RatingChangedEvent):
                    await HandleRatingEvent(MessageSerializationHelper.DeserializeObjectFromBin<RatingChangedEvent>(e.Body));
                    break;
               
                    throw new NotImplementedException($"Event not supported: {e.BasicProperties?.Type}");
            }

        }

        private async Task HandleRatingEvent(RatingChangedEvent ratingChangedEvent)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IRestaurantService restaurantService = scope.ServiceProvider.GetRequiredService<IRestaurantService>();

                await restaurantService.UpdateRestaurantAverageRating(ratingChangedEvent.idRestaurant, ratingChangedEvent.averageRating);
            }
        }

        #endregion

        #region pusblished events

        public void ProductAvailabilityChanged(ProductAvailabilityChangedEvent productAvailabilityChanged)
        {
            using (var connection = cFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                IBasicProperties props = channel.CreateBasicProperties();
                props.Type = nameof(ProductAvailabilityChangedEvent);

                var body = MessageSerializationHelper.SerializeObjectToBin(productAvailabilityChanged);

                 
                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: ApplicationEvents.BasketQueue,
                                     basicProperties: props,
                                     body: body);
            }

        }

        public void ProductPriceChanged(PriceChangedEvent priceChanged)
        {
            using (var connection = cFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                IBasicProperties props = channel.CreateBasicProperties();
                props.Type = nameof(PriceChangedEvent);

                var body = MessageSerializationHelper.SerializeObjectToBin(priceChanged);

                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: ApplicationEvents.BasketQueue,
                                     basicProperties: props,
                                     body: body);
            }
        }
        #endregion
    }
}
