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
        /// The auth token of the algo instance
        /// </summary>
        public string AuthToken { get; set; }

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
        /// Parameter which should mark if history update is ignored
        /// </summary>
        public bool IgnoreHistory { get; set; }

        /// <summary>
        /// The start period for the data feed.
        /// </summary>
        public DateTime StartFrom { get; set; }

        /// <summary>
        /// The end date for the data feed.
        /// </summary>
        public DateTime EndOn { get; set; }
    }
}
