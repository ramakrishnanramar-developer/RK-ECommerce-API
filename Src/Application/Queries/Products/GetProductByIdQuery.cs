using Domain.Enitities;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries
{
    public record GetProductByIdQuery(int id) : IRequest<ProductsEntity>;
    public class GetProductByIdQueryHandler(IProductRepository productRepository)
        : IRequestHandler<GetProductByIdQuery, ProductsEntity>
    {
        public async Task<ProductsEntity?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await productRepository.GetProductById(request.id);
        }


    }
}
