using Domain.Enitities;
using Domain.Enum;
using Domain.Interfaces;
using MediatR;

namespace Application.Command
{
    public record AddProductCommand(ProductsEntity Products) : IRequest<ProductsEntity>;
    public class AddProductCommandHandler(IProductRepository productRepository)
        : IRequestHandler<AddProductCommand, ProductsEntity>
    {
        public async Task<ProductsEntity> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            ProductsEntity product = ProductsEntity.CreateProducts(request.Products.Name,
                request.Products.Type, request.Products.IndividualPrice, request.Products.ProfessionalHighRevenuePrice
                , request.Products.ProfessionalLowRevenuePrice);
            return await productRepository.AddProducts(product);
        }
    }
}
