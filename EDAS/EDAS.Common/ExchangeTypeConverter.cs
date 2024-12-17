using RabbitMQ.Client;

namespace EDAS.Common;

public static class ExchangeTypeConverter
{
    public static string Convert(string type)
    {
        return type.ToLower() switch
        {
            "direct" => ExchangeType.Direct,
            "topic" => ExchangeType.Topic,
            _ => throw new ArgumentException("Invalid argument")
        };
    }
}
