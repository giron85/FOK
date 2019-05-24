using System;
using System.Collections.Generic;

namespace Reti.Lab.FoodOnKontainers.Deliveries.Api.DTO
{
    public class Delivery
    {
        public int Id { get; set; }
        public int IdOrder { get; set; }
        public int? IdRider { get; set; }
        public DateTime? TakeChargeDate { get; set; }
        public DateTime? PickUpDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? DeliveryRequestedDate { get; set; }
        public string PickUpAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public int? IdStatus { get; set; }
        public int? IdRestaurant { get; set; }
        public string RestaurantName { get; set; }
        public string DeliveryName { get; set; }
    }
}
