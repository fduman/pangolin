using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pangolin.Extensions;

public static class PangolinExtensions
{
    public static IHostBuilder UsePangolin(this IHostBuilder hostBuilder)
    {
        hostBuilder.UsePangolin(siloBuilder =>
        {
            siloBuilder
                .AddReminders()
                .UseLocalhostClustering()
                .UseInMemoryReminderService()
                .AddMemoryGrainStorage("pangolin");
        });

        return hostBuilder;
    }

    public static IHostBuilder UsePangolin(this IHostBuilder hostBuilder, Action<ISiloBuilder> siloBuilder)
    {
        hostBuilder.UseOrleans(siloBuilder)
            .ConfigureServices(p =>
            {
                p.AddScoped<ITriggerFactory, TriggerFactory>();
            });

        return hostBuilder;
    }
}