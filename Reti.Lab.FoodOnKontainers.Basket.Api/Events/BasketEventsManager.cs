using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Reti.Lab.FoodOnKontainers.Basket.Api.Basket;
using Reti.Lab.FoodOnKontainers.Events.DTO;
using Reti.Lab.FoodOnKontainers.Events.DTO.Restaurants;
using Reti.Lab.FoodOnKontainers.Events.DTO.Utilities;
using Reti.Lab.FoodOnKontainers.Middleware.Dto;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Reti.Lab.FoodOnKontainers.Basket.Api.Events
{
    public class BasketEventsManager : BackgroundService
    {
        private readonly ConnectionFactory _factory;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BasketEventsManager(IServiceScopeFactory serviceScopeFactory, [FromServices]RabbitMQConfigurations rabbitMQConfiguration)
        {
            _serviceScopeFactory = serviceScopeFactory;

            _factory = new ConnectionFactory()
            {
                HostName = rabbitMQConfiguration.HostName,
                Port = rabbitMQConfiguration.Port
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: ApplicationEvents.BasketQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += RestaurantsEvents_Received;
                channel.BasicConsume(ApplicationEvents.BasketQueue, true, consumer);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        private void RestaurantsEvents_Received(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine($"Message of type {e.BasicProperties?.Type} received from RestaurantsService");

            switch (e?.BasicProperties.Type)
            {
                case nameof(PriceChangedEvent):
                    HandlePriceChangedEvent(MessageSerializationHelper.DeserializeObjectFromBin<PriceChangedEvent>(e.Body));
                    break;
                case nameof(ProductAvailabilityChangedEvent):
                    HandleProductAvailabilityChangedEvent(MessageSerializationHelper.DeserializeObjectFromBin<ProductAvailabilityChangedEvent>(e.Body));
                    break;
                default:
                    throw new NotImplementedException($"Event not supported: {e.BasicProperties?.Type}");
            }
        }

        private void HandlePriceChangedEvent(PriceChangedEvent e)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IBasketService basketService = scope.ServiceProvider.GetRequiredService<IBasketService>();
                var keys = basketService.GetItems();
                foreach (var key in keys)
                {
                    var basket = basketService.GetBasket(int.Parse(key));
                    if (basket.Result.restaurantId == e.restaurantId && basket.Result.basketItems.Any(ba => ba.menuItemId == e.itemId))
                    {
                        basketService.UpdateItem(int.Parse(key), e.itemId, e.newPrice);
                    }
                }
            }
        }

        private void HandleProductAvailabilityChangedEvent(ProductAvailabilityChangedEvent e)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IBasketService basketService = scope.ServiceProvider.GetRequiredService<IBasketService>();
                var keys = basketService.GetItems();
                foreach (var key in keys)
                {
                    var basket = basketService.GetBasket(int.Parse(key));
                    if (basket.Result.restaurantId == e.restaurantId && basket.Result.basketItems.Any(ba => ba.menuItemId == e.itemId))
                    {
                        basketService.UpdateItem(int.Parse(key), e.itemId, e.available);
                    }
                }
            }
        }
    }
}
