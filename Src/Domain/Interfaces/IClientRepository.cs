using Domain.Enitities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<ClientEntity>> GetClients();
        Task<ClientEntity?> GetClientById(int Id);
        Task<ClientEntity> AddClients(ClientEntity entity);
        Task<ClientEntity> UpdateClients(int clientId, ClientEntity entity);
        Task<bool> DeleteClients(int clientId);
    }
}
