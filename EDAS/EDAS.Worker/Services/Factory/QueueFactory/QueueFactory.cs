using EDAS.Worker.Models.Enums;
using EDAS.Worker.Services.Queues;

namespace EDAS.Worker.Services.Factory.QueueFactory
{
    public class QueueFactory : IQueueFactory
    {
        public IRabbitMQueue Generate(QueueFactoryConfig queueFactoryConfig)
        {
            return queueFactoryConfig.queueType switch
            {
                QueueType.Combinatronics => new CombinatronicsQueue(queueFactoryConfig.channel,
                                                                    queueFactoryConfig.rabbitMqConfig,
                                                                    queueFactoryConfig.serviceProvider,
                                                                    queueFactoryConfig.mapper),
                _ => throw new ArgumentException("QueueFactory: Invalid option")
            };
        }
    }
}
