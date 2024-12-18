namespace EDAS.Worker;

public static class DependencyInjection
{
    public static IServiceCollection RegisterConfigs(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailConfig>(configuration.GetSection("EmailConfig"));

        services.Configure<WorkerType>(configuration.GetSection("WorkerType"));

        var workerType = configuration["WorkerType:Type"];

        services.Configure<BrokerConfig>(configuration.GetSection("RabbitMqConfig:Broker"));

        var queuesDict = configuration
            .GetSection("RabbitMqConfig:Queues")
            .Get<Dictionary<string, QueueConfig>>();

        var queueConfig = queuesDict[workerType];

        var brokerConfig = configuration.GetSection("RabbitMqConfig:Broker").Get<BrokerConfig>();

        var queuesConfigCollection = new QueueConfigCollection { QueuesConfig = queuesDict };

        services.AddSingleton(provider =>
        {
            return new RabbitMqConfig
            {
                ExchangeName = brokerConfig.ExchangeName,
                ExchangeType = brokerConfig.ExchangeType,
                QueueName = queueConfig.QueueName,
                RoutingKey = queueConfig.RoutingKey,
                QueueDurable = queueConfig.QueueDurable,
                QueueExclusive = queueConfig.QueueExclusive,
                QueueAutodelete = queueConfig.QueueAutodelete,
                AlgorithmType = queueConfig.AlgorithmType
            }; ;
        });

        services.AddSingleton(sp =>
        {
            return new RabbitMQClientService(brokerConfig.Hostname,
                                    brokerConfig.Username,
                                    brokerConfig.Password);
        });

        services.AddSingleton(queuesConfigCollection);

        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddHostedService<ConsumerService>();

        services.AddScoped<ICombinationsAlgorithmFactory, CombinationsAlgorithmFactory>();

        services.AddScoped<IEmailSender, EmailSender>();

        services.AddScoped<IQueueFactory, QueueFactory>();

        var assembly = typeof(Program).Assembly;

        //NOTE: MediatR scope is singleton in BackgroundService
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
        });

        services.AddScoped(typeof(IRequestHandler<CombinationsInput, CombinationsOutput>),
            typeof(SolveCombinationsAlgorithmHandler));

        services.AddAutoMapper(typeof(MapperProfile));

        return services;
    }
}
