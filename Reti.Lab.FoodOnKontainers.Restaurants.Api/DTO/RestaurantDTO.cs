using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reti.Lab.FoodOnKontainers.Restaurants.Api.DTO
{
    public class RestaurantDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
        public int IdRestaurantType { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool? Enabled { get; set; }
        public decimal? AverageRating { get; set; }
    }
}
