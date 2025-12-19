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
    public class GetAllCartQueryHandlerTests
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock = new();

        [DataTestMethod]
        // DataRow: NumberOfCarts, ExpectedCount, ExpectedException
        [DataRow(3, 3, null)]  // Case: Multiple carts found
        [DataRow(0, 0, null)]  // Case: No carts in DB
        [DataRow(1, 1, null)]  // Case: Single cart found
        public async Task GetAllCarts_TestVariousInputs(
            int itemCount,
            int expectedCount,
            Type? expectedException)
        {
            // Arrange
            var query = new GetAllCartQuery();
            var handler = new GetAllCartQueryHandler(_cartRepositoryMock.Object);

            // Generate mock cart data
            var mockCarts = Enumerable.Range(1, itemCount)
                .Select(i => CartEntity.CreateCart(clientId: i))
                .ToList();

            _cartRepositoryMock
                .Setup(repo => repo.GetCarts())
                .ReturnsAsync(mockCarts);

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
                    // Check type dynamically as per format
                    Assert.IsInstanceOfType(ex, expectedException);
                }
            }
            else
            {
                // Execute the handler
                var result = await handler.Handle(query, CancellationToken.None);

                // Assertions
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedCount, result.Count());

                // Verify the repository was called once
                _cartRepositoryMock.Verify(repo => repo.GetCarts(), Times.Once);
            }
        }
    }
}
