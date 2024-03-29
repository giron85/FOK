﻿using Microsoft.AspNetCore.Mvc;
using Reti.Lab.FoodOnKontainers.Orders.Api.Services;
using System.Threading.Tasks;

namespace Reti.Lab.FoodOnKontainers.Orders.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            this.ordersService = ordersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders() // TODO: add filters?
        {
            var result = await ordersService.GetOrders();
            return Ok(result);
        }

        [HttpGet("restaurant/{idRestaurant}")]
        public async Task<IActionResult> GetRestaurantsOrders(
            [FromRoute] int idRestaurant,
            [FromQuery] DTO.OrderFilter order
            )
        {
            order.IdRestaurant = idRestaurant;
            var result = await ordersService.GetOrders(order);
            return Ok(result);
        }

        [HttpGet("user/{idUser}")]
        public async Task<IActionResult> GetUserOrders(
            [FromRoute] int idUser,
            [FromQuery] DTO.OrderFilter order
            )
        {
            order.IdUser = idUser;
            var result = await ordersService.GetOrders(order);
            return Ok(result);
        }

        [HttpGet("{idOrder}")]
        public async Task<IActionResult> GetOrder([FromRoute] int idOrder)
        {

            var orderDb = await ordersService.GetOrder(idOrder);
            DTO.Order orderDto = ordersService.MapOrderModel(orderDb);
            return Ok(orderDto);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddOrder([FromBody] DTO.Order order)
        {
            var result = await ordersService.AddOrder(order);
            return Ok(result);
        }

        [HttpPut("update/order/{idOrder}/{idStatus}")]
        public async Task<IActionResult> UpdateOrder(
            [FromRoute] int idOrder,
            [FromRoute] int idStatus
            )
        {
            await ordersService.UpdateStatusOrder(idOrder, (DTO.Status)idStatus);
            return Ok();
        }

        [HttpDelete("delete/{idOrder}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int idOrder)
        {
            await ordersService.UpdateStatusOrder(idOrder, DTO.Status.Canceled);
            return Ok();
        }
    }
}