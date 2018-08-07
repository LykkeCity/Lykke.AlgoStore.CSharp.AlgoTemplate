using AutoMapper;
using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Charting;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Mapper
{
    public class AutoMapperAlgoProfile : Profile
    {
        public AutoMapperAlgoProfile()
        {
            CreateMap<Candle, CandleChartingUpdate>()
                .ForMember(dest => dest.InstanceId, opt => opt.Ignore())
                .ForMember(dest => dest.AssetPair, opt => opt.Ignore())
                .ForMember(dest => dest.CandleTimeInterval, opt => opt.Ignore());
        }
    }
}
