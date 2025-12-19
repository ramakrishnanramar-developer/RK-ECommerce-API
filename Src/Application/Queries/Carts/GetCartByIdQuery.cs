using Application.DTO;
using Application.Mapping;
using Domain.Enitities;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries
{
    public record GetCartByIdQuery(int id) : IRequest<CartWithItemsDto>;
    public class GetCartByIdQueryHandler(ICartRepository cartRepository)
        : IRequestHandler<GetCartByIdQuery, CartWithItemsDto>
    {
        public async Task<CartWithItemsDto?> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await cartRepository.GetCartById(request.id);
            return result.ToDto();
        }


    }
}
