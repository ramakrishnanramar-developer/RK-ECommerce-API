using Application;
using Domain;
using Infrastructure;

namespace API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAPPDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationDI()
                .AddInfrastructureDI()
                .AddDomainDI(configuration);
            return services;
        }
    }
}
