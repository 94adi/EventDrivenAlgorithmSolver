using RabbitMQ.Client;
using System.Text;

namespace EDAS.WebApp.Services.Producers;

public class ProducerCombinatronicsService : IProducerService
{

    private readonly IChannel _channel;
    private readonly BrokerExchangeConfig _exchangeConfig;

    public ProducerCombinatronicsService(IChannel channel,
        BrokerExchangeConfig exchangeConfig)
    {
        _channel = channel;
        _exchangeConfig = exchangeConfig;
    }

    public async Task SendMessageAsync(ProducerMessage message)
    {
        var exchangeType = ExchangeTypeConverter.Convert(_exchangeConfig.ExchangeType);

        await _channel.ExchangeDeclareAsync(exchange: _exchangeConfig.ExchangeName,
            type: exchangeType);

        var body = Encoding.UTF8.GetBytes(message.Message);

        await _channel.BasicPublishAsync(exchange: _exchangeConfig.ExchangeName,
            routingKey: _exchangeConfig.ExchangeKey,
            body: body);
    }
}
