using ECommerce.Shared.Infrastructure.EventBus.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Shared.Infrastructure.RabbitMq;

public static class RabbitMqStartupExtensions
{
    /// <summary>
    /// IRabbitMqConnection 인터페이스를 DI 컨테이너에 등록합니다.
    /// 싱글톤으로 Connection을 관리
    /// </summary>
    /// <param name="services">Service 등록을 위함</param>
    /// <param name="configuration">RabbitMqOptions 을 Bind 하기 위함</param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services, IConfigurationManager configuration)
    {
        var rabbitMqOptions = new RabbitMqOptions();
        configuration.GetSection(RabbitMqOptions.RabbitMqSectionName).Bind(rabbitMqOptions);

        services.AddSingleton<IRabbitMqConnection>(new RabbitMqConnection(rabbitMqOptions));

        return services;
    }
    /// <summary>
    /// 모든 마이크로 서비스가 이벤트를 게시하는 것은 아니므로 게시자만 호출해야 하는 새 메서드
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitMqEventPublisher(this IServiceCollection services)
    {
        services.AddScoped<IEventBus, RabbitMqEventBus>();

        return services;
    }
    public static IServiceCollection AddRabbitMqSubscriberService(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.Configure<EventBusOptions>(configuration.GetSection(EventBusOptions.EventBusSectionName));

        services.AddHostedService<RabbitMqHostedService>();

        return services;
    }
}
