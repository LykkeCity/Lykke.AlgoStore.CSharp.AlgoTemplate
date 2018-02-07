﻿using AutoMapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // To entities
            CreateMap<UserLog, UserLogEntity>();

            ForAllMaps((map, cfg) =>
            {
                if (map.DestinationType.IsSubclassOf(typeof(TableEntity)))
                {
                    cfg.ForMember("ETag", opt => opt.Ignore());
                    cfg.ForMember("PartitionKey", opt => opt.Ignore());
                    cfg.ForMember("RowKey", opt => opt.Ignore());
                    cfg.ForMember("Timestamp", opt => opt.Ignore());
                }
            });
        }
    }
}
