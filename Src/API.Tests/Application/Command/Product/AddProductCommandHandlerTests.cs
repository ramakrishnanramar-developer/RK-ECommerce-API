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
    public class AddProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock = new();

        [DataTestMethod]
        // DataRow: Name, Type, IndivPrice, HighRevPrice, LowRevPrice, ExpectedException
        [DataRow("Trade Platform", nameof(ProductTypeEnum.HighEndPhone), 100, 200, 150, null)]
        [DataRow("", "Service", 50, 80, 70, typeof(InvalidOperationException))] // Example: Validation logic might throw
        [DataRow("STP Automator", "On-Prem", -10, 20, 15, typeof(InvalidOperationException))] // Invalid price check
        public async Task AddProduct_TestVariousInputs(
            string name,
            string type,
            double individualPrice,
            double highRevPrice,
            double lowRevPrice,
            Type? expectedException)
        {
            // Arrange
            var inputEntity = new ProductsEntity
            {
                Name = name,
                Type = type,
                IndividualPrice = (decimal)individualPrice,
                ProfessionalHighRevenuePrice = (decimal)highRevPrice,
                ProfessionalLowRevenuePrice = (decimal)lowRevPrice
            };

            var command = new AddProductCommand(inputEntity);
            var handler = new AddProductCommandHandler(_productRepositoryMock.Object);

            // Setup mock to return whatever is passed to it
            _productRepositoryMock
                .Setup(repo => repo.AddProducts(It.IsAny<ProductsEntity>()))
                .ReturnsAsync((ProductsEntity p) => p);

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
                // Valid Case
                var result = await handler.Handle(command, CancellationToken.None);

                // Assertions
                Assert.IsNotNull(result);
                Assert.AreEqual(name, result.Name);
                Assert.AreEqual((decimal)individualPrice, result.IndividualPrice);

                // Verify repository was called exactly once
                _productRepositoryMock.Verify(repo =>
                    repo.AddProducts(It.Is<ProductsEntity>(p => p.Name == name)),
                    Times.Once);
            }
        }
    }
}
