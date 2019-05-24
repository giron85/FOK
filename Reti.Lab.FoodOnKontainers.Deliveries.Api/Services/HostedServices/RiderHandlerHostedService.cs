using Microsoft.Extensions.Hosting;
using Reti.Lab.FoodOnKontainers.Deliveries.Api.Services;
using Reti.Lab.FoodOnKontainers.Middleware;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Reti.Lab.FoodOnKontainers.Deliveries.Api.Services.HostedServices
{
    public class RiderHandlerHostedService : BackgroundService
    {
        private readonly IDeliveryService deliveryService;
        private readonly IRiderService riderService;
        private readonly ILogService logger;

        public RiderHandlerHostedService(
            IDeliveryService deliveryService, 
            IRiderService riderService,
            ILogService logger)
        {
            this.deliveryService = deliveryService;
            this.riderService = riderService;
            this.logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"I'm {nameof(RiderHandlerHostedService)}");
            while (!stoppingToken.IsCancellationRequested)
            {
                await AssignDeliveryToRider();
                await Task.Delay(10000, stoppingToken);
            }
        }

        private async Task AssignDeliveryToRider()
        {
            var deliveryToProcess = deliveryService.GetDeliveriesToAssign().FirstOrDefault();
            if (deliveryToProcess != null)
            {
                logger.Log($"Processing delivery {deliveryToProcess.Id}", Microsoft.Extensions.Logging.LogLevel.Information, System.Net.HttpStatusCode.OK, nameof(RiderHandlerHostedService));
                var availableRiders = await riderService.GetRiders(new DTO.RiderFilter() { Active = true });
                if (availableRiders.Any())
                {
                    // TODO: Calcolare in base a distanza da punti di consegna, quando saranno geography anche loro :)
                    var rider = availableRiders.OrderByDescending(r => r.AverageRating).First();
                    await deliveryService.UpdateDelivery(new DTO.Delivery()
                    {
                        Id = deliveryToProcess.Id,
                        IdRider = rider.IdRider,
                        TakeChargeDate = DateTime.UtcNow                        
                    });
                    logger.Log($"Assigned rider {availableRiders} to delivery {deliveryToProcess.Id}", Microsoft.Extensions.Logging.LogLevel.Information, System.Net.HttpStatusCode.OK, nameof(RiderHandlerHostedService));
                }
                else
                {
                    logger.Log($"No available riders for delivery {deliveryToProcess.Id}", Microsoft.Extensions.Logging.LogLevel.Warning, System.Net.HttpStatusCode.OK, nameof(RiderHandlerHostedService));
                }
            }
        }

        
    }
}
