using Domain.Enitities;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries
{
    public record GetAllCartQuery() : IRequest<IEnumerable<CartEntity>>;
    public class GetAllCartQueryHandler(ICartRepository cartRepository)
        : IRequestHandler<GetAllCartQuery, IEnumerable<CartEntity>>
    {
        public async Task<IEnumerable<CartEntity>> Handle(GetAllCartQuery request, CancellationToken cancellationToken)
        {
            return await cartRepository.GetCarts();
        }
    }
}
