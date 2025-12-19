using Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Enitities
{
    public class ProductsEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal IndividualPrice { get; set; }
        public decimal ProfessionalHighRevenuePrice { get; set; }
        public decimal ProfessionalLowRevenuePrice { get; set; }
        public static ProductsEntity CreateProducts(string name, string type, decimal individualPrice, decimal professionalHighRevenuePrice, decimal professionalLowRevenuePrice)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Product name is required");

            if (!System.Enum.IsDefined(typeof(ProductTypeEnum), type))
                throw new InvalidOperationException("Invalid ProductType");

            if (individualPrice <= 0 || professionalHighRevenuePrice <= 0 || professionalLowRevenuePrice <= 0)
                throw new InvalidOperationException("Prices must be positive");
            return new ProductsEntity
            {
                Name = name,
                Type = type,
                IndividualPrice = individualPrice,
                ProfessionalHighRevenuePrice = professionalHighRevenuePrice,
                ProfessionalLowRevenuePrice = professionalLowRevenuePrice
            };
        }

        // Method to get price for a client
        public decimal GetPriceFor(ClientEntity client)
        {
            if (client.ClientType.ToLower() == nameof(ClientTypeEnum.Individual).ToLower())
                return IndividualPrice;

            if (client.ClientType == nameof(ClientTypeEnum.Professional))
            {
                if (client.IsLargeProfessional())
                    return ProfessionalHighRevenuePrice;

                return ProfessionalLowRevenuePrice;
            }
            throw new InvalidOperationException("Unknown client type");
        }
        [InverseProperty("Product")]
        public virtual ICollection<CartItemsEntity> CartItems { get; set; } = new List<CartItemsEntity>();
    }

}
