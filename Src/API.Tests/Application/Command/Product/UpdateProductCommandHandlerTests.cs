using Application.Command;
using Domain.Enitities;
using Domain.Enum;
using Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Tests.Application.Command
{
    [TestClass]
    public class UpdateProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock = new();

        [DataTestMethod]
        // DataRow: RouteId, EntityId, Name, ExpectedException
        [DataRow(10, 10, "Updated Trade System", null)]                          // Valid case
        [DataRow(0, 10, "Invalid ID Test", typeof(InvalidOperationException))]    // ID is zero
        [DataRow(10, 20, "Mismatch Test", typeof(InvalidOperationException))]     // ID Mismatch
        public async Task UpdateProduct_TestVariousInputs(
            int routeId,
            int entityId,
            string name,
            Type? expectedException)
        {
            // Arrange
            var inputEntity = new ProductsEntity
            {
                Id = entityId,
                Name = name,
                Type = nameof(ProductTypeEnum.Laptop),
                IndividualPrice = 100m,
                ProfessionalHighRevenuePrice = 200m,
                ProfessionalLowRevenuePrice = 150m
            };

            var command = new UpdateProductCommand(routeId, inputEntity);
            var handler = new UpdateProductCommandHandler(_productRepositoryMock.Object);

            // Setup Repository: Return the product if ID matches routeId
            _productRepositoryMock
                .Setup(repo => repo.UpdateProducts(routeId, It.IsAny<ProductsEntity>()))
                .ReturnsAsync((int id, ProductsEntity p) =>
                {
                    p.Id = id; // Ensure the returned object reflects the updated ID
                    return p;
                });

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
                    // Validate if the exception thrown matches our expectation
                    Assert.IsInstanceOfType(ex, expectedException);

                    // Optional: Validate specific error messages for your trade system audits
                    if (routeId == 0) Assert.AreEqual("Invalid Product Id!", ex.Message);
                    if (routeId != entityId && routeId != 0) Assert.AreEqual("Product id Mismatched", ex.Message);
                }
            }
            else
            {
                // Valid Path
                var result = await handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(name, result.Name);
                Assert.AreEqual(routeId, result.Id);

                // Verify the repository was called exactly once with the mapped entity
                _productRepositoryMock.Verify(repo =>
                    repo.UpdateProducts(routeId, It.Is<ProductsEntity>(p => p.Name == name)),
                    Times.Once);
            }
        }
    }
}
