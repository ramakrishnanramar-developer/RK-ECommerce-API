using Application.DTO;
using Application.Mapping;
using Domain.Enitities;
using Domain.Enum;
using Domain.Interfaces;
using MediatR;

namespace Application.Command
{
    public record AddCartItemsCommand(CartWithItemsDto Cart) : IRequest<CartWithItemsDto>;
    public class AddCartItemsCommandHandler(ICartRepository cartRepository,
        ICartItemsRepository cartItemsRepository, IProductRepository productRepository)
        : IRequestHandler<AddCartItemsCommand, CartWithItemsDto>
    {
        public async Task<CartWithItemsDto> Handle(AddCartItemsCommand request, CancellationToken cancellationToken)
        {


            //Check valid Cart Id
            var cart = await cartRepository.GetCartById(request.Cart.Id);
            if (cart == null)
            {
                throw new InvalidDataException("Cart Id is not found!");
            }

            //Check Valid Product Id
            List<int> pIDs = request.Cart.Items.Select(x => x.ProductId).Distinct().ToList();
            var products = await productRepository.GetProductByIds(pIDs);
            if (!(products != null && pIDs.Count() == products.Count()))
            {
                var foundProductIds = products.Select(p => p.Id).ToHashSet();
                var invalidProductIds = pIDs.Where(id => !foundProductIds.Contains(id)).ToList();
                throw new InvalidDataException(
                    $"Invalid product ids: {string.Join(", ", invalidProductIds)}"
                );
            }
            else
            {
                //Check Product price is updated or Not
                List<string> productsName = new List<string>();
                foreach (var product in products)
                {
                    if (product.IndividualPrice == 0 || product.ProfessionalLowRevenuePrice == 0 || product.ProfessionalHighRevenuePrice == 0)
                    {
                        productsName.Add(product.Name);
                    }
                }
                if (productsName.Any())
                {
                    throw new InvalidDataException(
                                        $"Update the price in Products: {string.Join(", ", productsName)}"
                                    );
                }
            }
            //Update the Cart Items
            List<CartItemsEntity> cardItems = new List<CartItemsEntity>();
            foreach (var item in request.Cart.Items)
            {
                cardItems.Add(new CartItemsEntity(item.ProductId, item.Quantity, request.Cart.Id));
            }
            var result = await cartItemsRepository.AddCartItems(cardItems);
            return result.ToDto(); ;
        }
    }
}
