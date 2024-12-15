var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(sp =>
    new RabbitMQClientService(RabbitMQConfig.HOSTNAME, 
    RabbitMQConfig.USERNAME, 
    RabbitMQConfig.PASSWORD));

//taken from user secrets local file
builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailConfig"));

builder.Services.AddHostedService<ConsumerService>();

builder.Services.AddScoped<ICombinationsAlgorithmFactory, CombinationsAlgorithmFactory>();

builder.Services.AddScoped<IEmailSender, EmailSender>();

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
