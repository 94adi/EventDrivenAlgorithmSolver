using RabbitMQ.Client;

namespace EDAS.Worker.Services.Queues
{
    public class CombinatronicsQueue : IRabbitMQueue
    {

        private readonly IChannel _channel;
        private readonly RabbitMqConfig _rabbitMqConfig;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;

        public CombinatronicsQueue(IChannel channel,
            RabbitMqConfig rabbitMqConfigOption,
            IServiceProvider serviceProvider,
            IMapper mapper)
        {
            _channel = channel;
            _rabbitMqConfig = rabbitMqConfigOption;
            _serviceProvider = serviceProvider;
            _mapper = mapper;
        }

        public async Task StartConsuming()
        {
            await _channel.ExchangeDeclareAsync(exchange: _rabbitMqConfig.ExchangeName,
                            type: _rabbitMqConfig.ExchangeType);

            bool queueDurable = bool.TryParse(_rabbitMqConfig.QueueDurable, out bool queueDurableResult);
            bool queueExclusive = bool.TryParse(_rabbitMqConfig.QueueExclusive, out bool queueExclusiveResult);
            bool autoDelete = bool.TryParse(_rabbitMqConfig.QueueAutodelete, out bool autoDeleteResult);

            await _channel.QueueDeclareAsync(
                queue: _rabbitMqConfig.QueueName,
                durable: queueDurable,
                exclusive: queueExclusive,
                autoDelete: autoDelete,
                arguments: null);

            await _channel.QueueBindAsync(queue: _rabbitMqConfig.QueueName,
                exchange: _rabbitMqConfig.ExchangeName,
                routingKey: _rabbitMqConfig.RoutingKey);

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
                catch (Exception e)
                {
                    //message will not be validated hence it will be sent by the broker again
                    //log exception here
                    return;
                }
            };

            await _channel.BasicConsumeAsync(queue: RabbitMQConfig.QUEUE_NAME,
                autoAck: false,
                consumer: consumer);
        }
    }
}
