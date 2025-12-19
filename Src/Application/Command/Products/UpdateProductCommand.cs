using Domain.Enitities;
using Domain.Enum;
using Domain.Interfaces;
using MediatR;

namespace Application.Command
{
    public record UpdateProductCommand(int Id, ProductsEntity Products) : IRequest<ProductsEntity>;
    public class UpdateProductCommandHandler(IProductRepository productRepository)
        : IRequestHandler<UpdateProductCommand, ProductsEntity>
    {

        public async Task<ProductsEntity> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                throw new InvalidOperationException("Invalid Product Id!");
            }
            if (request.Id != request.Products.Id)
            {
                throw new InvalidOperationException("Product id Mismatched");
            }
            ProductsEntity product = ProductsEntity.CreateProducts(request.Products.Name,
               request.Products.Type, request.Products.IndividualPrice, request.Products.ProfessionalHighRevenuePrice
               , request.Products.ProfessionalLowRevenuePrice);
            return await productRepository.UpdateProducts(request.Id, product);
        }


    }
}
