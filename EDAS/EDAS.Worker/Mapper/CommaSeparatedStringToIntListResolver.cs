namespace EDAS.Worker.Mapper;

public class CommaSeparatedStringToIntListResolver : IValueResolver<CombinationsInputModel, CombinationsInput, List<int>>
{
    public List<int> Resolve(CombinationsInputModel source, 
        CombinationsInput destination, 
        List<int> destMember, 
        ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(source.ElementsCSV))
            return new List<int>();

        List<int> elements = new List<int>();

        try
        {
             elements = source.ElementsCSV
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();
        }
        catch
        {
            return new List<int>();
        }

        return elements;

    }
}
