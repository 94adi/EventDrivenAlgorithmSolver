using Newtonsoft.Json;

namespace EDAS.WebApp.Controllers;

public class AlgorithmController : Controller
{
    private readonly BrokerConfig _brokerConfig;
    private readonly QueueConfigCollection _queueConfigCollection;
    private readonly IProducerFactory _producerFactory;

    public AlgorithmController(
        IOptions<BrokerConfig> brokerConfigOption,
        QueueConfigCollection queueConfigCollection,
        IProducerFactory producerFactory)
    {
        _brokerConfig = brokerConfigOption.Value;
        _queueConfigCollection = queueConfigCollection;
        _producerFactory = producerFactory;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("[controller]/Backtracking/[action]")]
    public IActionResult Combinatronics()
    {
        
        var viewModel = new CombinatronicsVM();
        return View(viewModel);
    }

    [HttpPost]
    [Route("[controller]/Backtracking/[action]")]
    public async Task<IActionResult> Combinatronics([FromForm] CombinatronicsVM viewModel)
    {
        if (ModelState.IsValid)
        {
            var content = JsonConvert.SerializeObject(viewModel);

            var producerType = ProducerType.Combinatronics;

            var queueConfig = _queueConfigCollection.QueuesConfig[producerType];

            var producerFactoryConfig = new ProducerFactoryConfig(queueConfig, producerType);

            var producerService = await _producerFactory.Create(producerFactoryConfig);

            var message = new ProducerMessage
            {
                Message = content
            };

            await producerService.SendMessageAsync(message);

            return RedirectToAction(nameof(SubmissionSuccessful));
        }

        return View();
    }

    [HttpGet]
    public IActionResult SubmissionSuccessful()
    {
        return View();
    }
}
