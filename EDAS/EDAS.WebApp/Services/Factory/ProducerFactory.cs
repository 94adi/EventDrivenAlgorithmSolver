using EDAS.WebApp.Models;
using EDAS.WebApp.Services.Producers;

namespace EDAS.WebApp.Services.Factory;

public class ProducerFactory : IProducerFactory
{
    private readonly RabbitMQClientService _rabbitMQClientService;
    private readonly BrokerConfig _brokerConfig;
    private readonly QueueConfigCollection _queueConfigCollection;

    public ProducerFactory(RabbitMQClientService rabbitMQClientService,
        IOptions<BrokerConfig> brokerConfigOptions,
        QueueConfigCollection queueConfigCollection)
    {
        _rabbitMQClientService = rabbitMQClientService;
        _brokerConfig = brokerConfigOptions.Value;
        _queueConfigCollection = queueConfigCollection;
    }

    public async Task<IProducerService> Create(ProducerFactoryConfig config)
    {
        var channel = await _rabbitMQClientService.GetChannelAsync();

        var exchangeConfig = new BrokerExchangeConfig
        {
            ExchangeName = _brokerConfig.ExchangeName,
            ExchangeType = _brokerConfig.ExchangeType,
            ExchangeKey = _queueConfigCollection.QueuesConfig[config.producerType].RoutingKey
        };

        return config.producerType switch
        {
            ProducerType.Combinatronics => new ProducerCombinatronicsService(channel, exchangeConfig),
            _ => throw new ArgumentException("ProducerFactory : Invalid option")
        };

    }
}
