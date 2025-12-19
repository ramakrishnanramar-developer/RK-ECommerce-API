using Domain.Enitities;
using Domain.Interfaces;
using MediatR;

namespace Application.Command
{
    public record DeleteClientByIdCommand(int Id) : IRequest<bool>;
    public class DeleteClientByIdCommandHandler(IClientRepository clientRepository)
        : IRequestHandler<DeleteClientByIdCommand, bool>
    {
        public async Task<bool> Handle(DeleteClientByIdCommand request, CancellationToken cancellationToken)
        {
            var client = await clientRepository.GetClientById(request.Id);
            if (client == null) return false;

            return await clientRepository.DeleteClients(request.Id);
        }
    }
}
