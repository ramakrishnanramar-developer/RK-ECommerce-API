using Domain.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Domain;


namespace API.Tests.Domain
{
    [TestClass]
    public class DependencyInjectionTests
    {
        [TestMethod]
        public void AddDomainDI_RegistersConnectionStringOptions()
        {
            // Arrange: in-memory configuration
            var inMemorySettings = new Dictionary<string, string>
            {
                [$"{ConnectionStringOptions.SectionName}:DefaultConnection"] = "Server=.;Database=TestDb;Trusted_Connection=True;"
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Use official ServiceCollection from NuGet package
            var services = new ServiceCollection();

            // Act: call your extension method
            services.AddDomainDI(configuration);

            // Build the ServiceProvider
            var serviceProvider = services.BuildServiceProvider();

            // Assert: IOptions<ConnectionStringOptions> is registered correctly
            var options = serviceProvider.GetService<IOptions<ConnectionStringOptions>>();
            Assert.IsNotNull(options, "IOptions<ConnectionStringOptions> should be registered.");
            Assert.AreEqual("Server=.;Database=TestDb;Trusted_Connection=True;", options.Value.DefaultConnection);
        }
    }
}
