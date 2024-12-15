namespace EDAS.Worker.Services;

public class ConsumerService : BackgroundService
{
    private readonly RabbitMQClientService _rabbitMQClientService;
    private IChannel _channel;
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;

    public ConsumerService(RabbitMQClientService rabbitMQClientService,
        IMapper mapper,
        IServiceProvider serviceProvider)
    {
        _rabbitMQClientService = rabbitMQClientService;
        _mapper = mapper;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await _rabbitMQClientService.GetChannelAsync();

        await _channel.ExchangeDeclareAsync(exchange: RabbitMQConfig.EXCHANGE_NAME,
                                    type: RabbitMQConfig.EXCHANGE_TYPE);

        await _channel.QueueDeclareAsync(
            queue: RabbitMQConfig.QUEUE_NAME,
            durable: RabbitMQConfig.QUEUE_DURABLE,
            exclusive: RabbitMQConfig.QUEUE_EXCLUSIVE,
            autoDelete: RabbitMQConfig.QUEUE_AUTODELETE,
            arguments: null);

        await _channel.QueueBindAsync(queue: RabbitMQConfig.QUEUE_NAME, 
            exchange: RabbitMQConfig.EXCHANGE_NAME,
            routingKey: RabbitMQConfig.ROUTING_KEY);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            using var scope = _serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var emailService = scope.ServiceProvider.GetRequiredService<IEmailSender>();
            await emailService.SendEmailAsync("", "", "");

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var inputModel = JsonConvert.DeserializeObject<CombinationsInputModel>(message);
            try
            {
                var algorithmCommand = _mapper.Map<CombinationsInput>(inputModel);
                var combinationsOutput = await mediator.Send(algorithmCommand);

                //TO DO: use user's email address
                await emailService.SendEmailAsync("adrianrcotuna@gmail.com", 
                    "Solution", 
                    combinationsOutput.ToString());

                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch(Exception e)
            {
                //message will not be validated hence it will be sent by the broker again
                //log exception here
                return;
            }           
        };

        await _channel.BasicConsumeAsync(queue: RabbitMQConfig.QUEUE_NAME, 
            autoAck: false, 
            consumer: consumer);

        while (!stoppingToken.IsCancellationRequested) { }
    }
}
