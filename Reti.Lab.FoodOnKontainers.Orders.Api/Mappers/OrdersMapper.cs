using System;
using System.Linq;

namespace Reti.Lab.FoodOnKontainers.Orders.Api.Mappers
{
    internal class OrdersMapper
    {
        internal static Models.Order MapOrderDTO(DTO.Order input)
        {
            Models.Order output = new Models.Order()
            {
                CreateDate = DateTime.UtcNow,
                DeliveryAddress = input.DeliveryAddress,
                DeliveryRequestedDate = input.DeliveryRequestedDate,
                IdRestaurant = input.IdRestaurant,
                IdStatus = (int)DTO.Status.Inserted,
                IdUser = input.IdUser,
                OrderItem = input.OrderItem
                    .Select(item => new Models.OrderItem()
                    {
                        IdMenuItem = item.IdMenuItem,
                        MenuItemName = item.MenuItemName,
                        Price = item.Price,
                        Quantity = item.Quantity
                    })
                    .ToList(),
                Price = input.Price, // non calcolo somma degli item per permettere eventuali sconti
                RestaurantAddress = input.RestaurantAddress,
                RestaurantName = input.RestaurantName,
                UserName = input.UserName
            };
            return output;
        }
    }
}
