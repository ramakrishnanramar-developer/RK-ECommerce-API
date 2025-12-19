using Domain.Enitities;
using Domain.Enum;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class ClientRepository(AppDBContext appDBContext) : IClientRepository
    {
        public async Task<ClientEntity?> GetClientById(int Id)
        {
            return await appDBContext.Clients.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<IEnumerable<ClientEntity>> GetClients()
        {
            return await appDBContext.Clients.ToListAsync();
        }
        public async Task<ClientEntity> AddClients(ClientEntity entity)
        {
            appDBContext.Clients.Add(entity);
            await appDBContext.SaveChangesAsync();
            return entity;
        }
        public async Task<ClientEntity> UpdateClients(int clientId, ClientEntity entity)
        {
            var client = await GetClientById(clientId);
            if (client is not null)
            {
                if (entity.ClientType.ToLower() == nameof(ClientTypeEnum.Individual).ToLower())
                {
                    client.ClientType = nameof(ClientTypeEnum.Individual);
                    client.FirstName = entity.FirstName;
                    client.LastName = entity.LastName;
                    client.VATNumber = null;
                    client.AnnualRevenue = 0;
                    client.BusinessRegNo = null;
                    client.CompanyName = null;
                }
                else if (entity.ClientType.ToLower() == nameof(ClientTypeEnum.Professional).ToLower())
                {
                    client.ClientType = nameof(ClientTypeEnum.Professional);
                    client.FirstName = null;
                    client.LastName = null;
                    client.VATNumber = entity.VATNumber;
                    client.AnnualRevenue = entity.AnnualRevenue;
                    client.BusinessRegNo = entity.BusinessRegNo;
                    client.CompanyName = entity.CompanyName;
                }
                else
                {
                    throw new InvalidDataException("Invalid Client Type");
                }
                await appDBContext.SaveChangesAsync();
                return client;
            }
            return entity;
        }
        public async Task<bool> DeleteClients(int clientId)
        {
            var client = await GetClientById(clientId);
            if (client is not null)
            {
                appDBContext.Clients.Remove(client);
                return await appDBContext.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}
