using Domain.Enitities;
using Domain.Enum;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{

    public class CartRepository(AppDBContext appDBContext) : ICartRepository
    {
        public async Task<CartEntity?> GetCartById(int Id)
        {
            return await appDBContext.Carts.Include(x => x.CartItems).Include(x => x.Client).FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<IEnumerable<CartEntity>> GetCarts()
        {
            return await appDBContext.Carts.ToListAsync();
        }
        public async Task<CartEntity> AddCarts(CartEntity entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            appDBContext.Carts.Add(entity);
            await appDBContext.SaveChangesAsync();
            return await GetCartById(entity.Id);
        }
        public async Task<CartEntity> UpdateCarts(int CartId, CartEntity entity)
        {
            var cart = await GetCartById(CartId);
            if (cart is not null)
            {
                cart.ClientId = entity.ClientId;
                cart.UpdatedDate = DateTime.UtcNow;
                await appDBContext.SaveChangesAsync();
                return cart;
            }
            return await GetCartById(entity.Id);
        }
        public async Task<bool> DeleteCarts(int CartId)
        {
            var Cart = await GetCartById(CartId);
            if (Cart is not null)
            {
                appDBContext.Carts.Remove(Cart);
                return await appDBContext.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}
