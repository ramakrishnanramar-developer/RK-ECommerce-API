using Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Enitities
{
    public class ClientEntity
    {
        public int Id { get; set; }
        public string ClientType { get; set; } // Individual / Professional

        // Individual
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Professional
        public string? CompanyName { get; set; }
        public string? VATNumber { get; set; }
        public string? BusinessRegNo { get; set; }
        public decimal? AnnualRevenue { get; set; }

        public bool IsLargeProfessional()
        {
            return ClientType == "Professional"
                   && AnnualRevenue.HasValue
                   && AnnualRevenue.Value > 10_000_000;
        }
        // Business Logic - Constructor / Factory
        public static ClientEntity CreateIndividual(int id, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new InvalidOperationException("FirstName is required");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new InvalidOperationException("LastName is required");

            return new ClientEntity
            {
                Id = id,
                ClientType = nameof(ClientTypeEnum.Individual),
                FirstName = firstName,
                LastName = lastName
            };
        }

        public static ClientEntity CreateProfessional(int id, string companyName, string businessRegNo, decimal annualRevenue, string? vatNumber = null)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                throw new InvalidOperationException("CompanyName is required");

            if (string.IsNullOrWhiteSpace(businessRegNo))
                throw new InvalidOperationException("BusinessRegNo is required");

            if (annualRevenue < 0)
                throw new InvalidOperationException("AnnualRevenue must be >= 0");

            return new ClientEntity
            {
                Id = id,
                ClientType = nameof(ClientTypeEnum.Professional),
                CompanyName = companyName,
                BusinessRegNo = businessRegNo,
                AnnualRevenue = annualRevenue,
                VATNumber = vatNumber
            };
        }

        // Update Methods
        public void UpdateIndividual(string firstName, string lastName)
        {
            if (ClientType != "Individual")
                throw new InvalidOperationException("Cannot update as Individual: Wrong client type");

            if (string.IsNullOrWhiteSpace(firstName))
                throw new InvalidOperationException("FirstName is required");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new InvalidOperationException("LastName is required");

            FirstName = firstName;
            LastName = lastName;
        }

        public void UpdateProfessional(string companyName, string businessRegNo, decimal annualRevenue, string? vatNumber = null)
        {
            if (ClientType != "Professional")
                throw new InvalidOperationException("Cannot update as Professional: Wrong client type");

            if (string.IsNullOrWhiteSpace(companyName))
                throw new InvalidOperationException("CompanyName is required");

            if (string.IsNullOrWhiteSpace(businessRegNo))
                throw new InvalidOperationException("BusinessRegNo is required");

            if (annualRevenue < 0)
                throw new InvalidOperationException("AnnualRevenue must be >= 0");

            CompanyName = companyName;
            BusinessRegNo = businessRegNo;
            AnnualRevenue = annualRevenue;
            VATNumber = vatNumber;
        }
        [InverseProperty("Client")]
        public virtual ICollection<CartEntity> Carts { get; set; } = new List<CartEntity>();
    }
}
