using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace QBranchService.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        return services;
    }
}