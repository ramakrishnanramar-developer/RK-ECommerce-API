using Application.Command;
using Application.DTO;
using Domain.Enitities;
using Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Tests.Application.Command
{
    [TestClass]
    public class AddCartCommandHandlerTests
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock = new();
        private readonly Mock<IClientRepository> _clientRepositoryMock = new();

        [DataTestMethod]
        // DataRow: ClientType, Revenue, ClientExists, ExpectedException
        [DataRow("Individual", 0.0, true, null)]                               // Success: Individual
        [DataRow("Professional", 50000.0, true, null)]                         // Success: Professional with Revenue
        [DataRow("Individual", 0.0, false, typeof(InvalidDataException))]      // Fail: Client Not Found
        [DataRow("Professional", 0.0, true, typeof(InvalidDataException))]     // Fail: Prof. with 0 Revenue
        public async Task AddCart_TestVariousInputs(
            string clientType,
            double revenue,
            bool clientExists,
            Type? expectedException)
        {
            // Arrange
            int clientId = 1;
            var cartDto = new CartDTO { ClientId = clientId };
            var command = new AddCartCommand(cartDto);
            var handler = new AddCartCommandHandler(_cartRepositoryMock.Object, _clientRepositoryMock.Object);

            // Setup: Mock Client Repository
            ClientEntity? mockClient = null;
            if (clientExists)
            {
                mockClient = clientType == "Individual"
                    ? ClientEntity.CreateIndividual(clientId, "John", "Doe")
                    : ClientEntity.CreateProfessional(clientId, "TradeCorp", "BR123", (decimal)revenue, "VAT123");
            }

            _clientRepositoryMock
                .Setup(repo => repo.GetClientById(clientId))
                .ReturnsAsync(mockClient);

            // Setup: Mock Cart Repository
            _cartRepositoryMock
                .Setup(repo => repo.AddCarts(It.IsAny<CartEntity>()))
                .ReturnsAsync((CartEntity c) => c);

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
                    // Verify the correct exception type
                    Assert.IsInstanceOfType(ex, expectedException);

                    // Specific Message Validation
                    if (!clientExists)
                        Assert.AreEqual("Client Id is not found!", ex.Message);
                    else if (clientType == "Professional" && revenue <= 0)
                        Assert.AreEqual("Unable to add in the Cart, Please update annual Revenue!", ex.Message);
                }
            }
            else
            {
                // Valid Path
                var result = await handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clientId, result.ClientId);

                // Verify the Cart was actually saved
                _cartRepositoryMock.Verify(repo => repo.AddCarts(It.IsAny<CartEntity>()), Times.Once);
            }
        }
    }
}
