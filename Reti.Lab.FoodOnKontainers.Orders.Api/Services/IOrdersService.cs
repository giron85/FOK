using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reti.Lab.FoodOnKontainers.Orders.Api.Services
{
    public interface IOrdersService
    {
        Task<List<Models.Order>> GetOrders();
        Task<List<Models.Order>> GetOrders(DTO.OrderFilter order);
        Task<Models.Order> GetOrder(int idOrder);
        Task<int> AddOrder(DTO.Order order);
        Task UpdateStatusOrder(int idOrder, DTO.Status newStatus);
        DTO.Order MapOrderModel(Models.Order orderDb);
    }
}
