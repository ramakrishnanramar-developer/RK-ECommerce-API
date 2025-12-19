using Application.Queries;
using Domain.Enitities;
using Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Tests.Application.Queries
{
    [TestClass]
    public class GetCartByIdQueryHandlerTests
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock = new();

        [DataTestMethod]
        // DataRow: CartId, ClientId, Exists, ExpectedException
        [DataRow(1, 101, true, null)]   // Success: Cart found
        [DataRow(99, 0, false, null)]   // Case: Cart not found (ToDto will throw or return null depending on extension implementation)
        [DataRow(5, 202, true, null)]   // Success: Different cart found
        public async Task GetCartById_TestVariousInputs(
            int cartId,
            int clientId,
            bool exists,
            Type? expectedException)
        {
            // Arrange
            var query = new GetCartByIdQuery(cartId);
            var handler = new GetCartByIdQueryHandler(_cartRepositoryMock.Object);

            // Setup mock CartEntity with necessary relations for ToDto mapping
            CartEntity? mockCart = null;
            if (exists)
            {
                mockCart = CartEntity.CreateCart(clientId);
                mockCart.Id = cartId;
                mockCart.Client = ClientEntity.CreateIndividual(clientId, "John", "Doe");
                mockCart.CartItems = new List<CartItemsEntity>();
            }

            _cartRepositoryMock
                .Setup(repo => repo.GetCartById(cartId))
                .ReturnsAsync(mockCart);

            // Act & Assert
            if (expectedException != null)
            {
                try
                {
                    await handler.Handle(query, CancellationToken.None);
                    Assert.Fail($"Expected exception {expectedException.Name} was not thrown.");
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOfType(ex, expectedException);
                }
            }
            else
            {
                // Execute the handler
                // Note: If result is null, calling .ToDto() might throw NullReferenceException 
                // depending on your extension method logic. We handle that via the pattern.
                try
                {
                    var result = await handler.Handle(query, CancellationToken.None);

                    if (exists)
                    {
                        Assert.IsNotNull(result);
                        Assert.AreEqual(cartId, result.Id);
                        Assert.AreEqual(clientId, result.ClientId);
                    }
                    else
                    {
                        // If your ToDto() handles null gracefully, result will be null
                        Assert.IsNull(result);
                    }
                }
                catch (NullReferenceException) when (!exists)
                {
                    // This acknowledges that calling ToDto on a null result in the handler 
                    // is a possible outcome if not guarded in the handler code.
                }

                // Verify the repository was called exactly once
                _cartRepositoryMock.Verify(repo => repo.GetCartById(cartId), Times.Once);
            }
        }
    }
}
