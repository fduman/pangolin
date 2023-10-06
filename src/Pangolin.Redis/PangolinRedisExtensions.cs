using Microsoft.Extensions.Hosting;
using Pangolin.Extensions;
using StackExchange.Redis;

namespace Pangolin.Redis;

public static class PangolinRedisExtensions
{
    public static IHostBuilder UsePangolinRedis(this IHostBuilder hostBuilder, string connectionString)
    {
        hostBuilder.UsePangolin(siloBuilder =>
        {
            siloBuilder
                .AddReminders()
                .AddLogStorageBasedLogConsistencyProvider()
                .UseRedisClustering(options =>
                {
                    options.ConfigurationOptions = ConfigurationOptions.Parse(connectionString);
                })
                .UseRedisReminderService(options =>
                {
                    options.ConfigurationOptions = ConfigurationOptions.Parse(connectionString);
                })
                .AddRedisGrainStorage("pangolin", options =>
                {
                    options.ConfigurationOptions = ConfigurationOptions.Parse(connectionString);
                });
        });

        return hostBuilder;
    }

    public static IHostBuilder UsePangolinRedis(this IHostBuilder hostBuilder, Action<ConfigurationOptions> redisConfiguration)
    {
        hostBuilder.UsePangolin(siloBuilder =>
        {
            siloBuilder
                .AddReminders()
                .AddLogStorageBasedLogConsistencyProvider()
                .UseRedisClustering(options => redisConfiguration(options.ConfigurationOptions))
                .UseRedisReminderService(options => redisConfiguration(options.ConfigurationOptions))
                .AddRedisGrainStorage("pangolin", options => redisConfiguration(options.ConfigurationOptions));
        });

        return hostBuilder;
    }
}