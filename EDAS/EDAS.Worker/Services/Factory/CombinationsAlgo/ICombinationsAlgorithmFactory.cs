using EDAS.BacktrackingCombinatronics;

namespace EDAS.Worker.Services.Factory.CombinationsAlgo;

public interface ICombinationsAlgorithmFactory
{
    ICombinationAlgo Create(CombinationAlgoInput input);
}
