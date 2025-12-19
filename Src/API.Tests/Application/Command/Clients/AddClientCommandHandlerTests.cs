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
    public class AddClientCommandHandlerTests
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock = new();

        [DataTestMethod]
        // DataRow: ClientType, FirstName, LastName, CompanyName, Revenue, ExpectedException
        [DataRow("Individual", "John", "Doe", null, 0.0, null)]
        [DataRow("Professional", null, null, "Citi Trade Corp", 1000000.0, null)]
        [DataRow("UnknownType", "Jane", "Smith", null, 0.0, typeof(InvalidOperationException))]
        public async Task AddClient_TestVariousInputs(
            string clientType,
            string? firstName,
            string? lastName,
            string? companyName,
            double revenue,
            Type? expectedException)
        {
            // Arrange
            var inputClient = new ClientEntity
            {
                ClientType = clientType,
                FirstName = firstName,
                LastName = lastName,
                CompanyName = companyName,
                BusinessRegNo = "BR12345",
                AnnualRevenue = (decimal)revenue,
                VATNumber = "VAT999"
            };

            var command = new AddClientCommand(inputClient);
            var handler = new AddClientCommandHandler(_clientRepositoryMock.Object);

            // Setup repository to return the entity it receives (simulating DB ID assignment)
            _clientRepositoryMock
                .Setup(repo => repo.AddClients(It.IsAny<ClientEntity>()))
                .ReturnsAsync((ClientEntity c) => c);

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
                    // Verify the correct exception type was thrown due to invalid ClientType
                    Assert.IsInstanceOfType(ex, expectedException);
                    Assert.AreEqual("Invalid ClientType", ex.Message);
                }
            }
            else
            {
                // Valid case execution
                var result = await handler.Handle(command, CancellationToken.None);

                // Assertions
                Assert.IsNotNull(result);
                Assert.AreEqual(clientType, result.ClientType);

                if (clientType == "Individual")
                {
                    Assert.AreEqual(firstName, result.FirstName);
                    Assert.AreEqual(lastName, result.LastName);
                }
                else if (clientType == "Professional")
                {
                    Assert.AreEqual(companyName, result.CompanyName);
                    Assert.AreEqual((decimal)revenue, result.AnnualRevenue);
                }

                // Verify the repository was called once with the mapped domain entity
                _clientRepositoryMock.Verify(repo => repo.AddClients(It.IsAny<ClientEntity>()), Times.Once);
            }
        }
    }
}
