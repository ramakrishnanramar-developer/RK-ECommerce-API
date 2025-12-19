using Domain.Enitities;
using Domain.Interfaces;
using MediatR;

namespace Application.Command
{
    public record DeleteProductByIdCommand(int Id) : IRequest<bool>;
    public class DeleteProductByIdCommandHandler(IProductRepository productRepository)
        : IRequestHandler<DeleteProductByIdCommand, bool>
    {
        public async Task<bool> Handle(DeleteProductByIdCommand request, CancellationToken cancellationToken)
        {
            var client = await productRepository.GetProductById(request.Id);
            if (client == null) return false;

            return await productRepository.DeleteProducts(request.Id);
        }
    }
}
