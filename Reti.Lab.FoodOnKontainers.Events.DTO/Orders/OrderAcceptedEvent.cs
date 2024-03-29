﻿using System;

namespace Reti.Lab.FoodOnKontainers.Events.DTO.Orders
{
    public class OrderAcceptedEvent
    {
        public int idOrder { get; set; }        
        public int idRestaurant { get; set; }
        public string restaurantName { get; set; }
        public string restaurantAddress { get; set; }
        public string deliveryAddress { get; set; }
        public string deliveryName { get; set; }
        public DateTime deliveryRequestedDate { get; set; }
    }
}
