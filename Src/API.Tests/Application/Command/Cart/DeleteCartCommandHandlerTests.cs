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
    public class DeleteCartCommandHandlerTests
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock = new();

        [DataTestMethod]
        // DataRow: CartId, DoesCartExist, RepoDeleteResponse, ExpectedResult, ExpectedException
        [DataRow(1, true, true, true, null)]        // Case: Cart found and deleted
        [DataRow(50, false, false, false, null)]    // Case: Cart not found (returns false)
        [DataRow(99, true, false, false, null)]     // Case: Cart found but DB delete failed
        public async Task DeleteCart_TestVariousInputs(
            int cartId,
            bool exists,
            bool repoDeleteResponse,
            bool expectedResult,
            Type? expectedException)
        {
            // Arrange
            var command = new DeleteCartByIdCommand(cartId);
            var handler = new DeleteCartByIdCommandHandler(_cartRepositoryMock.Object);

            // Setup: Mock GetCartById
            CartEntity? mockCart = exists
                ? CartEntity.CreateCart(101) // Dummy ClientId 101
                : null;

            _cartRepositoryMock
                .Setup(repo => repo.GetCartById(cartId))
                .ReturnsAsync(mockCart);

            // Setup: Mock Delete action
            _cartRepositoryMock
                .Setup(repo => repo.DeleteCarts(cartId))
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
                    // Verify exception type dynamically
                    Assert.IsInstanceOfType(ex, expectedException);
                }
            }
            else
            {
                // Execute handler
                var result = await handler.Handle(command, CancellationToken.None);

                // Assert final boolean result
                Assert.AreEqual(expectedResult, result);

                // Logic Flow Verification
                if (exists)
                {
                    // Ensure repository Delete was called if the cart was found
                    _cartRepositoryMock.Verify(repo => repo.DeleteCarts(cartId), Times.Once);
                }
                else
                {
                    // Ensure repository Delete was NEVER called if the cart was missing
                    _cartRepositoryMock.Verify(repo => repo.DeleteCarts(cartId), Times.Never);
                }
            }
        }
    }
}
