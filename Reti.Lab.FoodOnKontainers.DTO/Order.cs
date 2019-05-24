using System;
using System.Collections.Generic;

namespace Reti.Lab.FoodOnKontainers.DTO
{
    public class Order
    {
        public int Id { get; set; }        
        public int IdRestaurant { get; set; }
        public string RestaurantName { get; set; }
        public int IdUser { get; set; }
        public string UserName { get; set; }
        public Status IdStatus { get; set; }
        public decimal Price { get; set; }
        public string RestaurantAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime DeliveryRequestedDate { get; set; }
        public List<OrderItem> OrderItem { get; set; }
    }
}
