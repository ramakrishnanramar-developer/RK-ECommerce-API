using Domain.Enitities;

namespace API.Tests.Domain
{
    [TestClass]
    public class ProductsEntityTests
    {
        #region Product Creation Tests

        // --------- Single theory-style test covering multiple scenarios ----------
        [TestMethod]
        [DataRow("iPhone", "HighEndPhone", 1500.0, 1000.0, 1150.0, null)]
        [DataRow("Galaxy", "MidRangePhone", 1200.0, 900.0, 1000.0, null)]
        [DataRow("Laptop X", "Laptop", 2000.0, 1500.0, 1800.0, null)]
        [DataRow("", "Laptop", 1200.0, 900.0, 1000.0, typeof(InvalidOperationException))]
        [DataRow(null, "Laptop", 1200.0, 900.0, 1000.0, typeof(InvalidOperationException))]
        [DataRow("Phone", "InvalidType", 100.0, 90.0, 95.0, typeof(InvalidOperationException))]
        [DataRow("Phone", "HighEndPhone", -100.0, 90.0, 95.0, typeof(InvalidOperationException))]
        [DataRow("Phone", "HighEndPhone", 100.0, 0.0, 95.0, typeof(InvalidOperationException))]
        [DataRow("Phone", "HighEndPhone", 100.0, 90.0, -1.0, typeof(InvalidOperationException))]
        public void CreateProducts_TestVariousInputs(
        string name,
        string type,
        double _individualPrice,
        double _professionalHighRevenuePrice,
        double _professionalLowRevenuePrice,
        Type? expectedException)
        {
            decimal individualPrice = (decimal)_individualPrice;
            decimal professionalHighRevenuePrice = (decimal)_professionalHighRevenuePrice;
            decimal professionalLowRevenuePrice = (decimal)_professionalLowRevenuePrice;
            if (expectedException != null)
            {
                try
                {
                    ProductsEntity.CreateProducts(
                           name, type, individualPrice, professionalHighRevenuePrice, professionalLowRevenuePrice);
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
                var product = ProductsEntity.CreateProducts(
                    name,
                    type,
                    individualPrice,
                    professionalHighRevenuePrice,
                    professionalLowRevenuePrice);

                Assert.IsNotNull(product);
                Assert.AreEqual(name, product.Name);
                Assert.AreEqual(type, product.Type);
                Assert.AreEqual(individualPrice, product.IndividualPrice);
                Assert.AreEqual(professionalHighRevenuePrice, product.ProfessionalHighRevenuePrice);
                Assert.AreEqual(professionalLowRevenuePrice, product.ProfessionalLowRevenuePrice);
            }
        }

        #endregion

        #region Price Calculation Tests

        [TestMethod]
        [DataRow("Laptop", "Individual", 1200, 0, null)]
        [DataRow("Laptop", "Professional", 900, 20_000_000, null)]
        [DataRow("Laptop", "Professional", 1000, 50_000, null)]
        [DataRow("Laptop", "UnKnown", 1000, 50_000, typeof(InvalidOperationException))]
        public void GetPriceFor_ShouldReturnIndividualPrice_ForIndividualClient(string productType, string clientType, double rPrice, double annualRevenue, Type? expected)
        {
            decimal expectedPrice = (decimal)rPrice;
            decimal expectedAnnualRevenue = (decimal)annualRevenue;
            var product = ProductsEntity.CreateProducts(
                name: "Laptop",
                productType,
                 individualPrice: 1200,
                 professionalHighRevenuePrice: 900,
                professionalLowRevenuePrice: 1000);

            var client = new ClientEntity
            {
                ClientType = clientType,
                AnnualRevenue = expectedAnnualRevenue
            };
            if (expected == null)
            {
                decimal price = product.GetPriceFor(client);
                Assert.AreEqual(expectedPrice, price);
            }
            else
            {
                try
                {
                    decimal price = product.GetPriceFor(client);
                }
                catch (Exception ex)
                {
                    // Check type dynamically
                    Assert.IsInstanceOfType(ex, expected);
                }

            }
        }



        #endregion
    }
}
