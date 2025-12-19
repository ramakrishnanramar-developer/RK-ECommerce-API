using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Application;

namespace API.Tests.Application
{
    [TestClass]
    public class ApplicationDependencyInjectionTests
    {
        [TestMethod]
        public void AddApplicationDI_RegistersMediatRServices_WithoutResolving()
        {
            var services = new ServiceCollection();
            services.AddApplicationDI();

            // Check that IMediator descriptor exists
            var mediatorDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IMediator));
            Assert.IsNotNull(mediatorDescriptor, "IMediator should be registered in the service collection.");
        }
    }
}
