using Domain.Enitities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface ICartItemsRepository
    {
        Task<CartEntity> GetCartItems(int cartId);
        Task<CartEntity> AddCartItems(List<CartItemsEntity> entity);
    }
}
