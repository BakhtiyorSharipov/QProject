using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using QBranchService.Application.Consumers;

namespace QBranchService.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddScoped<ValidateBranchIdsConsumer>();
        services.AddScoped<ValidateCompanyConsumer>();
        services.AddScoped<ValidateCompanyServiceConsumer>();

        return services;
    }
}