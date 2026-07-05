using Microsoft.Extensions.DependencyInjection;
using VisionLab.Application.Images;

namespace VisionLab.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IImageAssetService, ImageAssetService>();
        return services;
    }
}