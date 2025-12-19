using Application.DTO;
using Application.Mapping;
using Domain.Enitities;
using Domain.Enum;
using Domain.Interfaces;
using MediatR;

namespace Application.Command
{
    public record AddCartCommand(CartDTO Cart) : IRequest<CartWithItemsDto>;
    public class AddCartCommandHandler(ICartRepository cartRepository,
        IClientRepository clientRepository)
        : IRequestHandler<AddCartCommand, CartWithItemsDto>
    {
        public async Task<CartWithItemsDto> Handle(AddCartCommand request, CancellationToken cancellationToken)
        {
            CartEntity cart = CartEntity.CreateCart(request.Cart.ClientId);
            var client = await clientRepository.GetClientById(cart.ClientId);
            if (client == null)
            {
                throw new InvalidDataException("Client Id is not found!");
            }
            else if (client.ClientType.ToLower() == nameof(ClientTypeEnum.Professional).ToLower() && client.AnnualRevenue <= 0)
            {
                throw new InvalidDataException("Unable to add in the Cart, Please update annual Revenue!");
            }
            var result = await cartRepository.AddCarts(cart);
            return result.ToDto(); ;
        }
    }
}
