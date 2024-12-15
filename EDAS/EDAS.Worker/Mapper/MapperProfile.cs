using AutoMapper;
using EDAS.BacktrackingCombinatronics;
using EDAS.Common.Models;
namespace EDAS.Worker.Mapper;

public class MapperProfile : Profile
{
	public MapperProfile()
	{
        CreateMap<CombinationsInputModel, CombinationsInput>()
             .ForMember(dest => dest.Elements, 
             opt => opt.MapFrom<CommaSeparatedStringToIntListResolver>());

        CreateMap<CombinationsInput, CombinationAlgoInput>();

        CreateMap<CombinationAlgoOutput, CombinationsOutput>()
            .ForMember(dest => dest.Elements,
                opt => opt.MapFrom(src => src.Solution));
    }
}
