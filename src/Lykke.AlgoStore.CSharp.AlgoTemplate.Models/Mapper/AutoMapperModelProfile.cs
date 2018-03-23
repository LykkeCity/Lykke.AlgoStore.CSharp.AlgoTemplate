﻿using AutoMapper;
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
                .ForMember(src => src.InstanceId, opt => opt.MapFrom(src => src.RowKey));

            CreateMap<UserLog, UserLogEntity>()
                .ForMember(dest => dest.RowKey, opt => opt.Ignore())
                .ForMember(dest => dest.PartitionKey, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForMember(dest => dest.ETag, opt => opt.Ignore());

            CreateMap<Statistics, StatisticsEntity>()
                .ForMember(dest => dest.RowKey, opt => opt.Ignore())
                .ForMember(dest => dest.PartitionKey, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForMember(dest => dest.ETag, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoInstanceTypeValue, opt => opt.Ignore());

            CreateMap<Statistics, StatisticsSummaryEntity>()
                .ForMember(dest => dest.RowKey, opt => opt.Ignore())
                .ForMember(dest => dest.PartitionKey, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForMember(dest => dest.ETag, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoInstanceTypeValue, opt => opt.Ignore())
                .ForMember(dest => dest.TotalNumberOfTrades, opt => opt.Ignore())
                .ForMember(dest => dest.TotalNumberOfStarts, opt => opt.Ignore());

            CreateMap<StatisticsSummary, StatisticsSummaryEntity>()
                .ForMember(dest => dest.RowKey, opt => opt.Ignore())
                .ForMember(dest => dest.PartitionKey, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForMember(dest => dest.ETag, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoInstanceTypeValue, opt => opt.Ignore());

            CreateMap<StatisticsSummaryEntity, StatisticsSummary>()
                .ForSourceMember(src => src.ETag, opt => opt.Ignore())
                .ForSourceMember(src => src.PartitionKey, opt => opt.Ignore())
                .ForSourceMember(src => src.RowKey, opt => opt.Ignore())
                .ForSourceMember(src => src.Timestamp, opt => opt.Ignore());
        }
    }
}
