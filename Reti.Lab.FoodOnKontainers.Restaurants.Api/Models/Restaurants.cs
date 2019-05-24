using System;
using System.Collections.Generic;

namespace Reti.Lab.FoodOnKontainers.Restaurants.Api.Models
{
    public partial class Restaurants
    {
        public Restaurants()
        {
            RestaurantsMenu = new HashSet<RestaurantsMenu>();
        }

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

        public RestaurantTypes IdRestaurantTypeNavigation { get; set; }
        public ICollection<RestaurantsMenu> RestaurantsMenu { get; set; }
    }
}
