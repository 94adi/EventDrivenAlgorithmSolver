using EDAS.WebApp.Models;

namespace EDAS.WebApp.Services.Producers;

public interface IProducerService
{
    Task SendMessageAsync(ProducerMessage message);
}
