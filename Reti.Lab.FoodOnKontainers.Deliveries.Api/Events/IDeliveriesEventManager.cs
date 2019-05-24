using Reti.Lab.FoodOnKontainers.Events.DTO.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reti.Lab.FoodOnKontainers.Deliveries.Api.Events
{
    public interface IDeliveriesEventManager
    {
        /// <summary>
        /// Pubblica l'evento "Presa in consegna da rider" su coda Order
        /// </summary>
        /// <param name="deliveryPickedUp">Dati del messaggio</param>
        void DeliveryPickedUp(DeliveryPickedUpEvent deliveryPickedUp);

        /// <summary>
        /// Pubblica l'evento "Consegna completata da rider" su coda Order
        /// </summary>
        /// <param name="deliveryCompleted">Dati del messaggio</param>
        void DeliveryCompleted(DeliveryCompletedEvent deliveryCompleted);
    }
}
