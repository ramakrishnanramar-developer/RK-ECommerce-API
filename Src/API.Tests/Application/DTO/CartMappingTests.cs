using Domain.Enitities;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Mapping;

namespace API.Tests.Application
{
    [TestClass]
    public class CartMappingTests
    {
        [DataTestMethod]
        // DataRow: ClientType, AnnualRevenue, Quantity, ExpectedUnitPrice, ExpectedTotal
        [DataRow("Individual", 0, 2, 10, 20)]
        [DataRow("Professional", 60000000, 3, 20, 60)] // High Revenue Tier
        [DataRow("Professional", 10000, 5, 15, 75)] // Low Revenue Tier
        public void ToDto_TestVariousInputs(
            string clientType,
            double annualRevenue,
            int quantity,
            int expectedUnitPrice,
            int expectedTotal)
        {
            // Arrange
            decimal expectedUnit = (decimal)expectedUnitPrice;
            decimal expectedCartTotal = (decimal)expectedTotal;

            // 1. Create Client based on type
            ClientEntity client = clientType == "Individual"
                ? ClientEntity.CreateIndividual(1, "John", "Doe")
                : ClientEntity.CreateProfessional(1, "CitiCorp", "BR123", (decimal)annualRevenue, "VAT123");

            // 2. Setup Product with tiered pricing
            var product = new ProductsEntity
            {
                Id = 101,
                Name = "Trade Terminal",
                IndividualPrice = 10m,
                ProfessionalHighRevenuePrice = 20m,
                ProfessionalLowRevenuePrice = 15m
            };

            // 3. Setup Cart and Items
            var cart = CartEntity.CreateCart(client.Id);
            cart.Id = 500;
            cart.Client = client;

            var cartItem = new CartItemsEntity(product.Id, quantity, cart.Id)
            {
                Product = product
            };

            cart.CartItems = new List<CartItemsEntity> { cartItem };

            // Act
            var dto = cart.ToDto();

            // Assert
            Assert.IsNotNull(dto);
            Assert.AreEqual(cart.Id, dto.Id);
            Assert.AreEqual(1, dto.Items.Count);

            var firstItem = dto.Items.First();
            Assert.AreEqual(expectedUnit, firstItem.UnitPrice, $"UnitPrice mismatch for {clientType}");
            Assert.AreEqual(expectedCartTotal, firstItem.TotalUnitPrice, "Item TotalUnitPrice calculation failed");
            Assert.AreEqual(expectedCartTotal, dto.Total, "Cart Total summation failed");
        }

        [TestMethod]
        public void ToDto_EmptyCart_ShouldReturnZeroTotal()
        {
            // Arrange
            var cart = CartEntity.CreateCart(1);
            cart.CartItems = new List<CartItemsEntity>();

            // Act
            var dto = cart.ToDto();

            // Assert
            Assert.AreEqual(0, dto.Items.Count);
            Assert.AreEqual(0, dto.Total);
        }
    }
}
