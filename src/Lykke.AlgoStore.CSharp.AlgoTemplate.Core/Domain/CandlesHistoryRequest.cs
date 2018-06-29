using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
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
        /// The name of the indicator to request candles for
        /// </summary>
        public string IndicatorName { get; set; }

        /// <summary>
        /// The auth token of the algo instance
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// The start date of the history data.
        /// </summary>
        public DateTime From { get; set; }

        /// <summary>
        /// The end date of the history data.
        /// </summary>
        public DateTime To { get; set; }
    }
}
