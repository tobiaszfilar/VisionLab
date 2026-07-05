using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VisionLab.Application.Images;
using VisionLab.Infrastructure.Images;

namespace VisionLab.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ImageStorageOptions>(
            configuration.GetSection("ImageStorage"));

        services.AddSingleton<IImageStorage, DiskImageStorage>();

        return services;
    }
}