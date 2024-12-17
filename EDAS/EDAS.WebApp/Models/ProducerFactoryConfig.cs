using RabbitMQ.Client;

namespace EDAS.WebApp.Models;

public class ProducerFactoryConfig
{
    public readonly string producerType;
    public readonly QueueConfig queueConfig;


    public ProducerFactoryConfig(QueueConfig queueConfig,
        string producerType)
    {
        this.producerType = producerType;
        this.queueConfig = queueConfig;
    }
}
