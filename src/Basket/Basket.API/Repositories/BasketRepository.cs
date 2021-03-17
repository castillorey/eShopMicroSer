using Basket.API.Data;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IBasketContext _context;

        public BasketRepository(IBasketContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ShoppingCart> GetBasket(string userName)
        {
            var basket = await _context
                                .Redis.StringGetAsync(userName);

            if (basket.IsNullOrEmpty)
                return null;            

            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }
        
        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            var updated = await _context.Redis.StringSetAsync(basket.UserName, JsonConvert.SerializeObject(basket));

            if (!updated)
            {
                return null;
            }

            return await GetBasket(basket.UserName);
        }

        public async Task<bool> DeleteBasket(string userName)
        {
            return await _context.
                    Redis.KeyDeleteAsync(userName);
        }

    }
}
