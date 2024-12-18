var builder = WebApplication.CreateBuilder(args);

var rabbitMqConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rabbitmqconfig.json");
builder.Configuration.AddJsonFile(rabbitMqConfigPath, optional: false, reloadOnChange: true);

builder.Services
    .RegisterConfigs(builder.Configuration)
    .RegisterServices();

var app = builder.Build();

app.Run();