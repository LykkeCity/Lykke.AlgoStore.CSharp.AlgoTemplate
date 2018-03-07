using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService
{
    /// <summary>
    /// Represents the model for a request made to <see cref="ICandlesService"/>
    /// </summary>
    public class CandleServiceRequest
    {
        /// <summary>
        /// The id of the request. The id of the request can be used to distinguish the
        /// responses.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// The asset pair for which the data should be collected/updated
        /// </summary>
        public string AssetPair { get; set; }

        /// <summary>
        /// <see cref="CandleTimeInterval"/> for the data collection and update
        /// </summary>
        public CandleTimeInterval CandleInterval { get; set; }

        /// <summary>
        /// The start period for the data feed.
        /// </summary>
        public DateTime StartFrom { get; set; }

    }
}
