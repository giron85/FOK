using GeoAPI.Geometries;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using Reti.Lab.FoodOnKontainers.Deliveries.Api.DAL;
using Reti.Lab.FoodOnKontainers.Deliveries.Api.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reti.Lab.FoodOnKontainers.Deliveries.Api.Services
{
    public class RiderService : IRiderService
    {
        private readonly DeliveriesDbContext deliveriesDbContext;

        public RiderService(DeliveriesDbContext deliveriesDbContext)
        {
            this.deliveriesDbContext = deliveriesDbContext;
        }

        public async Task<List<Models.Rider>> GetRiders(DTO.RiderFilter filter)
        {
            return await deliveriesDbContext.Rider                
                .Where(r => (filter.Active.HasValue && filter.Active.Value == r.Active) || !filter.Active.HasValue)
                .ToListAsync();
        }

        public async Task<Models.Rider> GetRider(int idRider)
        {
            return await deliveriesDbContext.Rider
                .SingleAsync(r => idRider == r.IdRider);
        }

        public async Task<int> AddRider(DTO.Rider rider)
        {
            Models.Rider newRider = RiderMapper.MapNewRiderDTO(rider);
            deliveriesDbContext.Rider.Add(newRider);
            await deliveriesDbContext.SaveChangesAsync();
            return newRider.IdRider;
        }

        public async Task UpdateRider(DTO.Rider rider)
        {
            Models.Rider current = await GetRider(rider.IdRider);
            RiderMapper.MapRiderDTO(current, rider);
            await deliveriesDbContext.SaveChangesAsync();
        }

    }
}
