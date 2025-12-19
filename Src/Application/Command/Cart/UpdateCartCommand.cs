using Application.DTO;
using Application.Mapping;
using Domain.Enitities;
using Domain.Enum;
using Domain.Interfaces;
using MediatR;

namespace Application.Command
{
    public record UpdateCartCommand(int Id, CartWithIdDTO Cart) : IRequest<CartWithItemsDto>;
    public class UpdateCartCommandHandler(ICartRepository cartRepository)
        : IRequestHandler<UpdateCartCommand, CartWithItemsDto>
    {

        public async Task<CartWithItemsDto> Handle(UpdateCartCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                throw new InvalidOperationException("Invalid Product Id!");
            }
            if (request.Id != request.Cart.Id)
            {
                throw new InvalidOperationException("Cart id Mismatched");
            }
            CartEntity cart = CartEntity.CreateCart(request.Cart.ClientId);
            var result = await cartRepository.UpdateCarts(request.Id, cart);
            return result.ToDto();
        }


    }
}
