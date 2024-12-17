using EDAS.WebApp.Models;
using EDAS.WebApp.Services.Producers;

namespace EDAS.WebApp.Services.Factory;

public interface IProducerFactory
{
    Task<IProducerService> Create(ProducerFactoryConfig config);
}
