using Application.DTO;
using Application.Mapping;
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
   
    public class CartItemsRepository(AppDBContext appDBContext) : ICartItemsRepository
    {
        public async Task<CartEntity> GetCartItems(int cartId)
        {
            var result = await appDBContext.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(x => x.Id == cartId);
            return result;
        }
        private async Task<IEnumerable<CartItemsEntity>> GetCartByCartId(int cartId)
        {
            return await appDBContext.CartItems.Where(x => x.CartId == cartId).ToListAsync();
        }
        public async Task<CartEntity> AddCartItems(List<CartItemsEntity> entity)
        {
            int cartId = entity.FirstOrDefault()!.CartId;
            var carts = await GetCartByCartId(cartId);
            foreach (var item in entity)
            {
                var exist = carts.FirstOrDefault(x => x.ProductId == item.ProductId);
                if (exist == null)
                {
                    item.CreatedDate = DateTime.UtcNow;
                    appDBContext.CartItems.Add(item);
                }
                else
                {
                    exist.Quantity = item.Quantity;
                    exist.UpdatedDate = DateTime.UtcNow;
                }
            }
            await appDBContext.SaveChangesAsync();
            return await GetCartItems(cartId);
        }

    }
}
