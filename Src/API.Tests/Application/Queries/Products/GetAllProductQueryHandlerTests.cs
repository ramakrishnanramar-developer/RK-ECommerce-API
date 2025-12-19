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
    public class GetAllProductQueryHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock = new();

        [DataTestMethod]
        // DataRow: NumberOfProductsToReturn, ExpectedCount, ExpectedException
        [DataRow(3, 3, null)]  // Case: Multiple products found
        [DataRow(0, 0, null)]  // Case: No products in database (Empty list)
        [DataRow(1, 1, null)]  // Case: Single product found
        public async Task GetAllProducts_TestVariousInputs(
            int itemCount,
            int expectedCount,
            Type? expectedException)
        {
            // Arrange
            var query = new GetAllProductQuery();
            var handler = new GetAllProductQueryHandler(_productRepositoryMock.Object);

            // Generate a mock list based on the DataRow count
            var mockProducts = Enumerable.Range(1, itemCount)
                .Select(i => new ProductsEntity
                {
                    Id = i,
                    Name = $"Product {i}",
                    IndividualPrice = 10m * i
                })
                .ToList();

            _productRepositoryMock
                .Setup(repo => repo.GetProducts())
                .ReturnsAsync(mockProducts);

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
                var result = await handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedCount, result.Count());

                // Verify the repository was called exactly once
                _productRepositoryMock.Verify(repo => repo.GetProducts(), Times.Once);
            }
        }
    }
}
