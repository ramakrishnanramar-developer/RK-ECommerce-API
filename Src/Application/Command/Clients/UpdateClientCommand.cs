using Domain.Enitities;
using Domain.Enum;
using Domain.Interfaces;
using MediatR;

namespace Application.Command
{
    public record UpdateClientCommand(int Id, ClientEntity Client) : IRequest<ClientEntity>;
    public class UpdateClientCommandHandler(IClientRepository clientRepository)
        : IRequestHandler<UpdateClientCommand, ClientEntity>
    {

        public async Task<ClientEntity> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                throw new InvalidOperationException("Invalid Client Id!");
            }
            if (request.Id != request.Client.Id)
            {
                throw new InvalidOperationException("Client id Mismatched");
            }
            var client = await clientRepository.GetClientById(request.Id)
                     ?? throw new InvalidOperationException("Client not found");

            // Call domain update methods
            if (client.ClientType.ToLower() == nameof(ClientTypeEnum.Individual).ToLower())
            {
                client.UpdateIndividual(
                    request.Client.FirstName!,
                    request.Client.LastName!
                );
            }
            else if (client.ClientType.ToLower() == nameof(ClientTypeEnum.Professional).ToLower())
            {
                client.UpdateProfessional(
                    request.Client.CompanyName!,
                    request.Client.BusinessRegNo!,
                    request.Client.AnnualRevenue!.Value,
                    request.Client.VATNumber
                );
            }

            return await clientRepository.UpdateClients(request.Id, request.Client);
        }
    }
}
