namespace Basket.Service.Infrastructure.Data.Redis;

public static class RedisExtensions
{
    public static void AddRedisCache(this IServiceCollection services, IConfigurationManager configurationManager)
    {
        var redisOption = new RedisOptions();
        configurationManager.GetSection(RedisOptions.RedisSectionName).Bind(redisOption);

        services.AddStackExchangeRedisCache(options => options.Configuration = redisOption.Configuration);
    }
}
