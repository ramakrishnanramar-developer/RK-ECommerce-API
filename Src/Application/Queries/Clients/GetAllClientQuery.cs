using Domain.Enitities;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries
{
    public record GetAllClientQuery() : IRequest<IEnumerable<ClientEntity>>;
    public class GetAllClientQueryHandler(IClientRepository clientRepository)
        : IRequestHandler<GetAllClientQuery, IEnumerable<ClientEntity>>
    {
        public async Task<IEnumerable<ClientEntity>> Handle(GetAllClientQuery request, CancellationToken cancellationToken)
        {
            return await clientRepository.GetClients();
        }


    }
}
