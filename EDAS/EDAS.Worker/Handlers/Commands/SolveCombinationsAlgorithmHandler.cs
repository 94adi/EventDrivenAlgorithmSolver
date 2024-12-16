using EDAS.Worker.Services.Factory.CombinationsAlgo;

namespace EDAS.Worker.Handlers.Commands;

public class CombinationsInput : IRequest<CombinationsOutput>
{
    public int N { get; set; }

    public int K { get; set; }

    public List<int> Elements { get; set; }

    public CombinationsInput()
    {
        
    }
}

public class CombinationsOutput 
{ 
    public List<List<int>> Elements { get; set; }

    public CombinationsOutput()
    {
        
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var pair in Elements) 
        {
            string current = string.Join(", ", pair) + "\n";
            sb.Append(current);
        }

        string result = sb.ToString();

        return result;
    }
}


public class SolveCombinationsAlgorithmHandler(ICombinationsAlgorithmFactory combinationAlgoFactory,
    IMapper mapper)
    : IRequestHandler<CombinationsInput, CombinationsOutput>
{
    public async Task<CombinationsOutput> Handle(CombinationsInput request, CancellationToken cancellationToken)
    {
        if (request.Elements == null || request.Elements.Count == 0) 
        {
            throw new ArgumentException("Bad arguments");
        }

        var input = mapper.Map<CombinationsInput, CombinationAlgoInput>(request);

        var combinationService = combinationAlgoFactory.Create(input);

        var output = combinationService.Run();

        var result = mapper.Map<CombinationsOutput>(output);

        return result;
    }
}
