using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Lykke.MatchingEngine.Connector.Abstractions.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    public class MarketOrderRequest
    {
        public string AssetPairId { get; set; }

        public string Asset { get; set; }

        public OrderAction OrderAction { get; set; }

        public double Volume { get; set; }
    }
}
