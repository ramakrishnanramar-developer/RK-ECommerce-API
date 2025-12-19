using Domain.Enitities;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries
{
    public record GetClientByIdQuery(int id) : IRequest<ClientEntity>;
    public class GetClientByIdQueryHandler(IClientRepository clientRepository)
        : IRequestHandler<GetClientByIdQuery, ClientEntity>
    {
        public async Task<ClientEntity?> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            return await clientRepository.GetClientById(request.id);
        }


    }
}
