using Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Enitities
{
    public class CartItemsEntity
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }


        public CartItemsEntity(int productId, int quantity, int cartId)
        {
            if (quantity <= 0) throw new InvalidOperationException("Quantity must be positive");
            if (productId <= 0) throw new InvalidOperationException("ProductId should be valid!");
            if (cartId <= 0) throw new InvalidOperationException("CartId should be valid!");

            ProductId = productId;
            Quantity = quantity;
            CartId = cartId;
        }

        // Calculate price using Product pricing rules
        public decimal GetPriceFor(ClientEntity client)
        {
            return Product.GetPriceFor(client);
        }
        [ForeignKey("CartId")]
        [InverseProperty("CartItems")]
        public virtual CartEntity Cart { get; set; } = null!;

        [ForeignKey("ProductId")]
        [InverseProperty("CartItems")]
        public virtual ProductsEntity Product { get; set; } = null!;
    }
}
