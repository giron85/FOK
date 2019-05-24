using Microsoft.EntityFrameworkCore;
using Reti.Lab.FoodOnKontainers.Orders.Api.DAL;
using Reti.Lab.FoodOnKontainers.Orders.Api.Events;
using Reti.Lab.FoodOnKontainers.Orders.Api.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reti.Lab.FoodOnKontainers.Orders.Api.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly OrdersDbContext ordersDbContext;
        private readonly IOrdersEventManager eventManager;

        public OrdersService(OrdersDbContext ordersDbContext, IOrdersEventManager eventManager)
        {
            this.ordersDbContext = ordersDbContext;
            this.eventManager = eventManager;
        }

        public async Task<List<Models.Order>> GetOrders()
        {
            return await ordersDbContext.Order
                .ToListAsync();
        }

        public async Task<List<Models.Order>> GetOrders(DTO.OrderFilter order)
        {
            return await ordersDbContext.Order
                .Where(o => (order.Today && o.DeliveryRequestedDate.Date == DateTime.Today) || !order.Today)
                .Where(o => (order.IdRestaurant.HasValue && order.IdRestaurant.Value == o.IdRestaurant) || !order.IdRestaurant.HasValue)
                .Where(o => (order.Status.HasValue && (int)order.Status.Value == o.IdStatus) || !order.Status.HasValue)
                .Where(o => (order.IdUser.HasValue && order.IdUser.Value == o.IdUser) || !order.IdUser.HasValue)
                .ToListAsync();
        }

        public async Task<Models.Order> GetOrder(int idOrder)
        {
            return await ordersDbContext.Order
                .Include(o => o.OrderItem)
                .Include(o => o.IdStatusNavigation)
                .SingleAsync(o => idOrder == o.Id);
        }

        public async Task<int> AddOrder(DTO.Order order)
        {
            Models.Order newOrder = OrdersMapper.MapOrderDTO(order);
            ordersDbContext.Order.Add(newOrder);
            await ordersDbContext.SaveChangesAsync();
            return newOrder.Id;
        }

        public async Task UpdateStatusOrder(int idOrder, DTO.Status newStatus)
        {
            Models.Order order = await GetOrder(idOrder);
            order.IdStatus = (int)newStatus;
            await ordersDbContext.SaveChangesAsync();

            PublishOrderStatusChange(order);
        }        

        public DTO.Order MapOrderModel(Models.Order orderDb)
        {
            DTO.Order output = new DTO.Order()
            {
                Id = orderDb.Id,
                DeliveryAddress = orderDb.DeliveryAddress,
                IdRestaurant = orderDb.IdRestaurant,
                IdStatus = DTO.Status.Inserted,
                IdUser = orderDb.IdUser,
                OrderItem = orderDb.OrderItem
                    .Select(item => new DTO.OrderItem()
                    {
                        IdMenuItem = item.IdMenuItem,
                        MenuItemName = item.MenuItemName,
                        Price = item.Price,
                        Quantity = item.Quantity
                    })
                    .ToList(),
                Price = orderDb.Price, // non calcolo somma degli item per permettere eventuali sconti
                RestaurantAddress = orderDb.RestaurantAddress,
                RestaurantName = orderDb.RestaurantName,
                UserName = orderDb.UserName
            };

            return output;
        }

        private void PublishOrderStatusChange(Models.Order order)
        {
            switch ((DTO.Status)order.IdStatus)
            {
                case DTO.Status.Accepted:
                    var orderAcceptedMessage = new FoodOnKontainers.Events.DTO.Orders.OrderAcceptedEvent()
                    {
                        deliveryAddress = order.DeliveryAddress,
                        deliveryName = order.UserName,
                        deliveryRequestedDate = order.DeliveryRequestedDate,
                        idOrder = order.Id,
                        idRestaurant = order.IdRestaurant,
                        restaurantAddress = order.RestaurantAddress,
                        restaurantName = order.RestaurantName
                    };
                    eventManager.OrderAccepted(orderAcceptedMessage);
                    break;
                case DTO.Status.Rejected:
                    var orderRejectedMessage = new FoodOnKontainers.Events.DTO.Orders.OrderRejectedEvent()
                    {
                        idOrder = order.Id,
                        idUser = order.IdUser,
                        price = order.Price
                    };
                    eventManager.OrderRejected(orderRejectedMessage);
                    break;
                case DTO.Status.Inserted:
                case DTO.Status.Delivering:
                case DTO.Status.Completed:
                case DTO.Status.Canceled:
                default:
                    break;
            }
        }        
    }
}
