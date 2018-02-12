using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    /// <summary>
    /// A request for a historic candles
    /// </summary>
    public class CandlesHistoryRequest
    {
        /// <summary>
        /// The asset pair
        /// </summary>
        public string AssetPair { get; set; }

        /// <summary>
        /// The interval of the history data. <see cref="CandleTimeInterval"/>
        /// </summary>
        public CandleTimeInterval Interval { get; set; }

        /// <summary>
        /// The start data of the history data.
        /// </summary>
        public DateTime From { get; set; }
    }
}
