using Domain.Enitities;
using Domain.Enum;
using Domain.Interfaces;
using MediatR;

namespace Application.Command
{
    public record AddClientCommand(ClientEntity Client) : IRequest<ClientEntity>;
    public class AddClientCommandHandler(IClientRepository clientRepository)
        : IRequestHandler<AddClientCommand, ClientEntity>
    {
        public async Task<ClientEntity> Handle(AddClientCommand request, CancellationToken cancellationToken)
        {
            ClientEntity client;
            if (request.Client.ClientType.ToLower() == nameof(ClientTypeEnum.Individual).ToLower())
            {
                client = ClientEntity.CreateIndividual(
                   id: 0,
                   firstName: request.Client.FirstName!,
                   lastName: request.Client.LastName!
               );
            }
            else if (request.Client.ClientType.ToLower() == nameof(ClientTypeEnum.Professional).ToLower())
            {
                client = ClientEntity.CreateProfessional(
                    id: 0,
                    companyName: request.Client.CompanyName!,
                    businessRegNo: request.Client.BusinessRegNo!,
                    annualRevenue: request.Client.AnnualRevenue!.Value,
                    vatNumber: request.Client.VATNumber
                );
            }
            else
            {
                throw new InvalidOperationException("Invalid ClientType");
            }
            return await clientRepository.AddClients(client);
        }
    }
}
