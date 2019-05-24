using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reti.Lab.FoodOnKontainers.Reviews.Api.DTO;
using Reti.Lab.FoodOnKontainers.Reviews.Api.Services;

namespace Reti.Lab.FoodOnKontainers.Reviews.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private IReviewService reviewService;

        public ReviewController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }


        [HttpGet("restaurant/{idRestaurant}")]
        public async Task<IActionResult> GetRestaurantsReviews(int idRestaurant)
        {
            var restaurantReviews = await this.reviewService.GetRestaurantReviews(idRestaurant);
            return Ok(restaurantReviews);
        }

        [HttpGet("ryder/{idRyder}")]
        public async Task<IActionResult> GetRyderReviews(int idRyder)
        {
            var ryderReviews = await this.reviewService.GetRiderReviews(idRyder);
            return Ok(ryderReviews);
        }

        [HttpGet("restaurant/order/{idOrder}")]
        public async Task<IActionResult> GetRestaurantsReviewByOder(int idOrder)
        {
            var restaurantReview = await this.reviewService.GetRestaurantReviewByOrder(idOrder);
            return Ok(restaurantReview);

        }

        [HttpGet("ryder/order/{idOrder}")]
        public async Task<IActionResult> GetRyderReviewByOder(int idOrder)
        {
            var ryderReview = await this.reviewService.GetRiderReviewByOrder(idOrder);
            return Ok(ryderReview);

        }


        [HttpGet("restaurant/user/{idUser}")]
        public async Task<IActionResult> GetRestaurantReviewByUser(int idUser)
        {
            var restaurantReview = await this.reviewService.GetRestaurantsReviewByUser(idUser);
            return Ok(restaurantReview);
        }

        [HttpGet("ryder/user/{idUser}")]
        public async Task<IActionResult> GetRyderReviewByUser(int idUser)
        {
            var ryderReview = await this.reviewService.GetRidersReviewByUser(idUser);
            return Ok(ryderReview);
        }

        [HttpGet("ryder/{idRider}/{idUser}")]
        public async Task<IActionResult> GetRyderReviewByUser(int idRider, int idUser )
        {
            var ryderReview = await this.reviewService.GetRiderReviewByUser(idUser,idRider);
            return Ok(ryderReview);
        }

        [HttpGet("restaurant/{idRestaurant}/{idUser}")]
        public async Task<IActionResult> GetRestaurantReviewByUser(int idRestaurant, int idUser)
        {
            var restaurantReview = await this.reviewService.GetRestaurantReviewByUser(idUser, idRestaurant);
            return Ok(restaurantReview);
        }

        [HttpPost("restaurant/add")]
        public async Task<IActionResult> AddRestaurantReview(RestaurantReviewDto restaurantReviewDto)
        {
            await this.reviewService.AddRestaurantReview(restaurantReviewDto);
            return Ok();

        }

        [HttpPost("ryder/add")]
        public async Task<IActionResult> AddRyderReview(RiderReviewDto ryderReviewDto)
        {
            await this.reviewService.AddRiderReview(ryderReviewDto);
            return Ok();
        }

    }
}