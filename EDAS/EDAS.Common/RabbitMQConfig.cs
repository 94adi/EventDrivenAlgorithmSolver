using RabbitMQ.Client;

namespace EDAS.Common;

//FOR DEV ONLY
public static class RabbitMQConfig
{
    public const string HOSTNAME = "localhost";

    public const string PORT = "15672";

    public const string USERNAME = "guest";

    public const string PASSWORD = "guest";

    public const string EXCHANGE_NAME = "direct_combinatronics";

    public const string EXCHANGE_TYPE = ExchangeType.Direct;

    public const string ROUTING_KEY = "backtracking_input";

    public const string QUEUE_NAME = "combinatronics_queue";

    public const bool QUEUE_DURABLE = true;

    public const bool QUEUE_EXCLUSIVE = false;

    public const bool QUEUE_AUTODELETE = false;
}