using Domain.Enitities;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries
{
    public record GetAllProductQuery() : IRequest<IEnumerable<ProductsEntity>>;
    public class GetAllProductQueryHandler(IProductRepository productRepository)
        : IRequestHandler<GetAllProductQuery, IEnumerable<ProductsEntity>>
    {
        public async Task<IEnumerable<ProductsEntity>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            return await productRepository.GetProducts();
        }
    }
}
