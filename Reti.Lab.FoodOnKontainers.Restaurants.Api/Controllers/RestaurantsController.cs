using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reti.Lab.FoodOnKontainers.Restaurants.Api.DTO;
using Reti.Lab.FoodOnKontainers.Restaurants.Api.Services;


namespace Reti.Lab.FoodOnKontainers.Restaurants.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private IRestaurantService restaurantService;

        public RestaurantsController(IRestaurantService restaurantService)
        {
            this.restaurantService = restaurantService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetRestaurants()
        {
            var restaurants = await restaurantService.GetRestaurants();
            return Ok(restaurants);
        }

        [HttpGet("detail/{idRestaurant}")]
        public async Task<IActionResult> GetRestaurantDetail(int idRestaurant)
        {
            var restaurantDetail = await restaurantService.GetRestaurantDetail(idRestaurant);
            return Ok(restaurantDetail);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddRestaurant(RestaurantDTO restaurant)
        {

            var newRestaurant = new Models.Restaurants
            {
                Address = restaurant.Address,
                Email = restaurant.Email,
                Enabled = true,
                IdRestaurantType = restaurant.IdRestaurantType,
                Latitude = restaurant.Latitude,
                Longitude = restaurant.Longitude,
                Name = restaurant.Name,
                PhoneNumber = restaurant.PhoneNumber
                
            };

            await restaurantService.AddRestaurant(newRestaurant);
            return Ok();
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateRestaurant(RestaurantDTO restaurant)
        {

            var updatedRestaurant = new Models.Restaurants
            {
                Id = restaurant.Id,
                Address = restaurant.Address,
                Email = restaurant.Email,
                Enabled = true,
                IdRestaurantType = restaurant.IdRestaurantType,
                Latitude = restaurant.Latitude,
                Longitude = restaurant.Longitude,
                Name = restaurant.Name,
                PhoneNumber = restaurant.PhoneNumber,
                AverageRating = restaurant.AverageRating

            };

            await restaurantService.UpdateRestaurant(updatedRestaurant);
            return Ok();
        }

        [HttpPut("update/rating")]
        public async Task<IActionResult> UpdateRestaurantRating(RestaurantRatingDTO restaurantRating)
        {

            await restaurantService.UpdateRestaurantAverageRating(restaurantRating.idRestaurant, restaurantRating.averageRating);
            return Ok();
        }

        [HttpDelete("delete/{idRestaurant}")]
        public async Task<IActionResult> DeleteRestaurant(int idRestaurant)
        {
            await restaurantService.DisableRestaurant(idRestaurant);
            return Ok();
        }

    }
}