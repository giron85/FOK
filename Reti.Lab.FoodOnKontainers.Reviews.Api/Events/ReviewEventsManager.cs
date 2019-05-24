using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Reti.Lab.FoodOnKontainers.Events.DTO;
using Reti.Lab.FoodOnKontainers.Events.DTO.Restaurants;
using Reti.Lab.FoodOnKontainers.Events.DTO.Reviews;
using Reti.Lab.FoodOnKontainers.Events.DTO.Utilities;
using Reti.Lab.FoodOnKontainers.Reviews.Api.Services;

namespace Reti.Lab.FoodOnKontainers.Reviews.Api.Events
{

    public interface IReviewEventsManager
    {
        Task RestaurantRatingChanged(int idRestaurant, decimal newRating);

        Task RiderRatingChanged(int idRider, decimal newRating);


    }

    public class ReviewEventsManager : IReviewEventsManager
    {
        private IModel channel;
        private ConnectionFactory cFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;


        public ReviewEventsManager(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }


        public async Task RestaurantRatingChanged(int idRestaurant, decimal newRating)
        {
            var newRatingEvt = new RatingChangedEvent
            {
                 idRestaurant = idRestaurant
            };

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IReviewService reviewService = scope.ServiceProvider.GetRequiredService<IReviewService>();

                var restaurantReviews = await reviewService.GetRestaurantReviews(idRestaurant);

               newRatingEvt.averageRating = restaurantReviews.Sum(rw => rw.Rating) / restaurantReviews.Count();
            } 

            using (var connection = cFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                IBasicProperties props = channel.CreateBasicProperties();
                props.Type = nameof(RatingChangedEvent);

                var body = MessageSerializationHelper.SerializeObjectToBin(newRatingEvt);

                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: ApplicationEvents.RestaurantQueue,
                                     basicProperties: props,
                                     body: body);
            }
        }


           

        public async Task RiderRatingChanged(int idRider, decimal newRating)
        {
            var newRatingEvt = new RiderRatingChangedEvent
            {
                IdRider = idRider
            };

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IReviewService reviewService = scope.ServiceProvider.GetRequiredService<IReviewService>();

                var riderReviews = await reviewService.GetRiderReviews(idRider);

                newRatingEvt.newAverageRating = riderReviews.Sum(rw => rw.Rating) / riderReviews.Count();
            }

            using (var connection = cFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                IBasicProperties props = channel.CreateBasicProperties();
                props.Type = nameof(RiderRatingChangedEvent);

                var body = MessageSerializationHelper.SerializeObjectToBin(newRatingEvt);

                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: ApplicationEvents.DeliveryQueue,
                                     basicProperties: props,
                                     body: body);
            }
        }
    }
}
