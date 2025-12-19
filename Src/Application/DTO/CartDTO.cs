using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO
{
    public class CartDTO
    {
        public int ClientId { get; set; }
    }
    public class CartWithIdDTO : CartDTO
    {
        public int Id { get; set; }
    }
    public class CartWithItemsDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public decimal Total { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
    }
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalUnitPrice { get; set; }
    }

}
