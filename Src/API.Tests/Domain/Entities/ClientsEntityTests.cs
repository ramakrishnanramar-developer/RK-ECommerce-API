using Domain.Enitities;
using Domain.Enum;
using System;

namespace API.Tests.Domain
{
    [TestClass]
    public class ClientsEntityTests
    {
        #region Client Creation Tests

        // --------- Single theory-style test covering multiple scenarios ----------
        [DataTestMethod]
        // Valid products
        [DataRow(0, "", "LName", typeof(InvalidOperationException))]
        [DataRow(0, "FName", "", typeof(InvalidOperationException))]
        [DataRow(0, "FName", "LName", null)]
        public void CreateIndividual_TestVariousInputs(
        int id, string firstName, string lastName,
        Type expectedException)
        {
            if (expectedException != null)
            {
                try
                {
                    ClientEntity.CreateIndividual(id, firstName, lastName);
                }
                catch (Exception ex)
                {
                    // Check type dynamically
                    Assert.IsInstanceOfType(ex, expectedException);
                }

            }
            else
            {
                // Valid input - should return a product
                var client = ClientEntity.CreateIndividual(id, firstName, lastName);

                Assert.IsNotNull(client);
                Assert.AreEqual(firstName, client.FirstName);
                Assert.AreEqual(lastName, client.LastName);
                Assert.AreEqual(id, client.Id);
                Assert.AreEqual(nameof(ClientTypeEnum.Individual), client.ClientType);
            }
        }

        [DataTestMethod]
        [DataRow(0, "", "", 0, "", typeof(InvalidOperationException))]
        [DataRow(0, "ABC", "", 0, "", typeof(InvalidOperationException))]
        [DataRow(0, "ABC", "BR", -2, "", typeof(InvalidOperationException))]
        [DataRow(0, "ABC", "BR", 0, "", typeof(InvalidOperationException))]
        [DataRow(0, "ABC", "BR", 10, "VAT", null)]
        public void CreateProfessional_TestVariousInputs(int id, string companyName, string businessRegNo,
            double annualRevenue, string? vatNumber = null, Type expectation = null)
        {
            decimal annRev = (decimal)annualRevenue;

            if (expectation != null)
            {
                try
                {
                    var client = ClientEntity.CreateProfessional(
                     id: 0,
                     companyName: companyName,
                     businessRegNo: businessRegNo,
                     annualRevenue: annRev,
                     vatNumber: vatNumber
                 );
                }
                catch (Exception ex)
                {
                    // Check type dynamically
                    Assert.IsInstanceOfType(ex, expectation);
                }
            }
            else
            {
                var client = ClientEntity.CreateProfessional(
                     id: 0,
                     companyName: companyName,
                     businessRegNo: businessRegNo,
                     annualRevenue: annRev,
                     vatNumber: vatNumber);

                Assert.IsNotNull(client);
                Assert.AreEqual(companyName, client.CompanyName);
                Assert.AreEqual(businessRegNo, client.BusinessRegNo);
                Assert.AreEqual(annRev, client.AnnualRevenue);
                Assert.AreEqual(vatNumber, client.VATNumber);
                Assert.AreEqual(id, client.Id);
                Assert.AreEqual(nameof(ClientTypeEnum.Professional), client.ClientType);
            }
        }
        #endregion
        [TestMethod]
        // Test cases for UpdateIndividual
        [DataRow("Professional", "John", "Doe", typeof(InvalidOperationException))] // Wrong client type
        [DataRow("Individual", "", "Doe", typeof(InvalidOperationException))]       // Empty first name
        [DataRow("Individual", "John", "", typeof(InvalidOperationException))]      // Empty last name
        [DataRow("Individual", "John", "Doe", null)]                                 // Valid update
        public void UpdateIndividual_TestVariousInputs(
            string clientType, string firstName, string lastName, Type? expectedException)
        {
            // Arrange: create a client based on clientType
            ClientEntity client = clientType == "Individual"
                ? ClientEntity.CreateIndividual(1, "OldFirst", "OldLast")
                : ClientEntity.CreateProfessional(1, "Company", "BR123", 1000m, "VAT123");

            if (expectedException != null)
            {
                try
                {
                    client.UpdateIndividual(firstName, lastName);
                }
                catch (Exception ex)
                {
                    // Check type dynamically
                    Assert.IsInstanceOfType(ex, expectedException);
                }
            }
            else
            {
                // Valid input - should update client
                client.UpdateIndividual(firstName, lastName);

                Assert.AreEqual(firstName, client.FirstName);
                Assert.AreEqual(lastName, client.LastName);
            }
        }
        [TestMethod]
        // Test cases for UpdateProfessional
        [DataRow("Individual", "NewCompany", "BR123", 1000, "VAT123", typeof(InvalidOperationException))]  // Wrong client type
        [DataRow("Professional", "", "BR123", 1000, "VAT123", typeof(InvalidOperationException))]           // Empty company name
        [DataRow("Professional", "Company", "", 1000, "VAT123", typeof(InvalidOperationException))]          // Empty businessRegNo
        [DataRow("Professional", "Company", "BR123", -1, "VAT123", typeof(InvalidOperationException))]      // Negative annualRevenue
        [DataRow("Professional", "Company", "BR123", 1000, "VAT123", null)]                                  // Valid update
        public void UpdateProfessional_TestVariousInputs(
            string clientType, string companyName, string businessRegNo,
            int annlRevenue, string vatNumber, Type? expectedException)
        {
            decimal annualRevenue = (decimal)annlRevenue;
            // Arrange: create client based on clientType
            ClientEntity client = clientType == "Professional"
                ? ClientEntity.CreateProfessional(1, "OldCompany", "OLD123", 5000m, "OLDVAT")
                : ClientEntity.CreateIndividual(1, "John", "Doe");

            if (expectedException != null)
            {
                try
                {
                    client.UpdateProfessional(companyName, businessRegNo, annualRevenue, vatNumber);
                }
                catch (Exception ex)
                {
                    // Check type dynamically
                    Assert.IsInstanceOfType(ex, expectedException);
                }
            }
            else
            {
                // Valid input - should update client
                client.UpdateProfessional(companyName, businessRegNo, annualRevenue, vatNumber);

                Assert.AreEqual(companyName, client.CompanyName);
                Assert.AreEqual(businessRegNo, client.BusinessRegNo);
                Assert.AreEqual(annualRevenue, client.AnnualRevenue);
                Assert.AreEqual(vatNumber, client.VATNumber);
            }
        }

        [TestMethod]
        // Test cases for IsLargeProfessional
        [DataRow("Individual", null, false)]          // Not professional
        [DataRow("Professional", null, false)]        // Professional but no revenue
        [DataRow("Professional", 5_000_000, false)]   // Professional but revenue <= 10M
        [DataRow("Professional", 15_000_000, true)]   // Professional and revenue > 10M
        public void IsLargeProfessional_TestVariousInputs(
            string clientType, int? annulRevenue, bool expectedResult)
        {
            decimal annualRevenue = annulRevenue == null ? 0 : (decimal)annulRevenue;
            // Arrange: create client based on clientType
            ClientEntity client = clientType == "Professional"
                ? ClientEntity.CreateProfessional(1, "Company", "BR123", annualRevenue, "VAT123")
                : ClientEntity.CreateIndividual(1, "John", "Doe");

            // Act
            bool result = client.IsLargeProfessional();

            // Assert
            Assert.AreEqual(expectedResult, result);
        }


    }
}
