using EDAS.Worker.Models.Enums;

namespace EDAS.Worker.Models;

public class QueueFactoryConfig
{
    public readonly QueueType queueType;
    public readonly IChannel channel;
    public readonly RabbitMqConfig rabbitMqConfig;
    public readonly IServiceProvider serviceProvider;
    public readonly IMapper mapper;

    public QueueFactoryConfig(IChannel channel,
        RabbitMqConfig rabbitMqConfig,
        IServiceProvider serviceProvider,
        IMapper mapper,
        QueueType queueType)
    {
        this.channel = channel;
        this.rabbitMqConfig = rabbitMqConfig;
        this.serviceProvider = serviceProvider;
        this.mapper = mapper;
        this.queueType = queueType;
    }
}
