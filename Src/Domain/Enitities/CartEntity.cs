
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Enitities
{
    public class CartEntity
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }


        public static CartEntity CreateCart(int clientId)
        {
            if (clientId == 0)
                throw new InvalidOperationException("Client Id is required!");

            return new CartEntity
            {
                ClientId = clientId,
            };
        }
        [InverseProperty("Cart")]
        public virtual ICollection<CartItemsEntity> CartItems { get; set; } = new List<CartItemsEntity>();

        [ForeignKey("ClientId")]
        [InverseProperty("Carts")]
        public virtual ClientEntity Client { get; set; } = null!;
    }
}
