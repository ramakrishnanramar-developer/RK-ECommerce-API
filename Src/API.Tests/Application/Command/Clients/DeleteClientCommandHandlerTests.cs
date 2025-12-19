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
    public class DeleteClientCommandHandlerTests
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock = new();

        [DataTestMethod]
        // DataRow: ClientId, DoesClientExistInDb, DeletionResultFromRepo, ExpectedFinalResult, ExpectedException
        [DataRow(1, true, true, true, null)]        // Case: Client exists and deleted successfully
        [DataRow(50, false, false, false, null)]    // Case: Client does not exist (returns false early)
        [DataRow(99, true, false, false, null)]     // Case: Client exists but DB deletion failed
        public async Task DeleteClient_TestVariousInputs(
            int clientId,
            bool exists,
            bool repoDeleteResponse,
            bool expectedResult,
            Type? expectedException)
        {
            // Arrange
            var command = new DeleteClientByIdCommand(clientId);
            var handler = new DeleteClientByIdCommandHandler(_clientRepositoryMock.Object);

            // Setup: Mock the "Get" check
            ClientEntity? mockClient = exists
                ? ClientEntity.CreateIndividual(clientId, "Test", "User")
                : null;

            _clientRepositoryMock
                .Setup(repo => repo.GetClientById(clientId))
                .ReturnsAsync(mockClient);

            // Setup: Mock the "Delete" action
            _clientRepositoryMock
                .Setup(repo => repo.DeleteClients(clientId))
                .ReturnsAsync(repoDeleteResponse);

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
                // Execute the handler
                var result = await handler.Handle(command, CancellationToken.None);

                // Verify the boolean result matches expectation
                Assert.AreEqual(expectedResult, result);

                // Logic Verification
                if (exists)
                {
                    // Ensure Delete was actually called if the client was found
                    _clientRepositoryMock.Verify(repo => repo.DeleteClients(clientId), Times.Once);
                }
                else
                {
                    // Ensure Delete was NEVER called if GetClientById returned null
                    _clientRepositoryMock.Verify(repo => repo.DeleteClients(clientId), Times.Never);
                }
            }
        }
    }
}
