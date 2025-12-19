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
    public class UpdateClientCommandHandlerTests
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock = new();

        [DataTestMethod]
        // DataRow: RouteId, EntityId, ClientType, ExistsInDb, ExpectedException
        [DataRow(10, 10, "Individual", true, null)]                            // Success: Individual
        [DataRow(20, 20, "Professional", true, null)]                          // Success: Professional
        [DataRow(0, 10, "Individual", true, typeof(InvalidOperationException))] // ID is Zero
        [DataRow(10, 20, "Individual", true, typeof(InvalidOperationException))] // ID Mismatch
        [DataRow(30, 30, "Individual", false, typeof(InvalidOperationException))]// Not Found
        public async Task UpdateClient_TestVariousInputs(
            int routeId,
            int entityId,
            string clientType,
            bool exists,
            Type? expectedException)
        {
            // Arrange
            var inputClient = new ClientEntity
            {
                Id = entityId,
                ClientType = clientType,
                FirstName = "UpdatedName",
                LastName = "UpdatedLast",
                CompanyName = "UpdatedCorp",
                BusinessRegNo = "BR999",
                AnnualRevenue = 500000m
            };

            var command = new UpdateClientCommand(routeId, inputClient);
            var handler = new UpdateClientCommandHandler(_clientRepositoryMock.Object);

            // Setup: Mock GetClientById to return a client or null
            ClientEntity? existingClient = exists
                ? (clientType == "Individual"
                    ? ClientEntity.CreateIndividual(routeId, "Old", "Name")
                    : ClientEntity.CreateProfessional(routeId, "OldCorp", "BR1", 100m, "VAT1"))
                : null;

            _clientRepositoryMock
                .Setup(repo => repo.GetClientById(routeId))
                .ReturnsAsync(existingClient);

            // Setup: Mock Update action
            _clientRepositoryMock
                .Setup(repo => repo.UpdateClients(routeId, It.IsAny<ClientEntity>()))
                .ReturnsAsync(inputClient);

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

                    // Specific message check (Optional, but useful for TDD)
                    if (routeId == 0) Assert.AreEqual("Invalid Client Id!", ex.Message);
                    else if (routeId != entityId) Assert.AreEqual("Client id Mismatched", ex.Message);
                    else if (!exists) Assert.AreEqual("Client not found", ex.Message);
                }
            }
            else
            {
                // Execute the valid path
                var result = await handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(routeId, result.Id);

                // Verify that UpdateClients was actually called
                _clientRepositoryMock.Verify(repo =>
                    repo.UpdateClients(routeId, It.Is<ClientEntity>(c => c.Id == routeId)),
                    Times.Once);
            }
        }
    }
}
