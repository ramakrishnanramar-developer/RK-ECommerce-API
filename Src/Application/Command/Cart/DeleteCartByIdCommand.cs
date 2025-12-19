using Domain.Enitities;
using Domain.Interfaces;
using MediatR;

namespace Application.Command
{
    public record DeleteCartByIdCommand(int Id) : IRequest<bool>;
    public class DeleteCartByIdCommandHandler(ICartRepository cartRepository)
        : IRequestHandler<DeleteCartByIdCommand, bool>
    {
        public async Task<bool> Handle(DeleteCartByIdCommand request, CancellationToken cancellationToken)
        {
            var client = await cartRepository.GetCartById(request.Id);
            if (client == null) return false;

            return await cartRepository.DeleteCarts(request.Id);
        }
    }
}
