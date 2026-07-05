using Microsoft.Extensions.DependencyInjection;

namespace VisionLab.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}