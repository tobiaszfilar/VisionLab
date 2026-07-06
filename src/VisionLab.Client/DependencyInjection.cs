using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace VisionLab.Client;

public static class DependencyInjection
{
    public static IServiceCollection AddVisionLabClient(
        this IServiceCollection services,
        Action<VisionLabClientOptions>? configureOptions = null)
    {
        if (configureOptions is not null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.AddOptions<VisionLabClientOptions>();
        }

        services.AddHttpClient<IVisionLabApiClient, VisionLabApiClient>(
            (serviceProvider, httpClient) =>
            {
                var options = serviceProvider
                    .GetRequiredService<IOptions<VisionLabClientOptions>>()
                    .Value;

                httpClient.BaseAddress = options.BaseAddress;
            });

        return services;
    }
}