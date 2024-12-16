using EDAS.Worker.Models.Enums;
using EDAS.Worker.Services.Factory.QueueFactory;

namespace EDAS.Worker.Services;

public class ConsumerService : BackgroundService
{
    private readonly RabbitMQClientService _rabbitMQClientService;
    private IChannel _channel;
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqConfig _rabbitMqConfig;

    public ConsumerService(RabbitMQClientService rabbitMQClientService,
        IMapper mapper,
        IServiceProvider serviceProvider,
        IOptions<RabbitMqConfig> rabbitMqConfig)
    {
        _rabbitMQClientService = rabbitMQClientService;
        _mapper = mapper;
        _serviceProvider = serviceProvider;
        _rabbitMqConfig = rabbitMqConfig.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        _channel = await _rabbitMQClientService.GetChannelAsync();

        using var scope = _serviceProvider.CreateScope();

        var queueFactory = scope.ServiceProvider.GetRequiredService<IQueueFactory>();

        var queueType = ConvertStringToEnum(_rabbitMqConfig.AlgorithmType);

        var queueFacoryConfig = new QueueFactoryConfig(_channel, 
            _rabbitMqConfig, 
            _serviceProvider, 
            _mapper, 
            queueType);

        var queue = queueFactory.Generate(queueFacoryConfig);

        await queue.StartConsuming();

        while (!stoppingToken.IsCancellationRequested) { }
    }

    private QueueType ConvertStringToEnum(string enumString)
    {
        var algoTypeLower = enumString.ToLower();

        var algoType = algoTypeLower.Substring(0, 1).ToUpper() + algoTypeLower.Substring(1);

        Enum.TryParse<QueueType>(algoType, true, out QueueType queueType);

        return queueType;
    }
}
