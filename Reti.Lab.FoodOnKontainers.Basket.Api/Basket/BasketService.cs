using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Reti.Lab.FoodOnKontainers.Basket.Api.Basket.Repository;
using Reti.Lab.FoodOnKontainers.Basket.Api.Dto;
using Reti.Lab.FoodOnKontainers.Basket.Api.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reti.Lab.FoodOnKontainers.Basket.Api.Basket
{
    public class BasketService : IBasketService
    {
        private IDistributedCache basketCache;
        private RedisConfig redisConfig;
        private readonly IBasketRepository _basketRepository;

        public BasketService(
            IDistributedCache _basketCache,
            IOptions<RedisConfig> redisConfig,
            IBasketRepository basketRepository
            )
        {
            basketCache = _basketCache;
            this.redisConfig = redisConfig.Value;
            _basketRepository = basketRepository;
        }

        public async Task<UserBasket> GetBasket(int userId)
        {
            try
            {
                UserBasket cachedBasket = null;
                var userBasket = await basketCache.GetStringAsync(userId.ToString());
                if(!string.IsNullOrWhiteSpace(userBasket))
                    cachedBasket = JsonConvert.DeserializeObject<UserBasket>(userBasket);
                return cachedBasket;
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

        public async Task SetBasket(UserBasket userBasket)
        {
            try
            {
                var basketAsString = JsonConvert.SerializeObject(userBasket);

                await SaveBasket(userBasket.userId.ToString(), JsonConvert.SerializeObject(basketAsString));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ClearBasket(int userId)
        {
            try
            {
                await basketCache.RemoveAsync(userId.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AddItem(UserBasket newItem)
        {
            try
            {
                var userBasket = await basketCache.GetStringAsync(newItem.userId.ToString());
                if (string.IsNullOrWhiteSpace(userBasket))
                {
                    await SetBasket(newItem);
                    userBasket = await basketCache.GetStringAsync(newItem.userId.ToString());
                }

                UserBasket cachedBasket = JsonConvert.DeserializeObject<UserBasket>(userBasket);

                foreach (var item in newItem.basketItems)
                    cachedBasket.basketItems.Add(item);

                await SaveBasket(newItem.userId.ToString(), JsonConvert.SerializeObject(cachedBasket));


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task UpdateItem(int userId, int itemId, int newQuantity)
        {
            try
            {
                var userBasket = await basketCache.GetStringAsync(userId.ToString());
                UserBasket cachedBasket = JsonConvert.DeserializeObject<UserBasket>(userBasket);

                cachedBasket.basketItems.Single(itm => itm.menuItemId == itemId).quantity = newQuantity;

                await SaveBasket(userId.ToString(), JsonConvert.SerializeObject(cachedBasket));

            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

        public async Task UpdateItem(int userId, int itemId, decimal newPrice)
        {
            try
            {
                var userBasket = await basketCache.GetStringAsync(userId.ToString());
                UserBasket cachedBasket = JsonConvert.DeserializeObject<UserBasket>(userBasket);

                cachedBasket.basketItems.Single(itm => itm.menuItemId == itemId).price = newPrice;

                await SaveBasket(userId.ToString(), JsonConvert.SerializeObject(cachedBasket));

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task UpdateItem(int userId, int itemId, bool available)
        {
            try
            {
                var userBasket = await basketCache.GetStringAsync(userId.ToString());
                UserBasket cachedBasket = JsonConvert.DeserializeObject<UserBasket>(userBasket);

                cachedBasket.basketItems.Single(itm => itm.menuItemId == itemId).available = available;

                await SaveBasket(userId.ToString(), JsonConvert.SerializeObject(cachedBasket));

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task RemoveItem(int userId, int itemId)
        {
            try
            {
                var userBasket = await basketCache.GetStringAsync(userId.ToString());
                UserBasket cachedBasket = JsonConvert.DeserializeObject<UserBasket>(userBasket);

                var itemToRem = cachedBasket.basketItems.Single(itm => itm.menuItemId == itemId);

                cachedBasket.basketItems.Remove(itemToRem);

                await SaveBasket(userId.ToString(), JsonConvert.SerializeObject(cachedBasket)); 
               

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<string> GetItems()
        {
            return _basketRepository.GetUsers();
        }

        private async Task SaveBasket(string key, object value)
        {
            await basketCache.SetStringAsync(key, JsonConvert.SerializeObject(value), GetCacheEntryOptions());
        }

        private DistributedCacheEntryOptions GetCacheEntryOptions()
        {
            return new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(this.redisConfig.expirationMinutes)
            };
        }
           
      
    }
}
