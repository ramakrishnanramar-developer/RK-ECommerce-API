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
    public class GetProductByIdQueryHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock = new();

        [DataTestMethod]
        // DataRow: RequestedId, DoesExistInDb, ExpectedName, ExpectedException
        [DataRow(1, true, "Core Banking Module", null)]  // Case: Product found
        [DataRow(999, false, null, null)]               // Case: Product not found (returns null)
        [DataRow(5, true, "STP Logic Engine", null)]     // Case: Different product found
        public async Task GetProductById_TestVariousInputs(
            int id,
            bool exists,
            string? expectedName,
            Type? expectedException)
        {
            // Arrange
            var query = new GetProductByIdQuery(id);
            var handler = new GetProductByIdQueryHandler(_productRepositoryMock.Object);

            // Setup the mock product entity
            ProductsEntity? mockProduct = exists
                ? new ProductsEntity { Id = id, Name = expectedName! }
                : null;

            _productRepositoryMock
                .Setup(repo => repo.GetProductById(id))
                .ReturnsAsync(mockProduct);

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

                if (exists)
                {
                    // Assert product details match
                    Assert.IsNotNull(result);
                    Assert.AreEqual(id, result.Id);
                    Assert.AreEqual(expectedName, result.Name);
                }
                else
                {
                    // Assert null is returned when not found
                    Assert.IsNull(result);
                }

                // Verify the repository was called with the specific ID from the query
                _productRepositoryMock.Verify(repo => repo.GetProductById(id), Times.Once);
            }
        }
    }
}
