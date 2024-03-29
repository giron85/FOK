﻿using System;
using System.Collections.Generic;

namespace Reti.Lab.FoodOnKontainers.Restaurants.Api.Models
{
    public partial class RestaurantTypes
    {
        public RestaurantTypes()
        {
            Restaurants = new HashSet<Restaurants>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Restaurants> Restaurants { get; set; }
    }
}
