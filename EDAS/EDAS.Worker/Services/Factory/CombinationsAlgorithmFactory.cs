namespace EDAS.Worker.Services.Factory;

public class CombinationsAlgorithmFactory : ICombinationsAlgorithmFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CombinationsAlgorithmFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public ICombinationAlgo Create(CombinationAlgoInput input)
    {
        return ActivatorUtilities.CreateInstance<CombinationAlgo>(_serviceProvider, input);
    }
}
