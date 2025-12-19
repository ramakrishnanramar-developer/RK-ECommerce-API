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
    public class GetClientByIdQueryHandlerTests
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock = new();

        [DataTestMethod]
        // DataRow: ClientId, ClientType, DoesExist, ExpectedException
        [DataRow(1, "Individual", true, null)]
        [DataRow(2, "Professional", true, null)]
        [DataRow(99, "Individual", false, null)] // Case: Returns null/not found
        public async Task GetClientById_TestVariousInputs(
            int clientId,
            string clientType,
            bool exists,
            Type? expectedException)
        {
            // Arrange
            var query = new GetClientByIdQuery(clientId);
            var handler = new GetClientByIdQueryHandler(_clientRepositoryMock.Object);

            // Setup mock response based on factory methods
            ClientEntity? mockClient = null;
            if (exists)
            {
                mockClient = clientType == "Individual"
                    ? ClientEntity.CreateIndividual(clientId, "John", "Doe")
                    : ClientEntity.CreateProfessional(clientId, "CitiCorp", "BR123", 1000m, "VAT123");
            }

            _clientRepositoryMock
                .Setup(repo => repo.GetClientById(clientId))
                .ReturnsAsync(mockClient);

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
                // Execute
                var result = await handler.Handle(query, CancellationToken.None);

                // Assert
                if (exists)
                {
                    Assert.IsNotNull(result);
                    Assert.AreEqual(clientId, result.Id);
                    Assert.AreEqual(clientType, result.ClientType);
                }
                else
                {
                    Assert.IsNull(result);
                }

                // Verify repo was called with correct ID
                _clientRepositoryMock.Verify(repo => repo.GetClientById(clientId), Times.Once);
            }
        }
    }
}
