namespace EDAS.Common.Models;

public class QueueConfig
{
    public string QueueName { get; set; }

    public string RoutingKey { get; set; }

    public string QueueDurable { get; set; }

    public string QueueExclusive { get; set; }

    public string QueueAutodelete { get; set; }

    public string AlgorithmType { get; set; }

}
