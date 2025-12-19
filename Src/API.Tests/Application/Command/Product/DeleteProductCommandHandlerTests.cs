using Application.Command;
using Domain.Enitities;
using Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Tests.Application.Command
{
    [TestClass]
    public class DeleteProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock = new();

        [DataTestMethod]
        // DataRow: ProductId, DoesProductExist, DeleteOperationSuccess, ExpectedResult, ExpectedException
        [DataRow(1, true, true, true, null)]       // Success: Product found and deleted
        [DataRow(50, false, false, false, null)]   // Fail: Product not found (returns false)
        [DataRow(99, true, false, false, null)]    // Fail: Product found but DB deletion failed
        public async Task DeleteProduct_TestVariousInputs(
            int productId,
            bool doesProductExist,
            bool deleteOperationSuccess,
            bool expectedResult,
            Type? expectedException)
        {
            // Arrange
            var command = new DeleteProductByIdCommand(productId);
            var handler = new DeleteProductByIdCommandHandler(_productRepositoryMock.Object);

            // Setup 1: Mock GetProductById
            ProductsEntity? mockProduct = doesProductExist
                ? new ProductsEntity { Id = productId, Name = "Test Product" }
                : null;

            _productRepositoryMock
                .Setup(repo => repo.GetProductById(productId))
                .ReturnsAsync(mockProduct);

            // Setup 2: Mock DeleteProducts
            _productRepositoryMock
                .Setup(repo => repo.DeleteProducts(productId))
                .ReturnsAsync(deleteOperationSuccess);

            // Act & Assert
            if (expectedException != null)
            {
                try
                {
                    await handler.Handle(command, CancellationToken.None);
                    Assert.Fail($"Expected exception {expectedException.Name} was not thrown.");
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOfType(ex, expectedException);
                }
            }
            else
            {
                var result = await handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.AreEqual(expectedResult, result);

                // Verify Logic Flow
                if (doesProductExist)
                {
                    // Should attempt to delete if product was found
                    _productRepositoryMock.Verify(repo => repo.DeleteProducts(productId), Times.Once);
                }
                else
                {
                    // Should return early and NOT attempt deletion if product not found
                    _productRepositoryMock.Verify(repo => repo.DeleteProducts(productId), Times.Never);
                }
            }
        }
    }
}
