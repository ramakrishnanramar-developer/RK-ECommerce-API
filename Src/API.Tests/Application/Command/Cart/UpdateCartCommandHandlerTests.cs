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
    public class UpdateCartCommandHandlerTests
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock = new();

        [DataTestMethod]
        // DataRow: RouteId, DtoId, ClientId, ExpectedException
        [DataRow(100, 100, 1, null)]                               // Valid case: IDs match
        [DataRow(0, 100, 1, typeof(InvalidOperationException))]    // ID is zero
        [DataRow(100, 200, 1, typeof(InvalidOperationException))]  // ID mismatch
        public async Task UpdateCart_TestVariousInputs(
            int routeId,
            int dtoId,
            int clientId,
            Type? expectedException)
        {
            // Arrange
            var cartDto = new CartWithIdDTO
            {
                Id = dtoId,
                ClientId = clientId
            };

            var command = new UpdateCartCommand(routeId, cartDto);
            var handler = new UpdateCartCommandHandler(_cartRepositoryMock.Object);

            // Setup Repository: Return a mock CartEntity that can be converted ToDto()
            // Note: Assuming ToDto() maps the ClientId correctly.
            var mockResult = CartEntity.CreateCart(clientId);

            _cartRepositoryMock
                .Setup(repo => repo.UpdateCarts(routeId, It.IsAny<CartEntity>()))
                .ReturnsAsync(mockResult);

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
                    // Verify the correct exception type was thrown
                    Assert.IsInstanceOfType(ex, expectedException);

                    // Optional: Verify specific error messages for your trade workflow logs
                    if (routeId == 0)
                        Assert.AreEqual("Invalid Product Id!", ex.Message);
                    else if (routeId != dtoId)
                        Assert.AreEqual("Cart id Mismatched", ex.Message);
                }
            }
            else
            {
                // Valid execution path
                var result = await handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(clientId, result.ClientId);

                // Verify the repository was called exactly once with a CartEntity
                _cartRepositoryMock.Verify(repo =>
                    repo.UpdateCarts(routeId, It.Is<CartEntity>(c => c.ClientId == clientId)),
                    Times.Once);
            }
        }
    }
}
