using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enitities;

namespace API.Tests.Domain
{
    [TestClass]
    public class CartItemsEntityTests
    {
        [TestMethod]
        // Test cases for CartItemsEntity constructor
        [DataRow(0, 1, 1, typeof(InvalidOperationException))]     // Invalid productId
        [DataRow(1, 0, 1, typeof(InvalidOperationException))]     // Invalid quantity
        [DataRow(1, 1, 0, typeof(InvalidOperationException))]     // Invalid cartId
        [DataRow(1, 2, 3, null)]                                   // Valid input
        public void CartItemsEntity_Constructor_TestVariousInputs(
            int productId, int quantity, int cartId, Type? expectedException)
        {
            if (expectedException != null)
            {
                try
                {
                    var cartItem = new CartItemsEntity(productId, quantity, cartId);
                }
                catch (Exception ex)
                {
                    // Check type dynamically
                    Assert.IsInstanceOfType(ex, expectedException);
                }
            }
            else
            {
                // Valid input - should create object
                var cartItem = new CartItemsEntity(productId, quantity, cartId);

                Assert.IsNotNull(cartItem);
                Assert.AreEqual(productId, cartItem.ProductId);
                Assert.AreEqual(quantity, cartItem.Quantity);
                Assert.AreEqual(cartId, cartItem.CartId);
            }
        }

        [DataTestMethod]
        // Test cases for GetPriceFor
        [DataRow("Individual", 10, null, 10)]   // Example: price calculation may throw for some setup
        [DataRow("Professional", 20, null, 15)]                             // Valid case: returns price
        public void GetPriceFor_TestVariousInputs(
            string clientType, int expctedPrice, Type? expectedException, int priceFrmProduct)
        {
            decimal expectedPrice = (decimal)expctedPrice, priceFromProduct = (decimal)priceFrmProduct;
            // Arrange
            ClientEntity client = clientType == "Individual"
                ? ClientEntity.CreateIndividual(1, "John", "Doe")
                : ClientEntity.CreateProfessional(1, "Company", "BR123", 1000m, "VAT123");

            var product = new ProductsEntity
            {
                // Setup Product.GetPriceFor to return a known value for the test
                IndividualPrice = 10,
                ProfessionalHighRevenuePrice = 20,
                ProfessionalLowRevenuePrice = 15
            };

            var cartItem = new CartItemsEntity(1, 1, 1)
            {
                Product = product
            };

            if (expectedException != null)
            {
                try
                {
                    var price = cartItem.GetPriceFor(client);
                }
                catch (Exception ex)
                {
                    // Check type dynamically
                    Assert.IsInstanceOfType(ex, expectedException);
                }
            }
            else
            {
                // Valid input - should return expected price
                var price = cartItem.GetPriceFor(client);
                Assert.AreEqual(priceFromProduct, price);
            }
        }

    }
}
