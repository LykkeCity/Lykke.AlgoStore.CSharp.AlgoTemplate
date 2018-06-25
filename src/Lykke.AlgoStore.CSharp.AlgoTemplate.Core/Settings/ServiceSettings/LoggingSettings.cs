using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings
{
    public class LoggingSettings
    {
        /// <summary>
        /// Max time for which entries will be in the in-memory buffer before they will be persisted.
        /// This setting affects max latency before entry will be persisted.
        /// </summary>
        public TimeSpan MaxBatchLifetime { get; set; }

        /// <summary>
        /// Amount of entries that triggers batch persisting.
        /// </summary>
        public int BatchSizeThreshold { get; set; }
    }
}
