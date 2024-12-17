var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("EDASWebAppContextConnection") 
    ?? throw new InvalidOperationException("Connection string 'EDASWebAppContextConnection' not found.");

builder.Services.AddDbContext<EDASWebAppContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<EDASWebAppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
}).AddEntityFrameworkStores<EDASWebAppContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(MapperProfile));

var rabbitMqConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rabbitmqconfig.json");

builder.Configuration.AddJsonFile(rabbitMqConfigPath, optional: false, reloadOnChange: true);

builder.Services.Configure<BrokerConfig>(builder.Configuration.GetSection("RabbitMqConfig:Broker"));

var brokerConfig = builder.Configuration.GetSection("RabbitMqConfig:Broker").Get<BrokerConfig>();

var queuesDict = builder.Configuration
                        .GetSection("RabbitMqConfig:Queues")
                        .Get<Dictionary<string, QueueConfig>>();

var queuesConfigCollection = new QueueConfigCollection { QueuesConfig = queuesDict };

builder.Services.AddSingleton(sp =>
    new RabbitMQClientService(brokerConfig.Hostname, brokerConfig.Username, brokerConfig.Password));

builder.Services.AddScoped<IProducerFactory, ProducerFactory>();

builder.Services.AddSingleton(queuesConfigCollection);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();