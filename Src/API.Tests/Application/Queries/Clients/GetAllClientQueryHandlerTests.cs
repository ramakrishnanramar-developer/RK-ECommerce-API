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
    public class GetAllClientQueryHandlerTests
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock = new();

        [DataTestMethod]
        // DataRow: NumberOfClients, ExpectedCount, ExpectedException
        [DataRow(2, 2, null)]  // Case: Multiple clients found
        [DataRow(0, 0, null)]  // Case: Empty database
        [DataRow(1, 1, null)]  // Case: Single client found
        public async Task GetAllClients_TestVariousInputs(
            int itemCount,
            int expectedCount,
            Type? expectedException)
        {
            // Arrange
            var query = new GetAllClientQuery();
            var handler = new GetAllClientQueryHandler(_clientRepositoryMock.Object);

            // Generate mock data: Mix of Individual and Professional
            var mockClients = new List<ClientEntity>();
            for (int i = 1; i <= itemCount; i++)
            {
                if (i % 2 == 0)
                    mockClients.Add(ClientEntity.CreateIndividual(i, "First" + i, "Last" + i));
                else
                    mockClients.Add(ClientEntity.CreateProfessional(i, "Corp" + i, "BR" + i, 1000m, "VAT" + i));
            }

            _clientRepositoryMock
                .Setup(repo => repo.GetClients())
                .ReturnsAsync(mockClients);

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
                    // Check type dynamically as per requested format
                    Assert.IsInstanceOfType(ex, expectedException);
                }
            }
            else
            {
                // Execute the handler logic
                var result = await handler.Handle(query, CancellationToken.None);

                // Assertions
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedCount, result.Count());

                // Verify the repository interface was called exactly once
                _clientRepositoryMock.Verify(repo => repo.GetClients(), Times.Once);
            }
        }
    }
}
