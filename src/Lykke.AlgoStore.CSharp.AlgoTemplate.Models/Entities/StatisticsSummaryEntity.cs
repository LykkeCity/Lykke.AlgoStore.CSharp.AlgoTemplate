﻿using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities
{
    public class StatisticsSummaryEntity : TableEntity
    {
        public string InstanceId { get; set; }

        public string Id { get; set; }

        public string AlgoInstanceTypeValue { get; set; }

        public AlgoInstanceType InstanceType
        {
            get
            {
                Enum.TryParse(AlgoInstanceTypeValue, out AlgoInstanceType type);
                return type;
            }
            set => AlgoInstanceTypeValue = value.ToString();
        }

        public int TotalNumberOfTrades { get; set; }

        public int TotalNumberOfStarts { get; set; }
    }
}
