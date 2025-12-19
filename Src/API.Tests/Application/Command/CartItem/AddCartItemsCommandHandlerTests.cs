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
    public class AddCartItemsCommandHandlerTests
    {
        private readonly Mock<ICartRepository> _cartRepoMock = new();
        private readonly Mock<ICartItemsRepository> _cartItemsRepoMock = new();
        private readonly Mock<IProductRepository> _productRepoMock = new();

        [DataTestMethod]
        // DataRow: CartExists, AllProductsValid, PricesConfigured, ExpectedException
        [DataRow(true, true, true, null)]                                     // Success path
        [DataRow(false, true, true, typeof(InvalidDataException))]           // Cart not found
        [DataRow(true, false, true, typeof(InvalidDataException))]           // Invalid Product IDs
        [DataRow(true, true, false, typeof(InvalidDataException))]          // Missing price config
        public async Task AddCartItems_TestVariousInputs(
            bool cartExists,
            bool allProductsValid,
            bool pricesConfigured,
            Type? expectedException)
        {
            // Arrange
            int cartId = 10;
            var cartDto = new CartWithItemsDto
            {
                Id = cartId,
                Items = new List<CartItemDto> { new() { ProductId = 1, Quantity = 2 } }
            };
            var command = new AddCartItemsCommand(cartDto);
            var handler = new AddCartItemsCommandHandler(_cartRepoMock.Object, _cartItemsRepoMock.Object, _productRepoMock.Object);

            // 1. Setup Cart Mock
            _cartRepoMock.Setup(r => r.GetCartById(cartId))
                .ReturnsAsync(cartExists ? CartEntity.CreateCart(1) : null);

            // 2. Setup Product Mock
            var pIds = cartDto.Items.Select(x => x.ProductId).ToList();
            var mockProducts = allProductsValid
                ? new List<ProductsEntity>
                  {
                  new() {
                      Id = 1,
                      Name = "Test Product",
                      IndividualPrice = pricesConfigured ? 10m : 0m,
                      ProfessionalHighRevenuePrice = pricesConfigured ? 20m : 0m,
                      ProfessionalLowRevenuePrice = pricesConfigured ? 15m : 0m
                  }
                  }
                : new List<ProductsEntity>(); // Empty list simulates "not all found"

            _productRepoMock.Setup(r => r.GetProductByIds(It.IsAny<List<int>>()))
                .ReturnsAsync(mockProducts);

            // 3. Setup CartItems Mock (Return same object to simulate ToDto mapping)
            var cartEntity = CartEntity.CreateCart(1);
            _cartItemsRepoMock.Setup(r => r.AddCartItems(It.IsAny<List<CartItemsEntity>>()))
                .ReturnsAsync(cartEntity);

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

                    // Verify specific error messages for audit reliability
                    if (!cartExists) Assert.IsTrue(ex.Message.Contains("Cart Id is not found"));
                    else if (!allProductsValid) Assert.IsTrue(ex.Message.Contains("Invalid product ids"));
                    else if (!pricesConfigured) Assert.IsTrue(ex.Message.Contains("Update the price"));
                }
            }
            else
            {
                var result = await handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.IsNotNull(result);
                _cartItemsRepoMock.Verify(r => r.AddCartItems(It.IsAny<List<CartItemsEntity>>()), Times.Once);
            }
        }
    }
}
