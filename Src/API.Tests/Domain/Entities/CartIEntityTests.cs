using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enitities;

namespace API.Tests.Domain
{
    [TestClass]
    public class CartIEntityTests
    {

        [TestMethod]
        // Test cases for CreateCart
        [DataRow(0, typeof(InvalidOperationException))]  // Invalid clientId
        [DataRow(1, null)]                                // Valid clientId
        public void CreateCart_TestVariousInputs(
            int clientId, Type? expectedException)
        {
            if (expectedException != null)
            {
                try
                {
                    var cart = CartEntity.CreateCart(clientId);
                }
                catch (Exception ex)
                {
                    // Check type dynamically
                    Assert.IsInstanceOfType(ex, expectedException);
                }
            }
            else
            {
                // Valid input - should create cart
                var cart = CartEntity.CreateCart(clientId);

                Assert.IsNotNull(cart);
                Assert.AreEqual(clientId, cart.ClientId);
            }
        }

    }
}
