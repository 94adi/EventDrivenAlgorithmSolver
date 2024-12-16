namespace EDAS.Worker.Services.Queues;

public interface IRabbitMQueue
{
    Task StartConsuming();
}
