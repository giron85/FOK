using System;
using System.Collections.Generic;

namespace Reti.Lab.FoodOnKontainers.Orders.Api.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderItem = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public int IdRestaurant { get; set; }
        public string RestaurantName { get; set; }
        public int IdUser { get; set; }
        public string UserName { get; set; }
        public int IdStatus { get; set; }
        public decimal Price { get; set; }
        public string RestaurantAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime DeliveryRequestedDate { get; set; }

        public Status IdStatusNavigation { get; set; }
        public ICollection<OrderItem> OrderItem { get; set; }
    }
}
