var builder = WebApplication.CreateBuilder(args);

//TODO: refactor
builder.Services.AddSingleton(sp =>
    new RabbitMQClientService(RabbitMQConfig.HOSTNAME, 
    RabbitMQConfig.USERNAME, 
    RabbitMQConfig.PASSWORD));

//taken from user secrets local file
builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailConfig"));

builder.Services.Configure<WorkerType>(builder.Configuration.GetSection("WorkerType"));

var workerType = builder.Configuration["WorkerType:Type"];

builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection($"RabbitMqConfig:{workerType}"));

builder.Services.AddHostedService<ConsumerService>();

builder.Services.AddScoped<ICombinationsAlgorithmFactory, CombinationsAlgorithmFactory>();

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddScoped<IQueueFactory, QueueFactory>();

var assembly = typeof(Program).Assembly; 

//NOTE: MediatR scope is singleton in BackgroundService
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
});

builder.Services.AddScoped(typeof(IRequestHandler<CombinationsInput, CombinationsOutput>), 
    typeof(SolveCombinationsAlgorithmHandler));

builder.Services.AddAutoMapper(typeof(MapperProfile));

var app = builder.Build();

app.Run();