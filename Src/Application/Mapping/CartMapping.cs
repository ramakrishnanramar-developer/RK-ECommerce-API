using Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using Application.DTO;
using Domain.Enitities;

namespace Application.Mapping
{
    public static class CartMapping
    {
        public static CartWithItemsDto ToDto(this CartEntity cart)
        {
            var result = new CartWithItemsDto
            {
                Id = cart.Id,
                ClientId = cart.ClientId,
                Items = cart.CartItems != null && cart.CartItems.Count() > 0 ? cart.CartItems.Select(i => new CartItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice= i.GetPriceFor(cart.Client),
                    TotalUnitPrice = i.Quantity * i.GetPriceFor(cart.Client),
                }).ToList() : new List<CartItemDto>()
            };
            result.Total = result.Items.Sum(x => x.TotalUnitPrice);
            return result;
        }
    }
}
