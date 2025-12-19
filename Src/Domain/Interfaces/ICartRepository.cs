using Domain.Enitities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface ICartRepository
    {
        Task<CartEntity?> GetCartById(int Id);
        Task<IEnumerable<CartEntity>> GetCarts();
        Task<CartEntity> AddCarts(CartEntity entity);
        Task<CartEntity> UpdateCarts(int CartId, CartEntity entity);
        Task<bool> DeleteCarts(int CartId);
    }
}
