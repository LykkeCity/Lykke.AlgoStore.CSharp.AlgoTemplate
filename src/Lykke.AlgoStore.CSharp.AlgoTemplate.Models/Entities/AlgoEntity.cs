﻿using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities
{
    public class AlgoEntity : TableEntity, IAlgo
    {
        public string AlgoId
        {
            get => RowKey;
            set { }
        }

        public string ClientId
        {
            get => PartitionKey;
            set { }
        }

        public string Name { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DatePublished { get; set; }


        public string Description { get; set; }
        public string AlgoVisibilityValue { get; set; }
        public string AlgoMetaDataInformationJSON { get; set; }

        public AlgoVisibility AlgoVisibility
        {
            get
            {
                Enum.TryParse(AlgoVisibilityValue, out AlgoVisibility visibility);
                return visibility;
            }
            set => AlgoVisibilityValue = value.ToString();
        }

        public static AlgoEntity Create(IAlgo algo)
        {
            AlgoEntity enitity = AutoMapper.Mapper.Map<AlgoEntity>(algo);

            enitity.PartitionKey = algo.ClientId;
            enitity.RowKey = algo.AlgoId;

            return enitity;
        }

        public static AlgoEntity CreateEntityWithAlgoIdPartionKey(IAlgo algo)
        {
            AlgoEntity enitity = AutoMapper.Mapper.Map<AlgoEntity>(algo);

            enitity.PartitionKey = algo.AlgoId;
            enitity.RowKey = algo.ClientId;

            return enitity;
        }
    }
}
