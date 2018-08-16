using AutoMapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper
{
    public class AutoMapperModelProfile : Profile
    {
        public AutoMapperModelProfile()
        {
            CreateMap<AlgoClientInstanceData, AlgoClientInstanceEntity>()
                .ForMember(dest => dest.RowKey, opt => opt.MapFrom(src => src.InstanceId))
                .ForMember(dest => dest.PartitionKey, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForMember(dest => dest.ETag, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoInstanceStatusValue, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoInstanceTypeValue, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoMetaDataInformation, opt => opt.Ignore())
                .ForSourceMember(src => src.AlgoMetaDataInformation, opt => opt.Ignore());

            CreateMap<AlgoClientInstanceEntity, AlgoClientInstanceData>()
                .ForSourceMember(dest => dest.RowKey, opt => opt.Ignore())
                .ForSourceMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForSourceMember(dest => dest.PartitionKey, opt => opt.Ignore())
                .ForSourceMember(dest => dest.ETag, opt => opt.Ignore())
                .ForSourceMember(dest => dest.AlgoInstanceTypeValue, opt => opt.Ignore())
                .ForSourceMember(dest => dest.AlgoInstanceStatusValue, opt => opt.Ignore())
                .ForSourceMember(dest => dest.AlgoMetaDataInformation, opt => opt.Ignore())
                .ForMember(src => src.AlgoMetaDataInformation, opt => opt.Ignore())
                .ForMember(src => src.InstanceId, opt => opt.MapFrom(src => src.RowKey))
                .ForMember(src => src.FakeTradingTradingAssetBalance, opt => opt.Ignore())
                .ForMember(src => src.FakeTradingAssetTwoBalance, opt => opt.Ignore());

            CreateMap<AlgoClientInstanceData, AlgoInstanceStoppingEntity>()
                .ForMember(dest => dest.RowKey, opt => opt.Ignore())
                .ForMember(dest => dest.PartitionKey, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForMember(dest => dest.ETag, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoInstanceStatusValue, opt => opt.Ignore());

            CreateMap<AlgoInstanceStoppingEntity, AlgoInstanceStoppingData>()
                .ForSourceMember(dest => dest.RowKey, opt => opt.Ignore())
                .ForSourceMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForSourceMember(dest => dest.PartitionKey, opt => opt.Ignore())
                .ForSourceMember(dest => dest.ETag, opt => opt.Ignore())
                .ForSourceMember(dest => dest.AlgoInstanceStatusValue, opt => opt.Ignore())
                .ForMember(src => src.AlgoId, opt => opt.Ignore());

            CreateMap<AlgoClientInstanceData, AlgoInstanceTcBuildEntity>()
                .ForMember(dest => dest.RowKey, opt => opt.Ignore())
                .ForMember(dest => dest.PartitionKey, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForMember(dest => dest.ETag, opt => opt.Ignore());

            CreateMap<AlgoInstanceTcBuildEntity, AlgoInstanceTcBuildData>()
                .ForSourceMember(dest => dest.RowKey, opt => opt.Ignore())
                .ForSourceMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForSourceMember(dest => dest.PartitionKey, opt => opt.Ignore())
                .ForSourceMember(dest => dest.ETag, opt => opt.Ignore())
                .ForMember(src => src.AlgoId, opt => opt.Ignore());

            CreateMap<AlgoInstanceTrade, AlgoInstanceTradeEntity>()
                .ForMember(dest => dest.RowKey, opt => opt.Ignore())
                .ForMember(dest => dest.PartitionKey, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForMember(dest => dest.ETag, opt => opt.Ignore());

            CreateMap<AlgoInstanceTradeEntity, AlgoInstanceTrade>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RowKey));

            CreateMap<FunctionChartingUpdateEntity, FunctionChartingUpdateData>()
                .ForSourceMember(dest => dest.RowKey, opt => opt.Ignore())
                .ForSourceMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForSourceMember(dest => dest.PartitionKey, opt => opt.Ignore())
                .ForSourceMember(dest => dest.ETag, opt => opt.Ignore());

            CreateMap<StatisticsSummary, StatisticsSummaryEntity>()
                 .ForMember(dest => dest.RowKey, opt => opt.Ignore())
                 .ForMember(dest => dest.PartitionKey, opt => opt.Ignore())
                 .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                 .ForMember(dest => dest.ETag, opt => opt.Ignore());

            CreateMap<StatisticsSummaryEntity, StatisticsSummary>()
                  .ForSourceMember(src => src.ETag, opt => opt.Ignore())
                  .ForSourceMember(src => src.PartitionKey, opt => opt.Ignore())
                  .ForSourceMember(src => src.RowKey, opt => opt.Ignore())
                  .ForSourceMember(src => src.Timestamp, opt => opt.Ignore())
                  .ForMember(dest => dest.NetProfit, opt => opt.Ignore());

            CreateMap<AlgoEntity, IAlgo>()
                .ForMember(dest => dest.AlgoVisibility, opt => opt.Ignore());

            CreateMap<IAlgo, AlgoEntity>()
                .ForMember(dest => dest.PartitionKey, opt => opt.Ignore())
                .ForMember(dest => dest.RowKey, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForMember(dest => dest.ETag, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoVisibilityValue, opt => opt.Ignore());

            CreateMap<AlgoEntity, AlgoDataInformation>()
                .ForMember(dest => dest.AlgoId, opt => opt.MapFrom(src => src.RowKey))
                .ForMember(dest => dest.Rating, opt => opt.Ignore())
                .ForMember(dest => dest.RatedUsersCount, opt => opt.Ignore())
                .ForMember(dest => dest.UsersCount, opt => opt.Ignore())
                .ForMember(dest => dest.Author, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoMetaDataInformation, opt => opt.Ignore());

            CreateMap<AlgoData, IAlgo>();

            CreateMap<IAlgo, AlgoData>();
        }
    }
}
